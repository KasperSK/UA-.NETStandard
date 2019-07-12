using Caliburn.Micro;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels
{
    public class ExtendedNodeIdViewModel : PropertyChangedBase
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
