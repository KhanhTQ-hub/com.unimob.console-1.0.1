using System;
using UnityEngine;

namespace com.unimob.console
{
    public class StandardConsole : IConsole, IDisposable
    {
        private const int MaximumConsoleEntries = 1200;
        private readonly CircularBuffer<ConsoleEntry> _consoleEntries;
        private readonly object _threadLock = new object();

        public event Action<ConsoleEntry> OnUpdated;
        public event Action<ConsoleEntry> OnError;

        public int ErrorCount { get; private set; }

        public int WarningCount { get; private set; }

        public int InfoCount { get; private set; }

        public IReadOnlyList<ConsoleEntry> Entries
        {
            get
            {
                if (_consoleEntries == null) return null;
                lock (_threadLock)
                {
                    return _consoleEntries;
                }
            }
        }

        public StandardConsole()
        {
            _consoleEntries = new CircularBuffer<ConsoleEntry>(MaximumConsoleEntries);
            Application.logMessageReceivedThreaded += UnityLogCallback;
        }

        public void Dispose()
        {
            _consoleEntries.Clear();
            Application.logMessageReceivedThreaded -= UnityLogCallback;
        }

        private void UnityLogCallback(string condition, string stackTrace, LogType type)
        {
            lock (_threadLock)
            {
                var prevMessage = _consoleEntries.Count > 0
                    ? _consoleEntries[_consoleEntries.Count - 1]
                    : null;

                Counter(type, 1);

                if (prevMessage != null && prevMessage.LogType == type && prevMessage.Message == condition && prevMessage.StackTrace == stackTrace)
                {
                    EntryDuplicated(prevMessage);
                }
                else
                {
                    var newEntry = new ConsoleEntry
                    {
                        LogType = type,
                        StackTrace = stackTrace,
                        Message = condition
                    };

                    EntryAdded(newEntry);
                }
            }
        }

        private void EntryAdded(ConsoleEntry entry)
        {
            if (_consoleEntries.IsFull)
            {
                Counter(_consoleEntries.Front().LogType, -1);
                _consoleEntries.PopFront();
            }

            _consoleEntries.PushBack(entry);
            OnUpdatedInvoke(entry);
        }

        private void EntryDuplicated(ConsoleEntry entry)
        {
            entry.Count++;
            OnUpdatedInvoke(entry);
        }

        private void Counter(LogType type, int amount)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    ErrorCount += amount;
                    break;
                case LogType.Warning:
                    WarningCount += amount;
                    break;
                case LogType.Log:
                    InfoCount += amount;
                    break;
            }
        }

        private void OnUpdatedInvoke(ConsoleEntry entry)
        {
            OnUpdated?.Invoke(entry);
            if (entry.LogType == LogType.Assert || entry.LogType == LogType.Error || entry.LogType == LogType.Exception)
            {
                OnError?.Invoke(entry);
            }
        }
    }
}