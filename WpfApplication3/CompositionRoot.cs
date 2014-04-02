using System.Linq;
using System.Windows;
using ReactiveUI;
using SimpleInjector;

namespace WpfApplication3
{
    public interface IDialogService
    {
        void ShowADialog(string message);
    }

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
            container.RegisterSingle(() => new DialogShower(container.GetInstance<MainWindow>()));
            container.RegisterSingle<IDialogService, DialogService>();
            container.RegisterSingle<ITestViewModelFactory, TestViewModelFactory>();
            container.Register<IRoutingState, RoutingState>();
            container.RegisterSingle<AppBootstrapper>();
            container.RegisterSingle(() =>
                               {
                                   var w = new MainWindow {
                                                              DataContext = container.GetInstance<AppBootstrapper>()
                                                          };
                                   return w;
                               });

            container.Verify();
        }

        private class DialogService : IDialogService
        {
            private readonly Container _container;

            public DialogService(Container container)
            {
                _container = container;
            }

            public void ShowADialog(string message)
            {
                _container.GetInstance<DialogShower>().ShowADialog(message);
            }
        }

        public Window GetMainWindow()
        {
            return _container.GetInstance<MainWindow>();
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