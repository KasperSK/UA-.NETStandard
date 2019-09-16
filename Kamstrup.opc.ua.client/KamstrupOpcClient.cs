using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.client
{
    public class KamstrupOpcClient : IDisposable
    {
        private class opcItem
        {
            public MonitoredItem MonitoredItem { get; set; }
            public string ItemValue { get; set; }
            public Type ItemType { get; set; }

        }

        private Session session;
        private SessionReconnectHandler reconnectHandler;
        private string endPointUrl;
        private EndpointDescription endpointDescription;
        private ConfiguredEndpoint configuredEndpoint;
        private Subscription subscription;
        private Dictionary<string, uint> nodeNameHandle;
        private Dictionary<string, opcItem> nodeNameMonitor;
        private ApplicationConfiguration applicationConfiguration;
        private ApplicationInstance applicationInstance;
        private bool doIHaveCertificate;
        private bool autoAcceptCertificates;
        private ushort NAMESPACE = 2;

        public event Action<string, object> TagNotification;

        public KamstrupOpcClient(string endpointUrl, bool acceptAllCertificates, int stopTimeout)
        {
            this.endPointUrl = endpointUrl;
            //endpointDescription = CoreClientUtils.SelectEndpoint(this.endPointUrl, false);
            nodeNameHandle = new Dictionary<string, uint>();
            nodeNameMonitor = new Dictionary<string, opcItem>();
        }

        public void Setup(string configSectionName)
        {
            applicationInstance = new ApplicationInstance
            {
                ApplicationName = "KamstrupOpcClient",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = configSectionName,
            };

            applicationConfiguration = applicationInstance.LoadApplicationConfiguration(false).Result;
            doIHaveCertificate = applicationInstance.CheckApplicationInstanceCertificate(false, 0).Result;

            if (doIHaveCertificate)
            {
                applicationConfiguration.ApplicationUri = Utils.GetApplicationUriFromCertificate(applicationConfiguration.SecurityConfiguration.ApplicationCertificate.Certificate);
                autoAcceptCertificates = applicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates;
            }

            applicationConfiguration.CertificateValidator.CertificateValidation += CertificateValidation;

            endpointDescription = CoreClientUtils.SelectEndpoint(this.endPointUrl, false);
            var endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);
            configuredEndpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            session = Session.Create(applicationConfiguration, configuredEndpoint, false, "Kamstrup OPC Client", 60000, null, null).Result;
            session.KeepAlive += KeepAlive;

            subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 1000 };
            subscription.DefaultItem.Notification += OnNotification;
            session.AddSubscription(subscription);

            session.FetchNamespaceTables();

            var ns = session.NamespaceUris;
        }

        public void Connect()
        {
            subscription.Create();
        }

        public void Disconnect()
        {
            session.RemoveSubscription(subscription);
        }

        private bool IsConnected()
        {
            return subscription.Created;
        }

        public void Write<T>(string id, T value)
        {
            var wvq = new WriteValueCollection() { new WriteValue() { AttributeId = Opc.Ua.Attributes.Value, NodeId = new NodeId(id, NAMESPACE), Value = new DataValue(new Variant((T)value)) } };
            session.Write(null, wvq, out StatusCodeCollection statusCodes, out DiagnosticInfoCollection diag);
        }

        public T Read<T>(string id) //TODO Report errors?
        {
            var rvq = new ReadValueIdCollection() { new ReadValueId() { AttributeId = Opc.Ua.Attributes.Value, NodeId = new NodeId(id, NAMESPACE) } };
            session.Read(null, 10, TimestampsToReturn.Both, rvq, out DataValueCollection dvc, out DiagnosticInfoCollection di);

            return dvc.First().GetValue<T>(default(T));
        }

        public bool Write(string id, string value)
        {
            var status = nodeNameMonitor.TryGetValue(id, out opcItem item);
            
            var u = Convert.ChangeType(value, item.ItemType);
            var wvq = new WriteValueCollection() { new WriteValue() { AttributeId = Opc.Ua.Attributes.Value, NodeId = item.MonitoredItem.ResolvedNodeId, Value = new DataValue(new Variant(u)) } };
            session.Write(null, wvq, out StatusCodeCollection statusCodes, out DiagnosticInfoCollection diag);

            return status;
        } 

        public string Read(string id)
        {
            nodeNameMonitor.TryGetValue(id, out opcItem item);
            return item.ItemValue;
        }

        public string SyncRead(string id)
        {
            var node = nodeNameMonitor.TryGetValue(id, out opcItem toRead);

            var rvq = new ReadValueIdCollection() { new ReadValueId() { AttributeId = Opc.Ua.Attributes.Value, NodeId = toRead.MonitoredItem.ResolvedNodeId} };
            session.Read(null, 10, TimestampsToReturn.Both, rvq, out DataValueCollection dvc, out DiagnosticInfoCollection di);

            return dvc.First().GetValue(typeof(object)).ToString();
        }

        public int AddGroup(string name, bool active, int deadband, int updateRate)
        {
            var newSubscription = new Subscription(session.DefaultSubscription, true);
            newSubscription.DisplayName = name;
            newSubscription.PublishingInterval = updateRate;
            newSubscription.PublishingEnabled = active;

            session.AddSubscription(newSubscription);
            newSubscription.Create();

            return (int)newSubscription.Id;
        }

        public string AddItem<T>(int groupHandle, string name, bool requestEvent)
        {
            var subscription = session.Subscriptions.Where(x => x.Id == (uint)groupHandle).First();
            var newMonitoredItem = new MonitoredItem(subscription.DefaultItem, true) { DisplayName = name, StartNodeId = new NodeId(name, NAMESPACE), MonitoringMode = requestEvent ? MonitoringMode.Reporting : MonitoringMode.Sampling };
            nodeNameHandle.Add(name, newMonitoredItem.ClientHandle);
            nodeNameMonitor.Add(name, new opcItem { MonitoredItem = newMonitoredItem, ItemValue = "", ItemType = typeof(T) });
            subscription.AddItem(newMonitoredItem);
            subscription.ApplyChanges();

            return SyncRead(name);
        }

        public ReferenceDescriptionCollection GetServerTagList()
        {
            ReferenceDescriptionCollection result = new ReferenceDescriptionCollection();
            try
            {
                ReferenceDescriptionCollection referenceDescriptions = session.FetchReferences(ObjectIds.ObjectsFolder);
                ReferenceDescriptionCollection additionalReferenceDescriptions = new ReferenceDescriptionCollection();
                result.AddRange(referenceDescriptions);
                for (var i = 0; i < referenceDescriptions.Count; i++)
                {
                    try
                    {
                        var refDef = FetchReferences(ExpandedNodeId.ToNodeId(referenceDescriptions[i].NodeId, session.NamespaceUris));
                        foreach (var r in refDef)
                        {
                            var item = referenceDescriptions.FirstOrDefault(x => x.NodeId == r.NodeId);
                            if (item == null)
                                referenceDescriptions.Add(r);
                            additionalReferenceDescriptions.Add(r);
                        }
                    }catch(Exception ex)
                    {
                        var e = new Exception($"{i}: {referenceDescriptions[i].NodeId}", ex);
                        e.Data.Add($"{i}", referenceDescriptions[i]);
                        throw e;
                    }
                    //Recurse(rd, result);
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public ReferenceDescriptionCollection GetChildren(ReferenceDescription referenceDescription)
        {
            var refDef = FetchReferences(ExpandedNodeId.ToNodeId(referenceDescription.NodeId, session.NamespaceUris));

            return refDef;
        }

        private ReferenceDescriptionCollection FetchReferences(NodeId nodeId)
        {
            byte[] continuationPoint;
            ReferenceDescriptionCollection descriptions;

            session.Browse(
                null,
                null,
                nodeId,
                0,
                BrowseDirection.Forward,
                null,
                true,
                0,
                out continuationPoint,
                out descriptions);

            // process any continuation point.
            while (continuationPoint != null)
            {
                byte[] revisedContinuationPoint;
                ReferenceDescriptionCollection additionalDescriptions;

                session.BrowseNext(
                    null,
                    false,
                    continuationPoint,
                    out revisedContinuationPoint,
                    out additionalDescriptions);

                continuationPoint = revisedContinuationPoint;

                descriptions.AddRange(additionalDescriptions);
            }

            return descriptions; 
        }

        private void Recurse(ReferenceDescription referenceDescription, ReferenceDescriptionCollection result) //TODO Rethink this.
        {
            ReferenceDescriptionCollection referenceDescriptions = session.FetchReferences(ObjectIds.ObjectsFolder);
            Byte[] continuationPoint;

            session.Browse(
                null,
                null,
                ExpandedNodeId.ToNodeId(referenceDescription.NodeId, session.NamespaceUris),
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HierarchicalReferences,
                true,
                (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                out continuationPoint,
                out referenceDescriptions);

            result.AddRange(referenceDescriptions);

            Byte[] revisedContinuationPoint;

            if (continuationPoint != null)
            {
                session.BrowseNext(
                    requestHeader: null,
                    releaseContinuationPoint: false,
                    continuationPoint: continuationPoint,
                    revisedContinuationPoint: out revisedContinuationPoint,
                    references: out referenceDescriptions);

                result.AddRange(referenceDescriptions);
            }
            //foreach (var rd in referenceDescriptions)
            //{
            //    //Console.WriteLine(space + " + {0}, {1}, {2}, {3}, {4}, {5}", rd.DisplayName, rd.BrowseName, rd.NodeClass, rd.NodeId, rd.ReferenceTypeId, rd.NodeId.IdType);
            //    if (rd.DisplayName.Text.Contains("_Hint") || rd.NodeClass == NodeClass.Variable)
            //        continue;
            //    //Thread.Sleep(500);
            //    Recurse(rd, result);
            //}
            //foreach (var rd in rdc)
            //{
            //    if (rd.NodeClass == NodeClass.VariableType || rd.NodeClass == NodeClass.Variable || rd.NodeClass == NodeClass.Object)
            //        return;
            //    var r = session.FetchReferences(new NodeId(rd.NodeId.Identifier, rd.NodeId.NamespaceIndex));
            //    result.AddRange(r);
            //    if(r.Count != 0)
            //        Recurse(r, result);
            //}
        }

        private void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            var value = item.DequeueValues();
            TagNotification(item.DisplayName, value.First().Value);
        }

        private void KeepAlive(Session sender, KeepAliveEventArgs e)
        {
            if (e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                if (reconnectHandler == null)
                {
                    reconnectHandler = new SessionReconnectHandler();
                    reconnectHandler.BeginReconnect(sender, 10000, ReconnectComplete);
                }
            }
        }

        private void ReconnectComplete(object sender, EventArgs e)
        {
            if (!(sender == reconnectHandler))
            {
                return;
            }

            session = reconnectHandler.Session;
            reconnectHandler.Dispose();
            reconnectHandler = null;
        }

        private void CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            e.Accept = autoAcceptCertificates; //TODO: Have event to ask user to accept.
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    subscription.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~KamstrupOpcClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
