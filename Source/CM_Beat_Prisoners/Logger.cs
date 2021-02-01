using System;
using System.Text;

using Verse;

namespace CM_Beat_Prisoners
{
    public static class Logger
    {
        public static bool MessageEnabled = false;
        public static bool WarningEnabled = true;
        public static bool ErrorEnabled = true;

        public static bool MessageInProgress = false;

        public static StringBuilder messageBuilder = new StringBuilder();

        public static void MessageNoCaller(string message)
        {
            if (MessageEnabled)
            {
                message = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name + " - " + message;
                Log.Message(message);
            }
        }

        public static void MessageFormat(object caller, string message, params object[] stuff)
        {
            if (MessageEnabled)
            {
                message = caller.GetType().ToString() + "." + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name + " - " + message;
                Log.Message(String.Format(message, stuff));
            }
        }

        public static void MessageFormat(string message, params object[] stuff)
        {
            if (MessageEnabled)
            {
                message = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name + " - " + message;
                Log.Message(String.Format(message, stuff));
            }
        }

        public static void WarningFormat(object caller, string message, params object[] stuff)
        {
            if (Logger.WarningEnabled)
            {
                message = caller.GetType().ToString() + "." + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name + " - " + message;
                Log.Warning(String.Format(message, stuff));
            }
        }

        public static void ErrorFormat(object caller, string message, params object[] stuff)
        {
            if (Logger.ErrorEnabled)
            {
                message = caller.GetType().ToString() + "." + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name + " - " + message;
                Log.Error(String.Format(message, stuff));
            }
        }

        // Building and displaying message assumes caller will be checking for MessageEnabled, WarningEnabled or ErrorEnabled
        public static void StartMessage(object caller, string message, params object[] stuff)
        {
            if (MessageEnabled)
            {
                MessageInProgress = true;
                messageBuilder = new StringBuilder(caller.GetType().ToString() + "." + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name + ": ");
                if (!string.IsNullOrEmpty(message))
                    AddToMessage(message, stuff);
            }
        }

        public static void AddToMessage(string message, params object[] stuff)
        {
            if (MessageInProgress)
                messageBuilder.AppendLine(string.Format(message, stuff));
        }

        public static void DisplayMessage()
        {
            if (MessageInProgress)
            {
                MessageInProgress = false;
                Log.Message(messageBuilder.ToString());
            }
        }

        public static void DisplayWarning()
        {
            if (MessageInProgress)
            {
                MessageInProgress = false;
                Log.Warning(messageBuilder.ToString());
            }
        }

        public static void DisplayError()
        {
            if (MessageInProgress)
            {
                MessageInProgress = false;
                Log.Error(messageBuilder.ToString());
            }
        }
    }
}
