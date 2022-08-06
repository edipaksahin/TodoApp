using System;

namespace TodoApp.Application.Dto.User
{
    public class LoginResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
