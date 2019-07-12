using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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

namespace Kamstrup.UA.Gui.Views
{
    /// <summary>
    /// Interaction logic for ApplicationDescriptionView.xaml
    /// </summary>
    public partial class ApplicationDescriptionView : ReactiveUserControl<ViewModels.ApplicationDescriptionViewModel>
    {
        public ApplicationDescriptionView()
        {
            InitializeComponent();
            this.WhenActivated(disposableregistration =>
            {

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.ApplicationUri,
                    view => view.ApplicationUriRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.ProductUri,
                    view => view.ProductUriRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.ApplicationName,
                    view => view.ApplicationNameRun)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
    viewmodel => viewmodel.ApplicationType,
    view => view.ApplicationTypeRun.Text)
    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.GatewayServerUri,
                    view => view.GatewayServerUriRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.DiscoveryProfileUri,
                    view => view.DiscoveryProfileUriRun.Text)
                    .DisposeWith(disposableregistration);
            });
        }
    }
}
