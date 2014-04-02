using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for DialogView.xaml
    /// </summary>
    public partial class DialogView : Window, IViewFor<IDialogViewModel<Answer>>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
                                                                                                  "ViewModel",
                                                                                                  typeof (IDialogViewModel<Answer>),
                                                                                                  typeof (DialogView),
                                                                                                  new PropertyMetadata(default(IDialogViewModel<Answer>)));

        public DialogView()
        {
            InitializeComponent();

            this.WhenAnyValue(t => t.ViewModel)
                .BindTo(this, v => v.DataContext);

            this.WhenAnyValue(t => t.ViewModel)
                .Where(vm => vm != null)
                .Subscribe(vm =>
                           {
                               foreach (var c in vm.Responses)
                               {
                                   var c1 = c;
                                   c.Subscribe(o => Debug.WriteLine(c1.CommandName));
                               }
                           });

            this.OneWayBind(ViewModel, vm => vm.Responses, v => v.Buttons.ItemsSource);
        }

        public IDialogViewModel<Answer> ViewModel
        {
            get
            {
                return (IDialogViewModel<Answer>)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        object IViewFor.ViewModel
        {
            get
            {
                return ViewModel;
            }
            set
            {
                ViewModel = (IDialogViewModel<Answer>)value;
            }
        }
    }
}