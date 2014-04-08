using System;
using ReactiveUI;

namespace ReactiveDialog.Decorators
{
    public class RecoveryCommandDecorator <T> : IRecoveryCommand
        where T : struct
    {
        private readonly RecoveryCommand _inner;

        public RecoveryCommandDecorator(RecoveryCommand inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException("inner");
            }
            _inner = inner;
            _inner.CanExecuteChanged += (sender, args) => OnCanExecuteChanged();

            T result;
            Response = Enum.TryParse(CommandName, true, out result) ? result : default(T);
        }

        public T Response { get; private set; }

        public bool IsDefault
        {
            get
            {
                return _inner.IsDefault;
            }
            set
            {
                _inner.IsDefault = value;
            }
        }

        public bool IsCancel
        {
            get
            {
                return _inner.IsCancel;
            }
            set
            {
                _inner.IsCancel = value;
            }
        }

        public IObservable<Exception> ThrownExceptions
        {
            get
            {
                return _inner.ThrownExceptions;
            }
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return _inner.Subscribe(observer);
        }

        public bool CanExecute(object parameter)
        {
            return _inner.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _inner.Execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Dispose()
        {
            _inner.Dispose();
            CanExecuteChanged = null;
        }

        public IObservable<T1> RegisterAsync <T1>(Func<object, IObservable<T1>> asyncBlock)
        {
            return _inner.RegisterAsync(asyncBlock);
        }

        public IObservable<bool> CanExecuteObservable
        {
            get
            {
                return _inner.CanExecuteObservable;
            }
        }

        public IObservable<bool> IsExecuting
        {
            get
            {
                return _inner.IsExecuting;
            }
        }

        public bool AllowsConcurrentExecution
        {
            get
            {
                return _inner.AllowsConcurrentExecution;
            }
        }

        public string CommandName
        {
            get
            {
                return _inner.CommandName;
            }
        }

        public RecoveryOptionResult? RecoveryResult
        {
            get
            {
                return _inner.RecoveryResult;
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}