using ReactiveUI;

namespace ReactiveDialog
{
    public interface IDialogShower : IDialogShower<Answer>
    {
    }

    public interface IDialogShower <T> where T : struct
    {
        T ShowDialog(IViewFor<IDialogViewModel<T>> view, IDialogViewModel<T> viewModel);
    }
}