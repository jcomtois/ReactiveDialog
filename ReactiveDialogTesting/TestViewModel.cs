using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ReactiveDialog;
using ReactiveUI;

namespace WpfApplication3
{
    public class TestViewModel : ReactiveObject, ITestViewModel
    {
        private static readonly Random _random = new Random();
        private readonly ObservableAsPropertyHelper<int> _counter;

        private readonly IDialogService _dialogService;
        private readonly IReactiveCommand<object> _sayHelloCommand;
        private readonly IReactiveCommand<object> _showDialogCommand;
        private string _hello;
        private string _rando;

        [DebuggerStepThrough]
        public TestViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            var changeTime = ReactiveCommand.CreateAsyncObservable(_ => Observable.Return(DateTime.UtcNow.ToLocalTime().ToString()));
            changeTime.Subscribe(s => Hello = s);

            var asyncHelloCommand = ReactiveCommand.CreateAsyncTask(_ =>
                                                                        Task.Run(() =>
                                                                                 {
                                                                                     Thread.Sleep(2000);
                                                                                     Debug.WriteLine(Thread.CurrentThread.IsThreadPoolThread);
                                                                                     return _random.NextDouble().ToString();
                                                                                 }
                                                                        ));

            asyncHelloCommand.Subscribe(s => Rando = s);

            _sayHelloCommand = ReactiveCommand.CreateCombined(new IReactiveCommand[]{changeTime, asyncHelloCommand});

            _showDialogCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.Hello, s => !string.IsNullOrWhiteSpace(s)));

            var dialog2Command = ReactiveCommand.CreateAsyncObservable(o =>
                                                                       {
                                                                           if (o is Answer)
                                                                           {
                                                                               return Observable.Return((Answer)o);
                                                                           }
                                                                           return Observable.Empty<Answer>();
                                                                       });

            dialog2Command
                .Where(a => a == Answer.Ok)
                .Subscribe(o =>
                           {
                               throw new Exception("AAAAAAAAA");
                           });

            //            _showDialogCommand.Subscribe(x =>
            //                                         {
            //                                             Hello = null;
            //                                         });

            var answer = new Subject<Answer>();
            answer.Subscribe(o => _dialogService.ShowInformation("Hello"));

            var b = true;

            _showDialogCommand.Subscribe(_ =>
                                         {
                                             var asyncCommand = ReactiveCommand.CreateAsyncObservable(o =>
                                                                                                      {
                                                                                                          Hello = null;
                                                                                                          b = !b;
                                                                                                          if (b)
                                                                                                          {
                                                                                                              throw (new InvalidOperationException("OOOOOOOO"));
                                                                                                          }
                                                                                                          answer.OnNext(
                                                                                                                        _dialogService.ShowInformation(
                                                                                                                                                       "Do it to it"));
                                                                                                          return Observable.Return(Unit.Default);
                                                                                                      });
                                             asyncCommand.ExecuteAsync().Subscribe();
                                         });

            _showDialogCommand.ThrownExceptions
                              .Subscribe(ex => _dialogService.ShowException(ex, "message"));

            _counter = new ObservableAsPropertyHelper<int>(Observable.Generate(0, i => true, i => i + 1, i => i, i => TimeSpan.FromSeconds(.01)),
                                                           i => this.RaisePropertyChanged("Counter"));
        }

        public IReactiveCommand<object> ShowDialogCommand
        {
            get
            {
                return _showDialogCommand;
            }
        }

        public IReactiveCommand<object> SayHelloCommand
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