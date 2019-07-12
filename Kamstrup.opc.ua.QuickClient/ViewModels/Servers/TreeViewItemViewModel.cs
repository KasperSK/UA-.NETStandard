using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Kamstrup.opc.ua.QuickClient.ViewModels.Servers
{
    public class TreeViewItemViewModel : Conductor<TreeViewItem>.Collection.OneActive, TreeViewItem
    {
        private readonly TreeViewItem _parent;
        private bool _isExpanded;
        private bool _isSelected;

        public TreeViewItemViewModel(TreeViewItem parent)
        {
            _parent = parent;
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                Set(ref _isExpanded, value);
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

    }
}
