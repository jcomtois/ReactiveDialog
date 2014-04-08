using System.Linq;
using System.Windows;
using ReactiveDialog;
using ReactiveDialog.Implementations;
using ReactiveUI;
using SimpleInjector;

namespace WpfApplication3
{
    public class CompositionRoot
    {
        private readonly Container _container;

        public CompositionRoot()
        {
            _container = new Container();

            SetupContainer(_container);

            RxApp.MutableResolver.RegisterConstant(new ViewResolver(), typeof (IViewLocator));
            RxApp.MutableResolver.Register(() => new TestView(), typeof (IViewFor<ITestViewModel>));
        }

        private static void SetupContainer(Container container)
        {
            container.Register<IViewFor<IDialogViewModel<Answer>>, ReactiveDialog.Implementations.View.DialogView>();
            container.RegisterSingle<IDialogShower>(() => new DialogShower(container.GetInstance<MainWindow>()));
            container.RegisterSingle<IDialogService, DialogService>();
            container.RegisterSingle<ITestViewModelFactory, TestViewModelFactory>();
            container.RegisterSingle<IRoutingState, RoutingState>();
            container.RegisterSingle<AppBootstrapper>();
            container.RegisterSingle<MainWindow>();

            container.Verify();
        }

        private class DialogService : IDialogService
        {
            private readonly Container _container;
            private readonly IDialogShower _dialogShower;

            public DialogService(Container container, IDialogShower dialogShower)
            {
                _container = container;
                _dialogShower = dialogShower;
            }

            public Answer ShowDialogFor(IDialogViewModel<Answer> viewModel)
            {
                var view = _container.GetInstance<IViewFor<IDialogViewModel<Answer>>>();
                return _dialogShower.ShowDialog(view, viewModel);
            }
        }

        public Window GetMainWindow()
        {
            var mainWindow = _container.GetInstance<MainWindow>();
            mainWindow.DataContext = _container.GetInstance<AppBootstrapper>();
            return mainWindow;
        }

        private class AppBootstrapper : ReactiveObject, IScreen
        {
            public AppBootstrapper(IRoutingState routingState, 
                ITestViewModelFactory testViewModelFactory)
            {
                Router = routingState;
                var vm = testViewModelFactory.Create(this);
                Router.NavigateAndReset.Execute(vm);
            }

            public IRoutingState Router { get; private set; }
        }

        public interface ITestViewModelFactory
        {
            ITestViewModel Create(IScreen screen);
        }

        private class TestViewModelFactory : ITestViewModelFactory
        {
            private readonly Container _container;

            public TestViewModelFactory(Container container)
            {
                _container = container;
            }

            public ITestViewModel Create(IScreen screen)
            {
                var vm = _container.GetInstance<TestViewModel>();
                vm.HostScreen = screen;
                return vm;
            }
        }

        private class ViewResolver : IViewLocator
        {
            public IViewFor ResolveView <T>(T viewModel, string contract = null) where T : class
            {
                var ints = viewModel.GetType().GetInterfaces();
                var found = ints.FirstOrDefault(i => i.Name.EndsWith(viewModel.GetType().Name));
                var vf = typeof (IViewFor<>).MakeGenericType(found);
                return (IViewFor)RxApp.MutableResolver.GetService(vf);
            }
        }
    }

   
}