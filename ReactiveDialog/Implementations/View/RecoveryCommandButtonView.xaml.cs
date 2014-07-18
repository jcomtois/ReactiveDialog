using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace ReactiveDialog.Implementations.View
{
    /// <summary>
    /// Interaction logic for RecoveryCommandButtonView.xaml
    /// </summary>
    public partial class RecoveryCommandButtonView : UserControl, IViewFor<RecoveryCommand>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
                                                                                                  "ViewModel",
                                                                                                  typeof (RecoveryCommand),
                                                                                                  typeof (RecoveryCommandButtonView),
                                                                                                  new PropertyMetadata(default(RecoveryCommand)));

        public RecoveryCommandButtonView()
        {
            InitializeComponent();

            this.WhenAnyValue(t => t.ViewModel)
                .BindTo(this, t => t.ButtonCommand.Command);

            this.WhenAnyValue(t => t.ViewModel.CommandName)
                .BindTo(this, t => t.ButtonCommand.Content);

            this.WhenAnyValue(t => t.ViewModel.IsCancel)
                .BindTo(this, t => t.ButtonCommand.IsCancel);

            this.WhenAnyValue(t => t.ViewModel.IsDefault)
                .BindTo(this, t => t.ButtonCommand.IsDefault);
        }

        public RecoveryCommand ViewModel
        {
            get
            {
                return (RecoveryCommand)GetValue(ViewModelProperty);
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
                ViewModel = (RecoveryCommand)value;
            }
        }
    }
}