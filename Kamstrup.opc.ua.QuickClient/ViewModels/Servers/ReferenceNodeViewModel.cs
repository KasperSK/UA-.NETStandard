using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels.Servers
{
    class ReferenceNodeViewModel : TreeViewItemViewModel
    {

        public ReferenceNodeViewModel(TreeViewItem parent, ReferenceDescription referenceDescription) : base(parent)
        {
            this.ReferenceDescription = referenceDescription;
            ReferenceDescriptionViewModel = new ReferenceDescriptionViewModel(referenceDescription);
        }

        public ReferenceDescriptionViewModel ReferenceDescriptionViewModel { get; set; }
        public ReferenceDescription ReferenceDescription { get; set; }
    }
}
