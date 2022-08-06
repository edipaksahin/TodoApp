using System;

namespace TodoApp.Application.Logging
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IgnoreRequestLoggingAttribute : Attribute { }
}
