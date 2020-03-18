using System;
using System.Text;

namespace HUF.InitFirebase
{
    public static class FirebaseUtils
    {
        public static string GetFullErrorMessage(this AggregateException exception)
        {
            var sb = new StringBuilder(exception.Message);
            if (exception.InnerExceptions.Count > 0)
            {
                foreach (var innerException in exception.InnerExceptions)
                {
                    sb.Append($"\n{innerException.Message}");
                    if (innerException.InnerException != null)
                        sb.Append($" Exc: {innerException.InnerException.Message}");
                }
            }

            return sb.ToString();
        }
    }
}