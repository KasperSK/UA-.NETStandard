using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels.Servers
{
    class TreeViewItemLeafViewModel : PropertyChangedBase, TreeViewItem
    {
        private bool _isExpanded;
        private bool _isSelected;
        private TreeViewItem _parent;

        public TreeViewItemLeafViewModel(TreeViewItem parent)
        {
            _parent = parent;
        }

        public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value); }
        public bool IsExpanded { get => _isExpanded; set { Set(ref _isExpanded, value); if (_isExpanded && _parent != null) _parent.IsExpanded = true;  } }
    }
}
