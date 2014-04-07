using System;
using System.Collections.Generic;
using ReactiveUI;

namespace WpfApplication3
{
    public static class DialogServiceExtensions
    {
        private static IDialogViewModel<Answer> CreateViewModel(
            string message,
            string caption = null,
            IEnumerable<Answer> possibleAnswers = null,
            StockUserErrorIcon icon = StockUserErrorIcon.Notice
            )

        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Please provide a message", "message");
            }

            var newCaption = caption ?? icon.ToString();
            var answers = possibleAnswers ?? new[] {Answer.Ok};

            return new DialogViewModel(message, answers) {
                                                             Icon = icon,
                                                             Caption = newCaption
                                                         };
        }

        public static Answer ShowInformation(this IDialogService service,
                                             string message)
        {
            return ShowInformation(service, message, new[] {Answer.Ok});
        }

        public static Answer ShowInformation(this IDialogService service,
                                             string message,
                                             IEnumerable<Answer> possibleAnswers)
        {
            return ShowInformation(service, message, "Information", possibleAnswers);
        }

        public static Answer ShowInformation(this IDialogService service,
                                             string message,
                                             string caption)
        {
            return ShowInformation(service, message, caption, new[] {Answer.Ok});
        }

        public static Answer ShowInformation(this IDialogService service,
                                             string message,
                                             string caption,
                                             IEnumerable<Answer> possibleAnswers)
        {
            return PerformDialogDisplay(service, message, caption, possibleAnswers, StockUserErrorIcon.Notice);
        }

        private static Answer PerformDialogDisplay(IDialogService<Answer> service,
                                                   string message,
                                                   string caption,
                                                   IEnumerable<Answer> possibleAnswers,
                                                   StockUserErrorIcon icon)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            var viewModel = CreateViewModel(message, caption, possibleAnswers, icon);
            return service.ShowDialogFor(viewModel);
        }

        public static Answer ShowWarning(this IDialogService service,
                                         string message,
                                         string caption,
                                         IEnumerable<Answer> possibleAnswers)
        {
            return PerformDialogDisplay(service, message, caption, possibleAnswers, StockUserErrorIcon.Warning);
        }

        public static Answer ShowWarning(this IDialogService service,
                                            string message)
        {
            return ShowWarning(service, message, new[] { Answer.Ok });
        }

        public static Answer ShowWarning(this IDialogService service,
                                             string message,
                                             IEnumerable<Answer> possibleAnswers)
        {
            return ShowWarning(service, message, "Warning", possibleAnswers);
        }

        public static Answer ShowWarning(this IDialogService service,
                                             string message,
                                             string caption)
        {
            return ShowWarning(service, message, caption, new[] { Answer.Ok });
        }

        public static Answer ShowError(this IDialogService service,
                                         string message,
                                         string caption,
                                         IEnumerable<Answer> possibleAnswers)
        {
            return PerformDialogDisplay(service, message, caption, possibleAnswers, StockUserErrorIcon.Error);
        }

        public static Answer ShowError(this IDialogService service,
                                            string message)
        {
            return ShowError(service, message, new[] { Answer.Ok });
        }

        public static Answer ShowError(this IDialogService service,
                                             string message,
                                             IEnumerable<Answer> possibleAnswers)
        {
            return ShowWarning(service, message, "Error", possibleAnswers);
        }

        public static Answer ShowError(this IDialogService service,
                                             string message,
                                             string caption)
        {
            return ShowError(service, message, caption, new[] { Answer.Ok });
        }

        public static Answer ShowQuestion(this IDialogService service,
                                         string message,
                                         string caption,
                                         IEnumerable<Answer> possibleAnswers)
        {
            return PerformDialogDisplay(service, message, caption, possibleAnswers, StockUserErrorIcon.Question);
        }

        public static Answer ShowQuestion(this IDialogService service,
                                            string message)
        {
            return ShowQuestion(service, message, new[] { Answer.Yes, Answer.No });
        }

        public static Answer ShowQuestion(this IDialogService service,
                                             string message,
                                             IEnumerable<Answer> possibleAnswers)
        {
            return ShowQuestion(service, message, "Question", possibleAnswers);
        }

        public static Answer ShowQuestion(this IDialogService service,
                                             string message,
                                             string caption)
        {
            return ShowQuestion(service, message, caption, new[] { Answer.Yes, Answer.No });
        }

        public static Answer ShowException(this IDialogService service, Exception exception, string message = null)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var caption = string.Format("{0} encountered!", exception.GetType().Name);
            var display = message ?? string.Empty;
            display += string.Format("{0}{0}{1}{0}{2}{0}{1}{0}{3}", Environment.NewLine, new string('-', 25), exception.Message, exception.StackTrace);
            return ShowError(service, display, caption);
        }
    }
}