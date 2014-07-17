using System.Reactive;
using ReactiveUI;

namespace WpfApplication3
{
    public interface ITestViewModel : IRoutableViewModel
    {
        IReactiveCommand<Unit> ShowDialogCommand { get; }
        IReactiveCommand<Unit> SayHelloCommand { get; }
        string Hello { get; }
        int Counter { get; }
        string Rando { get; }
    }
}