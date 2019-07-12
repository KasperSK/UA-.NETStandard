using Caliburn.Micro;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels
{
    public class NodeIdViewModel : PropertyChangedBase
    {
        private NodeId _expandedNodeId;

        public NodeIdViewModel(NodeId expandedNodeId)
        {
            _expandedNodeId = expandedNodeId;
        }

        public ushort NamespaceIndex => _expandedNodeId.NamespaceIndex;
        public string IdType => _expandedNodeId.IdType.ToString();
        public string Identifier => _expandedNodeId.Identifier.ToString();
    }
}
