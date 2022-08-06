using System;

namespace TodoApp.Application.Common
{
    public interface IApplicationUser
    {
        Guid UserId { get; }
        string Email { get; }
        string Name { get; }
    }
}
