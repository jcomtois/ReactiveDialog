using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for TestView.xaml
    /// </summary>
    public partial class TestView : UserControl, IViewFor<ITestViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof (ITestViewModel),
                                        typeof (TestView),
                                        new PropertyMetadata(default(ITestViewModel)));

        public TestView()
        {
            InitializeComponent();

            this.WhenAnyValue(t => t.ViewModel)
                .BindTo(this, v => v.DataContext);

            //this.WhenAny(t => t.ViewModel, t => t.Value)
            //    .Subscribe(x => DataContext = ViewModel);
        }

        public ITestViewModel ViewModel
        {
            get
            {
                return (ITestViewModel)GetValue(ViewModelProperty);
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
                ViewModel = (ITestViewModel)value;
            }
        }
    }
}