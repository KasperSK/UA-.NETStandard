using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Kamstrup.opc.ua.QuickClient.ViewModels.Servers
{
    public class ServerViewModel : TreeViewItemViewModel
    {
        private Kamstrup.opc.ua.client.KamstrupOpcClient client;

        public ServerViewModel(TreeViewItem treeViewItem, string endpointUrl) : base(treeViewItem)
        {
            client = new client.KamstrupOpcClient(endpointUrl, true, 60000);
            client.Setup("Kamstrup.Opc.Client");

            var refDes = client.GetServerTagList();

            var transformed = refDes.Select(x => new ReferenceNodeViewModel(this, x) { DisplayName = $"{x.BrowseName}" });
            var L = transformed.ToList();
            Items.AddRange(L);

            foreach (var r in L)
            {
                var rd = client.GetChildren(r.ReferenceDescription);
                var t = rd.Select(x => new ReferenceNodeViewModel(r, x) { DisplayName = $"{x.BrowseName}" });
                r.Items.AddRange(t);

            }
        }


        public void Connect()
        {
            client.Connect();
        }

        public void SetupTags()
        {
            var groupId = client.AddGroup("General", true, 0, 100);
            client.AddItem<ushort>(groupId, "Water.KKU.General.PLC.WatchDog", true);
            client.TagNotification += Client_TagNotification;
        }

        private void Client_TagNotification(string arg1, object arg2)
        {
            System.Diagnostics.Debug.WriteLine(arg1 + ": Value " + arg2.ToString());

            Task.Run(() => 
            {
                var u = Convert.ToUInt16(arg2);
                client.Write("Water.KKU.General.PLC.WatchDog", (u + 10).ToString());
            });
        }
    }
}
