using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Opc.Ua;

namespace Kamstrup.UA.Gui.ViewModels
{
    public class ApplicationDescriptionViewModel : ReactiveObject
    {
        private ApplicationDescription _applicationDescription;
        public ApplicationDescriptionViewModel(ApplicationDescription applicationDescription)
        {
            _applicationDescription = applicationDescription;
        }

        public string ApplicationUri => _applicationDescription.ApplicationUri;
        public string ProductUri => _applicationDescription.ProductUri;
        public string ApplicationName => _applicationDescription.ApplicationName.Text;
        public string ApplicationType => _applicationDescription.ApplicationType.ToString();
        public string GatewayServerUri => _applicationDescription.GatewayServerUri;
        public string DiscoveryProfileUri => _applicationDescription.DiscoveryProfileUri;
        public List<string> DiscoveryUrls => _applicationDescription.DiscoveryUrls;
    }
}
