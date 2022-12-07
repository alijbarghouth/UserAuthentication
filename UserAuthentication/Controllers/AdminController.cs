using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAuthentication.Dto;
using UserAuthentication.Hashing;
using UserAuthentication.Models;
using UserAuthentication.Service.AdminService;
using UserAuthentication.Token;

namespace UserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAdminService _service;
        private readonly IHashPassword _hash;
        private readonly IConfiguration _configuration;

        public AdminController(IMapper mapper, IAdminService service, IHashPassword hash, IConfiguration configuration)
        {
            _mapper = mapper;
            _service = service;
            _hash = hash;
            _configuration = configuration;
        }

        [HttpGet("getdata"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            var admin = await _service.getAllAdmin();
            return Ok(admin);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAdmin(AdminDto dto)
        {
            _hash.CraeteHashPassword(dto.Password, out byte[] passwordHash, out byte[] passwordSlot);

            var admin = _mapper.Map<Admin>(dto);
            admin.PasswordHash = passwordHash;
            admin.PasswordSlot = passwordSlot;

            await _service.RegisterAdmin(admin);

            return Ok(admin);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AdminDto dto)
        {


            var admin = await _service.getName(dto.Name);

            if (admin is null)
            {
                return BadRequest("The name is not exist");
            }
            if (!_hash.varifyPassword(dto.Password, admin.PasswordHash, admin.PasswordSlot))
            {
                return BadRequest("The password is wrong");
            }
            string token;
            if (admin.Name == "alibarghouth")
            {
                token = CreateTokenAlibarghouth(admin);

                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken, admin);
            }
            else
            {
                token = CreateToken(admin);
                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken, admin);
            }
            return Ok(token);

        }
        [HttpPost("refresh-token"), Authorize(Roles = "alibarghouth")]
        public async Task<IActionResult> RefreshToken(AdminDto dto)
        {
            var admin = await _service.getName(dto.Name);

            var refreshToken = Request.Cookies["refreshToken"];

            if (!admin.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (admin.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(admin);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken, admin);

            return Ok(token);
        }
        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, Admin admin)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            admin.RefreshToken = newRefreshToken.Token;
            admin.TokenCreated = newRefreshToken.Created;
            admin.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(Admin admin)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Name),
                new Claim(ClaimTypes.Role,"Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        private string CreateTokenAlibarghouth(Admin admin)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Name),
                new Claim(ClaimTypes.Role,"alibarghouth")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }



    }
}
