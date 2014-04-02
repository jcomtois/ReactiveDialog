using ReactiveUI;

namespace WpfApplication3
{
    public static class DialogServiceExtensions
    {
        public static Answer ShowInformation(this IDialogService service, string message)
        {
            var viewModel = new DialogViewModel(message, new[] {Answer.Ok}) {
                                                                                Icon = StockUserErrorIcon.Notice,
                                                                                Caption = "Information"
                                                                            };
            return service.ShowDialogFor(viewModel);
        }
    }
}