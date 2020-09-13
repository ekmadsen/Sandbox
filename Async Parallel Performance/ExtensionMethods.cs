using System;
using System.Text;


namespace ErikTheCoder.Sandbox.AsyncParallelPerformance
{
    public static class ExtensionMethods
    {
        public static string GetSummary(this Exception Exception)
        {
            var stringBuilder = new StringBuilder();
            var exception = Exception;
            while (exception != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Exception Type = {exception.GetType().FullName}.");
                stringBuilder.AppendLine($"Exception Message = {exception.Message}.");
                stringBuilder.AppendLine($"Exception StackTrace = {exception.StackTrace}.");
                exception = exception.InnerException;
            }
            return stringBuilder.ToString();
        }
    }
}
