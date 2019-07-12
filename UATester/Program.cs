using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;

namespace UATester
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random();
            string endpoint = "";
            //endpoint = "opc.tcp://localhost:49320";
            //endpoint = "opc.tcp://DK-KSK-HP800SFF.kamstrup.dk:49321"; //TODO: Anonymous log in must be enabled for this to work.
            endpoint = "opc.tcp://192.168.250.1";

            KamstrupOpcClient kamstrupOpcClient = new KamstrupOpcClient(endpoint, true, 10000);
            kamstrupOpcClient.TagNotigication += (s, o) => Console.WriteLine(s + ": " + o);
            kamstrupOpcClient.Setup("Kamstrup.Opc.Client");
            kamstrupOpcClient.Connect();
            //kamstrupOpcClient.AddItem("WATER.KKU.GENERAL.PLC.WATCHDOG", true);
            kamstrupOpcClient.AddItem("WATCHDOG", true);
            kamstrupOpcClient.AddItem("WATER.KKU.GENERAL.PC.WATCHDOG", false);

            kamstrupOpcClient.Write("WATER.KKU.GENERAL.PLC.WATCHDOG", (ushort)r.Next());

            Console.WriteLine("Press key to read");
            Console.ReadLine();
            Console.WriteLine("Press enter to exit");
            bool stop = true;
            Task.Run(() =>
            {
                Console.ReadLine();
                stop = false;
            });

            while (stop)
            {
                //Console.WriteLine("WATCHDOG: " + kamstrupOpcClient.Read<ushort>("WATCHDOG"));
                Console.WriteLine("WATER.KKU.GENERAL.PC.WATCHDOG: " + kamstrupOpcClient.Read<ushort>("WATER.KKU.GENERAL.PC.WATCHDOG"));
                Thread.Sleep(1000);
            }

            kamstrupOpcClient.Disconnect();
        }

        public class KamstrupOpcClient : IDisposable
        {
            private Session session;
            private SessionReconnectHandler reconnectHandler;
            private string endPointUrl;
            private EndpointDescription endpointDescription;
            private ConfiguredEndpoint configuredEndpoint;
            private Subscription subscription;
            private Dictionary<string, uint> nodeNameHandle;
            private ApplicationConfiguration applicationConfiguration;
            private ApplicationInstance applicationInstance;
            private bool doIHaveCertificate;
            private bool autoAcceptCertificates;
            private ushort NAMESPACE = 4;

            public event Action<string, object> TagNotigication;
            
            public KamstrupOpcClient(string endpointUrl, bool acceptAllCertificates, int stopTimeout)
            {
                this.endPointUrl = endpointUrl;
                endpointDescription = CoreClientUtils.SelectEndpoint(this.endPointUrl, false);
                nodeNameHandle = new Dictionary<string, uint>();
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

                var endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);
                configuredEndpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                session = Session.Create(applicationConfiguration, configuredEndpoint, false, "Kamstrup OPC Client", 60000, null, null).Result;
                session.KeepAlive += KeepAlive;

                subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 250 };
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

            public void AddItem(string name, bool requestEvent)
            {
                var newMonitoredItem = new MonitoredItem(subscription.DefaultItem, true) { DisplayName = name, StartNodeId = new NodeId(name, NAMESPACE), MonitoringMode = requestEvent ? MonitoringMode.Reporting : MonitoringMode.Sampling };
                nodeNameHandle.Add(name, newMonitoredItem.ClientHandle);
                subscription.AddItem(newMonitoredItem);
                subscription.ApplyChanges();
            }

            private void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
            {
                var value = item.DequeueValues();
                TagNotigication(item.DisplayName, value.First().Value);
            }

            private void KeepAlive(Session sender, KeepAliveEventArgs e)
            {
                if(e.Status != null && ServiceResult.IsNotGood(e.Status))
                {
                    if(reconnectHandler == null)
                    {
                        reconnectHandler = new SessionReconnectHandler();
                        reconnectHandler.BeginReconnect(sender, 10000, ReconnectComplete);
                    }
                }
            }

            private void ReconnectComplete(object sender, EventArgs e)
            {
                if(!(sender == reconnectHandler))
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
}
