using System;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
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

        private static readonly IViewLocator ViewLocatorInstance = new ViewLocatorImplementation();

        private bool _canClose;

        public DialogView()
        {
            InitializeComponent();

            var closeCancelled = this.Events().Closing
                                     .Do(args => args.Cancel = !_canClose)
                                     .Where(args => args.Cancel)
                                     .Select(_ => Unit.Default);

            closeCancelled.Subscribe(_ => SystemSounds.Hand.Play());

            this.WhenAnyValue(t => t.ViewModel.Caption)
                .BindTo(this, t => t.Title);

            this.WhenAnyValue(t => t.ViewModel.Icon)
                .Select(ConvertIcon)
                .BindTo(this, t => t.Icon);

            this.WhenAnyValue(t => t.Icon)
                .BindTo(this, t => t.ImageIcon.Source);

            this.WhenAnyValue(t => t.TextScale)
                .BindTo(this, t => t.TextBoxMessage.FontSize);

            this.WhenAnyValue(t => t.ViewModel.Message)
                .BindTo(this, t => t.TextBoxMessage.Text);

            this.WhenAnyValue(t => t.ViewModel.Responses)
                .BindTo(this, t => t.ItemsControlResponses.ItemsSource);

            this.WhenAnyValue(t => t.ViewModel.Responses)
                .SelectMany(r => r.Select(c => c))
                .Merge()
                .Subscribe(_ => Close());

            this.WhenAnyValue(t => t.ViewModel.CanClose)
                .Subscribe(b =>
                           {
                               _canClose = b;
                           });

            this.WhenAnyValue(t => t.ViewModel.Message,
                              t => t.FontSize,
                              (s, z) => new {message = s, fontsize = z}
                )
                .Subscribe(a =>
                           {
                               if (a.message.Length < 100)
                               {
                                   TextScale = a.fontsize * 1.5d;
                               }
                               else
                               {
                                   ClearValue(TextScaleProperty);
                               }
                           });

            this.WhenAnyValue(t => t.ViewModel)
                .Subscribe(vm =>
                           {
                               if (vm != null)
                               {
                                   return;
                               }
                               _canClose = true;
                           });
        }

        public static IViewLocator ViewLocator
        {
            get
            {
                return ViewLocatorInstance;
            }
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

        private static BitmapSource ConvertIcon(StockUserErrorIcon stockUserErrorIcon)
        {
            Icon icon;

            switch (stockUserErrorIcon)
            {
                case StockUserErrorIcon.Critical:
                    icon = SystemIcons.Hand;
                    break;
                case StockUserErrorIcon.Error:
                    icon = SystemIcons.Error;
                    break;
                case StockUserErrorIcon.Question:
                    icon = SystemIcons.Question;
                    break;
                case StockUserErrorIcon.Warning:
                    icon = SystemIcons.Warning;
                    break;
                case StockUserErrorIcon.Notice:
                    icon = SystemIcons.Information;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                                                       Int32Rect.Empty,
                                                       BitmapSizeOptions.FromEmptyOptions());
        }

        private class ViewLocatorImplementation : IViewLocator
        {
            public IViewFor ResolveView <T>(T viewModel, string contract = null) where T : class
            {
                if (viewModel is RecoveryCommand)
                {
                    return new RecoveryCommandButtonView();
                }
                throw new InvalidOperationException("Requested Invalid View for ViewModel");
            }
        }
    }
}