using System;

namespace TodoApp.Application.Extensions
{
    public static class TypeExtensions
    {
        public static string GetRequestName(this Type type)
            => type.FullName[(type.FullName.LastIndexOf(".") + 1)..].Replace("+", ".");
    }
}
