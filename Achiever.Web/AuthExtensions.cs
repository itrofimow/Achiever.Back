using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Achiever.Core;
using Achiever.Core.Models.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Achiever.Web
{
    public static class AuthExtensions
    {
        public static IServiceCollection ConfigureAuth(this IServiceCollection services, string secretKey)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetService<IUserService>();
                            var userId = context.Principal.FindFirst(CustomClaimTypes.Id)?.Value;
                            var user = await userService.GetById(userId);
                            
                            if (user == null)
                            {
                                context.Fail("Unauthorized");
                            }
                            else
                            {
                                ((CurrentUser)context.HttpContext.RequestServices.GetService<ICurrentUser>()).User = user;
                                var subject = new ClaimsIdentity(new Claim[]
                                {
                                    new Claim(CustomClaimTypes.Id, user.Id),
                                    new Claim(CustomClaimTypes.Nickname, user.Nickname),
                                });
                                
                                context.Principal.AddIdentity(subject);
                            }
                        }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            
            return services;
        }
    }
}