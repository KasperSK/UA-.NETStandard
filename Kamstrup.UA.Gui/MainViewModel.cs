using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Opc.Ua;
using Kamstrup.UA.Gui.ViewModels;

namespace Kamstrup.UA.Gui
{
    public class MainViewModel : ReactiveUI.ReactiveObject
    {
        private string _endPoint;
        public string EndPoint
        {
            get => _endPoint;
            set => this.RaiseAndSetIfChanged(ref _endPoint, value);
        }

        private string _error;
        public string Error
        {
            get => _error;
            set => this.RaiseAndSetIfChanged(ref _error, value);
        }

        private readonly ReactiveCommand<string, IEnumerable<EndPointDescriptionViewModel>> _findEndPointsCmd;
        private readonly ObservableAsPropertyHelper<IEnumerable<EndPointDescriptionViewModel>> _foundEndpoints;
        public IEnumerable<EndPointDescriptionViewModel> FoundEndPoints => _foundEndpoints.Value;

        public MainViewModel()
        {
            //ConnectCommand = ReactiveCommand.CreateFromTask((ct) => Connect());

            //_findEndPointsCmd = ReactiveCommand
            //                               .CreateFromTask<string, IEnumerable<EndPointDescriptionViewModel>>((s, ct) => FindEndpoints(s, ct));

            //this.WhenAnyValue(x => x.EndPoint)
            //    .Throttle(TimeSpan.FromMilliseconds(800))
            //    .Select(endPoint => endPoint?.Trim())
            //    .DistinctUntilChanged()
            //    .Do((s) => Error = string.Empty)
            //    .Where(endPoint => !string.IsNullOrWhiteSpace(endPoint))
            //    .InvokeCommand(FindEndPointsCmd);



            //_foundEndpoints =
            //    this.FindEndPointsCmd
            //    .ToProperty(this, x => x.FoundEndPoints);

            //_serverNodes =
            //    this.ConnectCommand
            //    .ToProperty(this, x => x.ServerNodes);

            //FindEndPointsCmd.ThrownExceptions.Subscribe(error =>
            //{
            //    Error = error.Message;

            //});


        }

        ReactiveCommand<string, IEnumerable<EndPointDescriptionViewModel>> FindEndPointsCmd => _findEndPointsCmd;

        private async Task<IEnumerable<EndPointDescriptionViewModel>> FindEndpoints(string endpoint, CancellationToken token)
        {

             var endpoints = await Task.Run(() => Opc.Ua.Client.CoreClientUtils.SelectEndpoint(endpoint, false));

            return new List<EndPointDescriptionViewModel>() { new EndPointDescriptionViewModel(endpoints) };
        }

        private readonly ObservableAsPropertyHelper<List<ReferenceDescriptionViewModel>> _serverNodes;
        public List<ReferenceDescriptionViewModel> ServerNodes => _serverNodes.Value;

        public ReactiveCommand<Unit, Task<List<ReferenceDescriptionViewModel>>> ConnectCommand;

        public async Task<List<ReferenceDescriptionViewModel>> Connect()
        {
            var opcClient = new Kamstrup.opc.ua.client.KamstrupOpcClient(EndPoint, true, 10000);
            opcClient.Setup("Kamstrup.Opc.Client");
            opcClient.Connect();

            var refDes = await Task.Run(() => opcClient.GetServerTagList());

            return refDes.Select(x => new ReferenceDescriptionViewModel(x)).ToList();
        }
    }
}
