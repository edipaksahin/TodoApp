using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Dto.User;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Extensions;
using TodoApp.Application.Interface;
using TodoApp.Application.Settings;
using TodoApp.Application.Wappers;

namespace TodoApp.Application.Features.Commands.User
{
    public class Login
    {
        public class Command : IRequest<ServiceResponse<LoginResponseModel>>
        {
            public LoginRequestModel Request { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
                => RuleFor(i => i.Request)
                   .NotNull()
                   .SetValidator(new LoginRequestModel.Validator());
        }

        public class Handler : IRequestHandler<Command, ServiceResponse<LoginResponseModel>>
        {
            private readonly IApplicationDbContext _applicationDbContext;
            private readonly JwtSettings _jwtSettings;
            public Handler(IApplicationDbContext applicationDbContext, JwtSettings jwtSettings)
            {
                _applicationDbContext = applicationDbContext;
                _jwtSettings = jwtSettings;
            }

            public async Task<ServiceResponse<LoginResponseModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = _applicationDbContext.Users.Where(x => x.Email == request.Request.Email).FirstOrDefault();

                if (user.IsNull())
                    throw new EntityNotFoundException("User", request.Request.Email);

                var response = new LoginResponseModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Token = GenerateToken(user)
                };

                return new ServiceResponse<LoginResponseModel>(response);
            }

            private string GenerateToken(Domain.Entities.User currentUser)
            {
                var key = Encoding.ASCII.GetBytes(_jwtSettings.JwtSecretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(nameof(currentUser.Id), currentUser.Id.ToString()),
                    new Claim(nameof(currentUser.Name), currentUser.Name),
                    }),
                    Expires = System.DateTime.Now.AddDays(_jwtSettings.JwtTokenExpireDay),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
        }
    }
}
