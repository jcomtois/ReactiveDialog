using System;
using System.Reactive.Linq;
using System.Threading;
using ReactiveUI;

namespace WpfApplication3
{
    public class TestViewModel : ReactiveObject, ITestViewModel
    {
        private static readonly Random _random = new Random();
        private readonly ObservableAsPropertyHelper<int> _counter;

        private readonly IDialogService _dialogService;
        private readonly ReactiveCommand _sayHelloCommand;
        private readonly ReactiveCommand _showDialogCommand;
        private string _hello;
        private string _rando;

        public TestViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _sayHelloCommand = new ReactiveCommand();
            _sayHelloCommand.Subscribe(x => Hello = DateTime.UtcNow.ToLocalTime().ToString());

            _showDialogCommand = new ReactiveCommand(
                this.WhenAny(x => x.Hello,
                             h => !string.IsNullOrWhiteSpace(h.Value)));

            _showDialogCommand.Subscribe(x =>
                                         {
                                             Hello = null;
                                             _dialogService.ShowADialog("Hello");
                                         });

            _counter = new ObservableAsPropertyHelper<int>(Observable.Generate(0, i => true, i => i + 1, i => i, i => TimeSpan.FromSeconds(.01)),
                                                           i => raisePropertyChanged("Counter"));
             
            _sayHelloCommand.RegisterAsyncAction(o =>
                                                 {
                                                     Thread.Sleep(2000);
                                                     Rando = _random.NextDouble().ToString();
                                                 });
        }

        public ReactiveCommand ShowDialogCommand
        {
            get
            {
                return _showDialogCommand;
            }
        }

        public ReactiveCommand SayHelloCommand
        {
            get
            {
                return _sayHelloCommand;
            }
        }

        public string Hello
        {
            get
            {
                return _hello;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref _hello, value);
            }
        }

        public int Counter
        {
            get
            {
                return _counter.Value;
            }
        }

        public string Rando
        {
            get
            {
                return _rando;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref _rando, value);
            }
        }

        public string UrlPathSegment
        {
            get
            {
                return GetType().FullName;
            }
        }

        public IScreen HostScreen { get; set; }
    }
}