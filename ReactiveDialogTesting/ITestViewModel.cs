using System.Reactive;
using ReactiveUI;

namespace WpfApplication3
{
    public interface ITestViewModel : IRoutableViewModel
    {
        IReactiveCommand<object> ShowDialogCommand { get; }
        IReactiveCommand<object> SayHelloCommand { get; }
        string Hello { get; }
        int Counter { get; }
        string Rando { get; }
    }
}