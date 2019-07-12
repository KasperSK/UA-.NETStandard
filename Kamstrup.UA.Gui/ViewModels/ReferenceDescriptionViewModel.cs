using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using ReactiveUI;

namespace Kamstrup.UA.Gui.ViewModels
{
    public class ReferenceDescriptionViewModel : ReactiveObject
    {
        private ReferenceDescription _referenceDescription;

        public ReferenceDescriptionViewModel(ReferenceDescription referenceDescription)
        {
            _referenceDescription = referenceDescription;
        }

        public NodeIdViewModel ReferenceTypeId => new NodeIdViewModel(_referenceDescription.ReferenceTypeId);
        public bool IsForwardReference => _referenceDescription.IsForward;
        public ExtendedNodeIdViewModel NodeId => new ExtendedNodeIdViewModel(_referenceDescription.NodeId);
        public string BrowseName => _referenceDescription.BrowseName.ToString();
        public string DisplayName => _referenceDescription.DisplayName.Text;
        public string NodeClass => _referenceDescription.NodeClass.ToString();
        public ExtendedNodeIdViewModel TypeDefinition => new ExtendedNodeIdViewModel(_referenceDescription.TypeDefinition);
    }
}
