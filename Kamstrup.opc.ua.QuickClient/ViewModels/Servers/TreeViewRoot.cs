using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels.Servers
{
    public class TreeViewRoot : Conductor<TreeViewItem>
    {
        public BindableCollection<TreeViewItem> Root { get; set; } = new BindableCollection<TreeViewItem>();

        public void ChangeSelection(TreeViewItem selection)
        {
            if (selection == null || selection == ActiveItem) return;

            ChangeActiveItem(selection, true);
        }
    }
}
