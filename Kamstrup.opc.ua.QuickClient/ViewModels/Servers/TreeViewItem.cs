using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels.Servers
{
    public interface TreeViewItem
    {
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
    }
}
