using Caliburn.Micro;
using Kamstrup.opc.ua.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels
{
    public class MainWindowViewModel : Servers.TreeViewRoot
    {

        private KamstrupOpcClient kamstrupOpcClient;

        public MainWindowViewModel() 
        {         
            Root.Add(new Servers.ServerViewModel(null, "opc.tcp://localhost:49320") { DisplayName = "LocalUAServer" });
            Root.Add(new Servers.ServerViewModel(null, "opc.tcp://DK-KSK-HP800SFF.kamstrup.dk:49321") { DisplayName = "RemoteUAServer" });
            Root.Add(new Servers.ServerViewModel(null, "opc.tcp://172.20.17.69") { DisplayName = "PLCUAServer" });
        }

        public async void TestEndpoint(string endpointUri)
        {
            var endpoint = await Task.Run(() => Opc.Ua.Client.CoreClientUtils.SelectEndpoint(endpointUri, false));

            if (endpoint != null)
            {
                EndpointDescription = new EndpointDescriptionViewModel(endpoint);
            }
        }

        public async void Connect(string endpointUri)
        {
            foreach( var item in Root)
            {
                if(item is Servers.ServerViewModel)
                {
                    ((Servers.ServerViewModel)item).Connect();
                }
            }


            var server = Root[0] as Servers.ServerViewModel;

            server.SetupTags();
            //kamstrupOpcClient = new KamstrupOpcClient(endpointUri, true, 60000);
            //kamstrupOpcClient.Setup("Kamstrup.Opc.Client");

            //var refDes = await Task.Run(() => kamstrupOpcClient.GetServerTagList());

            //var transformed = refDes.Select(x => new ReferenceDescriptionViewModel(x));
            //referenceDescriptionViewModels.AddRange(transformed);
        }

        public BindableCollection<ReferenceDescriptionViewModel> referenceDescriptionViewModels { get; set; } = new BindableCollection<ReferenceDescriptionViewModel>();

        private EndpointDescriptionViewModel _endPointDescription;
        public EndpointDescriptionViewModel EndpointDescription
        {
            get
            {
                return _endPointDescription;
            }
            set
            {
                if (value != _endPointDescription)
                {
                    _endPointDescription = value;
                    NotifyOfPropertyChange(() => EndpointDescription);
                }
            }
        }
    }
}
