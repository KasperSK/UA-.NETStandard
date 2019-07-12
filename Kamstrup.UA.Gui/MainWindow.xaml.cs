using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Kamstrup.UA.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        private Kamstrup.opc.ua.client.KamstrupOpcClient KamstrupOpcClient;
        //private List<tag> tagList = new List<tag>();
        //private ObservableCollection<tag> tagList = new ObservableCollection<tag>();
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                    ViewModel => ViewModel.EndPoint,
                    View => View.endpoint.Text).DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    ViewModel => ViewModel.FoundEndPoints,
                    View => View.MonitoredTags.ItemsSource).DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    ViewModel => ViewModel.ServerNodes,
                    View => View.TagBrowser.ItemsSource).DisposeWith(disposableRegistration);
                

                this.OneWayBind(ViewModel,
                    ViewModel => ViewModel.Error,
                    View => View.Error.Text).DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.ConnectCommand,
                    view => view.ConnectButton).DisposeWith(disposableRegistration);
            }
            );
        }

        //public void IsEndpointsAvailable()
        //{
        //    try
        //    {
        //        KamstrupOpcClient = new opc.ua.client.KamstrupOpcClient(endpoint.Text, true, 10000);
        //        KamstrupOpcClient.Setup("Kamstrup.Opc.Client");

        //        KamstrupOpcClient.Connect();
        //        KamstrupOpcClient.TagNotification += UpdateMTI;
        //        ConnectedText.Content = "Connected";
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ////IsEndpointsAvailable();
            //var tags = KamstrupOpcClient.GetServerTagList(); // TODO Make tree datastructure for this.

            //TagBrowser.ItemsSource = tags.Where(x => x.NodeClass == Opc.Ua.NodeClass.Variable);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ReferenceDescription item = TagBrowser.SelectedItem as ReferenceDescription;
            //KamstrupOpcClient.AddItem(item.NodeId.Identifier.ToString(), true);

            //tagList.Add(new tag { DisplayName = item.NodeId.Identifier.ToString(), Value = 0 });

            //MonitoredTags.ItemsSource = tagList;
        }

        //private void UpdateMTI(string tag, object value)
        //{
        //    var ta = tagList.First(x => x.DisplayName == tag);
        //    tagList.Remove(ta);
        //    tagList.Add(new tag { DisplayName = tag, Value = value });


        //    //ICollectionView view = CollectionViewSource.GetDefaultView(MonitoredTags.ItemsSource);
        //    //view.Refresh();
        //    //view.

        //}

        //public class tag
        //{
        //    public string DisplayName { get; set; }
        //    public object Value { get; set; }
        //}
    }
}
