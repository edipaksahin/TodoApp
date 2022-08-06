using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using TodoApp.Application.Dto.User;
using TodoApp.Application.Extensions;

namespace TodoApp.Application.Common
{
    public class ApplicationUser : IApplicationUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId => this.GetUserId();
        public string Email => this.GetUserEmail();
        public string Name => this.GetUserFullName();
        private Guid GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext
                                .User.Claims
                                .FirstOrDefault(claim => claim.Type == nameof(LoginResponseModel.Id));

            return userId.IsNotNull() ? Guid.Parse(userId.Value) : Guid.Empty;
        }

        private string GetUserEmail()
        {
            var userName = _httpContextAccessor.HttpContext
                                .User.Claims
                                .FirstOrDefault(claim => claim.Type == nameof(LoginResponseModel.Email));

            return userName.IsNotNull() ? userName.Value : "";
        }

        private string GetUserFullName()
        {
            var userName = _httpContextAccessor.HttpContext
                                .User.Claims
                                .FirstOrDefault(claim => claim.Type == nameof(LoginResponseModel.Name));

            return userName.IsNotNull() ? userName.Value : "";
        }
    }
}
