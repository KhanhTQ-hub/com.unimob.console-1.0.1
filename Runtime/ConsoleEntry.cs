using UnityEngine;

namespace com.unimob.console
{
    public class ConsoleEntry
    {
        private const int MessagePreviewLength = 180;
        private const int StackTracePreviewLength = 120;
        private string _messagePreview;
        private string _stackTracePreview;

        public int Count = 1;
        public LogType LogType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public ConsoleEntry() { }

        public ConsoleEntry(ConsoleEntry other)
        {
            Message = other.Message;
            StackTrace = other.StackTrace;
            LogType = other.LogType;
            Count = other.Count;
        }

        public string MessagePreview
        {
            get
            {
                if (_messagePreview != null)
                {
                    return _messagePreview;
                }

                if (string.IsNullOrEmpty(Message))
                {
                    return "";
                }

                _messagePreview = Message.Split('\n')[0];
                _messagePreview = _messagePreview.Substring(0, Mathf.Min(_messagePreview.Length, MessagePreviewLength));
                return _messagePreview;
            }
        }

        public string StackTracePreview
        {
            get
            {
                if (_stackTracePreview != null)
                {
                    return _stackTracePreview;
                }

                if (string.IsNullOrEmpty(StackTrace))
                {
                    return "";
                }

                _stackTracePreview = StackTrace.Split('\n')[0];
                _stackTracePreview = _stackTracePreview.Substring(0, Mathf.Min(_stackTracePreview.Length, StackTracePreviewLength));
                return _stackTracePreview;
            }
        }

        public bool Matches(ConsoleEntry other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Message, other.Message) && string.Equals(StackTrace, other.StackTrace) && LogType == other.LogType;
        }
    }
}