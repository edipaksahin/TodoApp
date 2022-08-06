using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using TodoApp.Application.Common;
using TodoApp.Application.Logging;
using TodoApp.Application.Pipelines;

namespace TodoApp.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationRegistration(this IServiceCollection services, string jwtKey)
        {
            var assm = Assembly.GetExecutingAssembly();

            services.AddScoped(typeof(IApplicationUser), typeof(ApplicationUser));
            services.AddAutoMapper(assm);
            services.AddMediatR(assm);
            services.AddSingleton(typeof(ILogger<>), typeof(SeriLogger<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggerBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            var key = Encoding.ASCII.GetBytes(jwtKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
