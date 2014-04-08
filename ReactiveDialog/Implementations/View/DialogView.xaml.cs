using System;
using System.ComponentModel;
using System.Media;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace ReactiveDialog.Implementations.View
{
    /// <summary>
    /// Interaction logic for DialogView.xaml
    /// </summary>
    public partial class DialogView : IViewFor<IDialogViewModel<Answer>>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof (IDialogViewModel<Answer>),
                                        typeof (DialogView),
                                        new PropertyMetadata(default(IDialogViewModel<Answer>)));

        public static readonly DependencyProperty TextScaleProperty = DependencyProperty.Register(
                                                                                                  "TextScale",
                                                                                                  typeof (double),
                                                                                                  typeof (DialogView),
                                                                                                  new PropertyMetadata(
                                                                                                      FontSizeProperty.DefaultMetadata.DefaultValue));

        private bool _canClose;
        private ReactiveCommand _playErrorSound;

        public DialogView()
        {
            this.WhenAnyValue(t => t.ViewModel)
                .Subscribe(vm =>
                           {
                               _playErrorSound = new ReactiveCommand();

                               if (vm == null)
                               {
                                   _canClose = true;
                                   return;
                               }

                               _playErrorSound.OfType<bool>().Where(b => b).Subscribe(b => SystemSounds.Hand.Play());

                               var observer = Observer.Create<IRecoveryCommand>(command => command.Subscribe(o => HandleCommand(command)));
                               vm.Responses.Subscribe(observer);

                               var canCloseChanged = vm.WhenAnyValue(v => v.CanClose);
                               canCloseChanged.Subscribe(b =>
                                                         {
                                                             _canClose = b;
                                                         });

                               if (vm.Message.Length < 100)
                               {
                                   TextScale = FontSize * 1.5d;
                               }
                               else
                               {
                                   ClearValue(TextScaleProperty);
                               }
                           });

            InitializeComponent();
        }

        public double TextScale
        {
            get
            {
                return (double)GetValue(TextScaleProperty);
            }
            set
            {
                SetValue(TextScaleProperty, value);
            }
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

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !_canClose;
            _playErrorSound.Execute(!_canClose);
            base.OnClosing(e);
        }

        private void HandleCommand(IRecoveryCommand command)
        {
            Close();
        }
    }
}