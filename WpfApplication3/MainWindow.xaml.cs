using System;
using System.Windows;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Guid guid = Guid.NewGuid();

        public override string ToString()
        {
            return guid.ToString();
        }
    }
}