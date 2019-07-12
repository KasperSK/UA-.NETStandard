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

namespace Kamstrup.UA.Gui.Views
{
    /// <summary>
    /// Interaction logic for EndPointDescriptionView.xaml
    /// </summary>
    public partial class EndPointDescriptionView : ReactiveUserControl<ViewModels.EndPointDescriptionViewModel>
    {
        public EndPointDescriptionView()
        {
            InitializeComponent();
            this.WhenActivated(disposableregistration =>
            {

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.EndPointUrl,
                    view => view.EndpointUrlRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.UserPolicies,
                    view => view.UserPoliciesListView.ItemsSource)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.SecurityMode,
                    view => view.SecurityModeRun)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.ApplicationDescriptionViewModel,
                    view => view.applicationDescriptionVMH.ViewModel)
                    .DisposeWith(disposableregistration);
            });
        }
    }
}
