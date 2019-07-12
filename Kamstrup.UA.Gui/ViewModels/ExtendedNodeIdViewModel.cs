using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using ReactiveUI;

namespace Kamstrup.UA.Gui.ViewModels
{
    public class ExtendedNodeIdViewModel
    {
        private ExpandedNodeId _expandedNodeId;

        public ExtendedNodeIdViewModel(ExpandedNodeId expandedNodeId)
        {
            _expandedNodeId = expandedNodeId;
        }

        public ushort NamespaceIndex => _expandedNodeId.NamespaceIndex;
        public string IdType => _expandedNodeId.IdType.ToString();
        public string Identifier => _expandedNodeId.Identifier.ToString();
        public string NamespaceUri => _expandedNodeId.NamespaceUri;
        public string ServerIndex => _expandedNodeId.ServerIndex.ToString();

    }
}
