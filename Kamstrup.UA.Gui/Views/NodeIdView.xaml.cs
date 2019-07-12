using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using System.Reactive.Disposables;
using Kamstrup.UA.Gui.ViewModels;

namespace Kamstrup.UA.Gui.Views
{
    /// <summary>
    /// Interaction logic for NodeIdView.xaml
    /// </summary>
    public partial class NodeIdView : ReactiveUserControl<NodeIdViewModel>
    {
        public NodeIdView()
        {
            InitializeComponent();
            this.WhenActivated(disposableregistration =>
            {

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.NamespaceIndex,
                    view => view.NamespaceIndexRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.IdType,
                    view => view.IdTypeRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.Identifier,
                    view => view.IdentifierRun)
                    .DisposeWith(disposableregistration);

            });
        }
    }
}
