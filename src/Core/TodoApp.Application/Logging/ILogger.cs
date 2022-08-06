using Serilog;

namespace TodoApp.Application.Logging
{
    public interface ILogger<out TContext> : ILogger { }
}
