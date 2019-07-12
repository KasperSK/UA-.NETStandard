﻿using Caliburn.Micro;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels
{
    public class ApplicationDescriptionViewModel : PropertyChangedBase
    {
        private ApplicationDescription _applicationDescription;
        public ApplicationDescriptionViewModel(ApplicationDescription applicationDescription)
        {
            _applicationDescription = applicationDescription;
        }

        public string ApplicationUri
        {
            get { return _applicationDescription.ApplicationUri; }
        }

        public string ProductUri => _applicationDescription.ProductUri;
        public string ApplicationName
        {
            get => _applicationDescription.ApplicationName.Text;
        }
        public string ApplicationType => _applicationDescription.ApplicationType.ToString();
        public string GatewayServerUri => _applicationDescription.GatewayServerUri;
        public string DiscoveryProfileUri => _applicationDescription.DiscoveryProfileUri;
        public List<string> DiscoveryUrls => _applicationDescription.DiscoveryUrls;
    }
}
