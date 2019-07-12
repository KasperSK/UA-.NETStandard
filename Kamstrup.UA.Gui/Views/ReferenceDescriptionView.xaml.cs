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
using Kamstrup.UA.Gui.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;


namespace Kamstrup.UA.Gui.Views
{
    /// <summary>
    /// Interaction logic for ReferenceDescriptionView.xaml
    /// </summary>
    public partial class ReferenceDescriptionView : ReactiveUserControl<ReferenceDescriptionViewModel>
    {
        public ReferenceDescriptionView()
        {
            InitializeComponent();
            this.WhenActivated(disposableregistration =>
            {

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.BrowseName,
                    view => view.BrowseNameRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.DisplayName,
                    view => view.DisplayNameRun.Text)
                    .DisposeWith(disposableregistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.NodeClass,
                    view => view.NodeClassRun)
                    .DisposeWith(disposableregistration);

                //this.OneWayBind(ViewModel,
                //    viewmodel => viewmodel.ReferenceTypeId,
                //    view => view.ReferenceTypeIdHost.ViewModel)
                //    .DisposeWith(disposableregistration);

                //this.OneWayBind(ViewModel,
                //    viewmodel => viewmodel.NodeId,
                //    view => view.NodeIdHost.ViewModel)
                //    .DisposeWith(disposableregistration);
            });
        }
    }
}
