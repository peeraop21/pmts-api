using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using System;


namespace PMTs.WebAPI.JwtHelpers
{
    public static class JwtExtensions
    {
        public static void GenerateToken(this UserDTO user, IConfiguration configuration)
        {
            try
            {
                var token = new JwtTokenBuilder()
                                .AddSecurityKey(JwtSecurityKey.Create(configuration.GetValue<string>("JwtSecretKey")))
                                .AddIssuer(configuration.GetValue<string>("JwtIssuer"))
                                .AddAudience(configuration.GetValue<string>("JwtAudience"))
                                .AddExpiry(30)
                                .AddClaim("Id", user.Id.ToString())
                                .AddRole("Other")
                                .Build();

                user.Token = token.Value;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
