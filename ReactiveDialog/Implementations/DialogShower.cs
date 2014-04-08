using System;
using System.Windows;
using ReactiveUI;

namespace ReactiveDialog.Implementations
{
    public class DialogShower : IDialogShower
    {
        private readonly Window _parent;

        public DialogShower(Window parent)
        {
            _parent = parent;
        }

        public Answer ShowDialog(IViewFor<IDialogViewModel<Answer>> view, IDialogViewModel<Answer> viewModel)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            view.ViewModel = viewModel;

            var dialog = view as Window;
            if (dialog == null)
            {
                throw new InvalidOperationException("View must derive from System.Windows.Window.");
            }

            dialog.Owner = _parent;
            dialog.ShowDialog();

            return viewModel.Response;
        }
    }
}