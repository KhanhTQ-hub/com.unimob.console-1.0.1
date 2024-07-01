using System;

namespace com.unimob.console
{
    public interface IConsole
    {
        int ErrorCount { get; }

        int WarningCount { get; }

        int InfoCount { get; }

        event Action<ConsoleEntry> OnUpdated;

        event Action<ConsoleEntry> OnError;

        IReadOnlyList<ConsoleEntry> Entries { get; }
    }
}