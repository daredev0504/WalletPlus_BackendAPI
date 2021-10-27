using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.AuthManager
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private AppUser _user;
        private readonly IConfigurationSection _goolgeSettings;
     

        public AuthenticationManager(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _goolgeSettings = _configuration.GetSection("web");
        }

        public async Task<bool> ValidateUser(AppUserLoginDto model)
        {
            _user = await _userManager.FindByEmailAsync(model.Email);
            return (_user != null &&
                    await _userManager.CheckPasswordAsync(_user,
                        model.Password));
        }

        public async Task<bool> ConfirmUserEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
               
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return true;
            }

            return false;
        }



        public async Task<IList<string>> GetRoles(AppUserLoginDto model)
        {
            _user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(_user);
            return roles;
        }

        public async Task<JwtAuthResult> CreateToken(AppUser appUser)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(appUser);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var accesToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new JwtAuthResult()
            {
                AccessToken = accesToken
            };
        }


        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret,
                SecurityAlgorithms.HmacSha256);
        }


        private async Task<List<Claim>> GetClaims(AppUser appUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,
                    appUser.Email),
                new Claim(ClaimTypes.Name,
                    appUser.FirstName),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id)
            };
            var roles = await _userManager.GetRolesAsync(appUser);
            foreach (var role in  roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,
                    role));
            }

            return claims;
        }



        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken(issuer: jwtSettings.GetSection("validIssuer")
                    .Value,
                audience: jwtSettings.GetSection("validAudience")
                    .Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("expires")
                    .Value)),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }
    }
}
