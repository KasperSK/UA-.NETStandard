using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using ReactiveUI;

namespace Kamstrup.UA.Gui.ViewModels
{
    public class EndPointDescriptionViewModel : ReactiveObject
    {
        private EndpointDescription _endpointDescription;

        public EndPointDescriptionViewModel(EndpointDescription endpointDescription)
        {
            _endpointDescription = endpointDescription;
            ApplicationDescriptionViewModel = new ApplicationDescriptionViewModel(endpointDescription.Server);
        }

        public ApplicationDescriptionViewModel ApplicationDescriptionViewModel;

        public string EndPointUrl => _endpointDescription.EndpointUrl;
        public string ServerName => _endpointDescription.Server.ApplicationName.Text;

        public List<UserTokenPolicy> UserPolicies => _endpointDescription.UserIdentityTokens;

        public string SecurityMode => _endpointDescription.SecurityMode.ToString();
    }


}
