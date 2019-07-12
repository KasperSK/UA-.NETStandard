using Caliburn.Micro;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels
{
    public class EndpointDescriptionViewModel : PropertyChangedBase
    {
        private EndpointDescription _endpointDescription;

        public EndpointDescriptionViewModel(EndpointDescription endpointDescription)
        {
            _endpointDescription = endpointDescription;
            ApplicationDescriptionViewModel = new ApplicationDescriptionViewModel(endpointDescription.Server);
        }

        public ApplicationDescriptionViewModel ApplicationDescriptionViewModel { get; set; }

        public string EndPointUrl => _endpointDescription.EndpointUrl;
        public string ServerName => _endpointDescription.Server.ApplicationName.Text;

        public List<UserTokenPolicy> UserPolicies => _endpointDescription.UserIdentityTokens;

        public string SecurityMode => _endpointDescription.SecurityMode.ToString();
    }
}
