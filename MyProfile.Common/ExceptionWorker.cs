using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Common
{
    public static class ExceptionWorker
    {
        public static string GetExceptionMessages(Exception e, string msgs = "")
        {
            if (e == null) return string.Empty;
            if (msgs == "") msgs = e.Message;
            if (e.InnerException != null)
                msgs += "\r\nInnerException: " + GetExceptionMessages(e.InnerException);
            return msgs;
        }
    }
}
