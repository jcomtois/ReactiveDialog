using System;
using ReactiveUI;

namespace ReactiveDialog.Decorators
{
    public class RecoveryCommandWithResponse <T> : RecoveryCommand
        where T : struct
    {
        public RecoveryCommandWithResponse(string commandName, Func<object, RecoveryOptionResult> handler = null) : base(commandName, handler)
        {
            T result;
            Response = Enum.TryParse(CommandName, true, out result) ? result : default(T);
        }

        public T Response { get; private set; }
    }
}