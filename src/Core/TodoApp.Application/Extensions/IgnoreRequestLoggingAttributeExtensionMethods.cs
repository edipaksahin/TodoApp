using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TodoApp.Application.Logging;

namespace TodoApp.Application.Extensions
{
    internal static class IgnoreRequestLoggingAttributeExtensionMethods
    {
        internal static bool ShouldIgnoreRequestLogging(this Type type)
        {
            if (type.GetCustomAttributes(typeof(IgnoreRequestLoggingAttribute), true).Any())
            {
                return true;
            }

            var declaringType = type.DeclaringType;

            while (declaringType != null)
            {
                if (declaringType.GetCustomAttributes(typeof(IgnoreRequestLoggingAttribute), true).Any())
                {
                    return true;
                }

                declaringType = declaringType.DeclaringType;
            }

            return false;
        }
    }
}
