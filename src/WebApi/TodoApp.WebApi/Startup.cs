using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoApp.Application;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Interface;
using TodoApp.Persistence.Context;
using TodoApp.Persistence.Extensions;

namespace TodoApp.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(ApplicationDbContext).Assembly.GetName().Name;
            var connectionString = Configuration.GetConnectionString(nameof(ApplicationDbContext));

            services
                .AddHttpClient()
                .AddHttpContextAccessor()
                .AddDistributedMemoryCache();

            services.AddApplicationRegistration();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });
                c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
               });
            });

            services
                .AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>())
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly))
                );

            services
                .AddControllers(options =>
                    options.Filters.Add(new HttpResponseExceptionFilter())
                )
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = string.Join(',', context.ModelState.Values
                            .SelectMany(x => x.Errors.Select(s => s.ErrorMessage)));

                        return new BadRequestObjectResult(errors);
                    };
                });

            services.TryAddScoped<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.InitializeDatabase<ApplicationDbContext>(true, true, SeedDatabase, true);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private async Task SeedDatabase(IServiceProvider services, IApplicationDbContext applicationDbContext)
        {
            if (applicationDbContext.Users.Any())
            {
                return;
            }

            var user = new Domain.Entities.User
            {
                Name = "Edip Aksahin",
                Email = "edipaksahin@gmail.com"
            };

            var todo = new Domain.Entities.Task
            {
                Title = "Default Todo",
                Description = "Default",
                IsCompleted = false,
            };

            await applicationDbContext.Users.AddAsync(user);
            await applicationDbContext.Tasks.AddAsync(todo);
            await applicationDbContext.SaveChangesAsync();
        }
    }
}
