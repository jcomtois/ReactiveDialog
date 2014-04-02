using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using ReactiveUI;

namespace WpfApplication3
{
    public class DialogViewModel : ReactiveObject, IDialogViewModel<Answer>
    {
        private readonly ObservableAsPropertyHelper<bool> _canClose;
        private readonly string _message;
        private Answer? _response;

        public DialogViewModel(string message)
        {
            _message = message;
            var oKResponse = new RecoveryCommandDecorator<Answer>(new RecoveryCommand(Answer.Ok.ToString()) {IsDefault = true});
            var cancelResponse = new RecoveryCommandDecorator<Answer>(new RecoveryCommand(Answer.Cancel.ToString(),
                                                                                          o => RecoveryOptionResult.CancelOperation) {IsCancel = true});
            var commands = new[] {
                                     oKResponse,
                                     cancelResponse
                                 };

            foreach (var d in commands)
            {
                var d1 = d;
                d.Subscribe(c => Response = d1.Response);
            }

            Responses = commands;
            var responseSet = this.WhenAnyValue(x => x.Response).Skip(1).Select(x => true);
            _canClose = responseSet.ToProperty(this, x => x.CanClose);
        }

        public IEnumerable<IRecoveryCommand> Responses { get; private set; }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public Answer Response
        {
            get
            {
                return _response ?? default(Answer);
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref _response, value);
            }
        }

        public bool CanClose
        {
            get
            {
                return _canClose.Value;
            }
        }
    }
}