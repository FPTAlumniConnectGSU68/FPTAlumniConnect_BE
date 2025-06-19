using FPTAlumniConnect.DataTier.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Utils
{
    public class JwtUtil
    {
        private JwtUtil()
        {
        }

        public static string GenerateJwtToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null.");
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new InvalidOperationException("User email is required to generate JWT token.");
            }
            if (user.RoleId == null)
            {
                throw new InvalidOperationException("User role is required to generate JWT token.");
            }
            if (user.UserId == 0) 
            {
                throw new InvalidOperationException("User ID is required to generate JWT token.");
            }
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            SymmetricSecurityKey secrectKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AlumniConnectSuperSecretKey123456"));
            var credentials = new SigningCredentials(secrectKey, SecurityAlgorithms.HmacSha256Signature);
            string issuer = "AlumniConnect";
            List<Claim> claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub,user.Email),
            new Claim("Role", user.RoleId.ToString()),
            new Claim("Id", user.UserId.ToString())
        };
            var expires = DateTime.Now.AddDays(1);
            var token = new JwtSecurityToken(issuer, null, claims, notBefore: DateTime.Now, expires, credentials);
            return jwtHandler.WriteToken(token);
        }
    }
}