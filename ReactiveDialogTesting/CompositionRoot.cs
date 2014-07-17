using System.Linq;
using System.Windows;
using ReactiveDialog;
using ReactiveDialog.Implementations;
using ReactiveDialog.Implementations.View;
using ReactiveUI;
using SimpleInjector;
using Splat;

namespace WpfApplication3
{
    public class CompositionRoot
    {
        private readonly Container _container;

        public CompositionRoot()
        {
            _container = new Container();

            SetupContainer(_container);

            Locator.CurrentMutable.RegisterConstant(new ViewResolver(), typeof (IViewLocator));
            Locator.CurrentMutable.Register(() => new TestView(), typeof (IViewFor<ITestViewModel>));
        }

        private static void SetupContainer(Container container)
        {
            container.Register<IViewFor<IDialogViewModel<Answer>>, DialogView>();
            container.RegisterSingle<IDialogShower>(() => new DialogShower(container.GetInstance<MainWindow>()));
            container.RegisterSingle<IDialogService, DialogService>();
            container.RegisterSingle<ITestViewModelFactory, TestViewModelFactory>();
            container.RegisterSingle<RoutingState>();
            container.RegisterSingle<AppBootstrapper>();
            container.RegisterSingle<MainWindow>();

            container.Verify();
        }

        public Window GetMainWindow()
        {
            var mainWindow = _container.GetInstance<MainWindow>();
            mainWindow.DataContext = _container.GetInstance<AppBootstrapper>();
            return mainWindow;
        }

        private class AppBootstrapper : ReactiveObject, IScreen
        {
            public AppBootstrapper(RoutingState routingState,
                                   ITestViewModelFactory testViewModelFactory)
            {
                Router = routingState;
                var vm = testViewModelFactory.Create(this);
                Router.NavigateAndReset.Execute(vm);
            }

            public RoutingState Router { get; private set; }
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
                return (IViewFor)Locator.CurrentMutable.GetService(vf);
            }
        }
    }
}