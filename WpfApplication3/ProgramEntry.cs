using System;
using System.Windows;

namespace WpfApplication3
{
    public static class ProgramEntry
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var app = new App();

                var root = new CompositionRoot();
                var main = root.GetMainWindow();

                app.MainWindow = main;
                app.ShutdownMode = ShutdownMode.OnMainWindowClose;

                app.Run(main);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Fatal Program Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}