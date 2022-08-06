namespace TodoApp.Application.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object obj) => obj is null;
        public static bool IsNotNull(this object obj) => !obj.IsNull();
    }
}
