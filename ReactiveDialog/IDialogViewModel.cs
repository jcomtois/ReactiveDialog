using System.Collections.Generic;
using ReactiveUI;

namespace ReactiveDialog
{
    public interface IDialogViewModel <T> where T : struct
    {
        IEnumerable<IRecoveryCommand> Responses { get; }
        string Message { get; }
        string Caption { get; set; }
        T Response { get; }
        bool CanClose { get; }
        StockUserErrorIcon Icon { get; set; }
    }
}