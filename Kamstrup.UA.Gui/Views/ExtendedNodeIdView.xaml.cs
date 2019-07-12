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
    /// Interaction logic for ExtendedNodeIdView.xaml
    /// </summary>
    public partial class ExtendedNodeIdView : ReactiveUserControl<ExtendedNodeIdViewModel>
    {
        public ExtendedNodeIdView()
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

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.NamespaceUri,
                    view => view.NamespaceUriRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.ServerIndex,
                    view => view.ServerIndexRun)
                    .DisposeWith(disposableregistration);
            });
        }
    }
}
