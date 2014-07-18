using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace ReactiveDialog.Implementations
{
    public class DialogViewModel : ReactiveObject, IDialogViewModel<Answer>
    {
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

            Message = message;

            var commands = CreateCommands(possibleAnswers).ToArray();

            foreach (var c in commands)
            {
                var response = c.Response;
                c.Subscribe(_ =>
                            {
                                Response = response;
                                CanClose = true;
                            });
            }

            Responses = commands;

            Icon = StockUserErrorIcon.Notice;
        }

        public IEnumerable<RecoveryCommand> Responses { get; private set; }

        public string Message { get; private set; }

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

        private static IEnumerable<RecoveryCommandWithResponse<Answer>> CreateCommands(IEnumerable<Answer> possibleAnswers)
        {
            var list = new List<RecoveryCommandWithResponse<Answer>>();

            var possible = possibleAnswers.Distinct().ToArray();

            if (!possible.Any())
            {
                possible = new[] {Answer.Ok};
            }

            foreach (var answer in possible)
            {
                RecoveryCommandWithResponse<Answer> command;
                switch (answer)
                {
                    case Answer.Cancel:
                        command =
                            new RecoveryCommandWithResponse<Answer>(Answer.Cancel.ToString(),
                                                                    o => RecoveryOptionResult.CancelOperation)
                            {IsCancel = true};
                        break;
                    case Answer.Ok:
                        command = new RecoveryCommandWithResponse<Answer>(Answer.Ok.ToString())
                                  {IsDefault = true};
                        break;
                    case Answer.Retry:
                        command = new RecoveryCommandWithResponse<Answer>(Answer.Retry.ToString(),
                                                                          o => RecoveryOptionResult.RetryOperation);
                        break;
                    case Answer.Abort:
                        command = new RecoveryCommandWithResponse<Answer>(Answer.Abort.ToString(),
                                                                          o => RecoveryOptionResult.FailOperation);
                        break;
                    case Answer.Yes:
                        command = new RecoveryCommandWithResponse<Answer>(Answer.Yes.ToString())
                                  {IsDefault = true};
                        break;
                    case Answer.No:
                        command =
                            new RecoveryCommandWithResponse<Answer>(Answer.No.ToString(),
                                                                    o => RecoveryOptionResult.CancelOperation)
                            {IsCancel = true};
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