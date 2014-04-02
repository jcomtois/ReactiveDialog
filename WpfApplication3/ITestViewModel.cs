using System.Security.Cryptography.X509Certificates;
using ReactiveUI;

namespace WpfApplication3
{
    public interface ITestViewModel : IRoutableViewModel
    {
        ReactiveCommand ShowDialogCommand { get; }
        ReactiveCommand SayHelloCommand { get; }
        string Hello { get; }
        int Counter { get; }
        string Rando { get; }
    }
}