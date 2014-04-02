using System;
using System.Windows;
using ReactiveUI;

namespace WpfApplication3
{
    public class DialogShower
    {
        private readonly Window _parent;

        public DialogShower(Window parent)
        {
            _parent = parent;
        }

        public void ShowADialog(string message)
        {
            var viewModel = new DialogViewModel(message);
            var dialog = new DialogView() {ViewModel = viewModel};
            dialog.Owner = _parent;
            dialog.ShowDialog();
        }
    }
}