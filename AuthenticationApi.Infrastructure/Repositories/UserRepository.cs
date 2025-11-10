using AuthenticationApi.Application.Dtos;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Resource.Share.Lib.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    internal class UserRepository(AuthenticationDbContext dbContext, IConfiguration config) : IUser
    {
        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user is not null ? user : null!;
        }
        public async Task<GetUserDto> GetUser(int userId)
        {
            var user = await dbContext.Users.FindAsync(userId);

            return user is not null ? new GetUserDto(user.Id, user.UserName!,user.Phone!, user.Address!, user.Email!, user.Role!) : null!;
        }

        public async Task<Response> Login(LoginDto loginDto)
        {
            var getUser = await GetUserByEmail(loginDto.Email);

            if(getUser is null)
            {
                return new Response(false, $"User by {loginDto.Email} not found!");
            }

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password!);

            if(!verifyPassword)
            {
                return new Response(false, "Invalid credentials!");
            }

            string token = GenerateToken(getUser);

            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!),
            };

            if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
                claims.Add(new(ClaimTypes.Role, user.Role!));

            var token = new JwtSecurityToken(
                    issuer: config["Authentication:Issuer"],
                    audience: config["Authentication:Audience"],
                    claims: claims,
                    expires: null,
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDto appUserDto)
        {
            var getUser = await GetUserByEmail(appUserDto.Email);

            if(getUser is not null)
            {
                return new Response(false, $"User by {appUserDto.Email} already exists!");
            }

            var result = dbContext.Add(new AppUser {
               Email = appUserDto.Email,
               Address = appUserDto.Address,
               UserName = appUserDto.UserName,
               Phone = appUserDto.Phone,
               Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password),
               Role = appUserDto.Role
            });

            await dbContext.SaveChangesAsync();

            return result.Entity.Id > 0 ? new Response(true, "User registered successfully.") : new Response(false, "Error occured during creating user!");
        }
    }
}
