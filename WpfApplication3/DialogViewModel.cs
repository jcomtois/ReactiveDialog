using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace WpfApplication3
{
    public class DialogViewModel : ReactiveObject, IDialogViewModel<Answer>
    {
        private readonly string _message;
        private bool _canClose;
        private string _caption;
        private Answer _response;

        public DialogViewModel(string message, IEnumerable<Answer> possibleAnswers)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (possibleAnswers == null)
            {
                throw new ArgumentNullException("possibleAnswers");
            }

            _message = message;

            var commands = CreateCommands(possibleAnswers).ToArray();

            var observer = Observer.Create<RecoveryCommandDecorator<Answer>>(d => d.Subscribe(o =>
                                                                                              {
                                                                                                  Response = d.Response;
                                                                                                  CanClose = true;
                                                                                              }));
            commands.Subscribe(observer);
            Responses = commands;

            Icon = StockUserErrorIcon.Notice;
        }

        public IEnumerable<IRecoveryCommand> Responses { get; private set; }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public string Caption
        {
            get
            {
                return string.IsNullOrWhiteSpace(_caption) ? Icon.ToString() : _caption;
            }
            set
            {
                _caption = value;
            }
        }

        public Answer Response
        {
            get
            {
                return _response;
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
                return _canClose;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref _canClose, value);
            }
        }

        public StockUserErrorIcon Icon { get; set; }

        private static IEnumerable<RecoveryCommandDecorator<Answer>> CreateCommands(IEnumerable<Answer> possibleAnswers)
        {
            var list = new List<RecoveryCommandDecorator<Answer>>();

            var possible = possibleAnswers.Distinct().ToArray();

            if (!possible.Any())
            {
                possible = new[] {Answer.Ok};
            }

            foreach (var answer in possible)
            {
                RecoveryCommandDecorator<Answer> command;
                switch (answer)
                {
                    case Answer.Cancel:
                        command =
                            new RecoveryCommandDecorator<Answer>(new RecoveryCommand(Answer.Cancel.ToString(),
                                                                                     o => RecoveryOptionResult.CancelOperation)
                                                                 {IsCancel = true}
                                );
                        break;
                    case Answer.Ok:
                        command = new RecoveryCommandDecorator<Answer>(new RecoveryCommand(Answer.Ok.ToString())
                                                                       {IsDefault = true});
                        break;
                    case Answer.Retry:
                        command = new RecoveryCommandDecorator<Answer>(new RecoveryCommand(Answer.Retry.ToString(),
                                                                                           o => RecoveryOptionResult.RetryOperation));
                        break;
                    case Answer.Abort:
                        command = new RecoveryCommandDecorator<Answer>(new RecoveryCommand(Answer.Abort.ToString(),
                                                                                           o => RecoveryOptionResult.FailOperation));
                        break;
                    case Answer.Yes:
                        command = new RecoveryCommandDecorator<Answer>(new RecoveryCommand(Answer.Yes.ToString())
                                                                       {IsDefault = true});
                        break;
                    case Answer.No:
                        command =
                            new RecoveryCommandDecorator<Answer>(new RecoveryCommand(Answer.No.ToString(),
                                                                                     o => RecoveryOptionResult.CancelOperation)
                                                                 {IsCancel = true});
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                list.Add(command);
            }

            if (list.Count == 1)
            {
                list[0].IsCancel = true;
                list[0].IsDefault = true;
            }

            return list.OrderBy(r => !r.IsDefault)
                       .ThenBy(r => r.IsCancel);
        }
    }
}