using AutoMapper;
using Library.Api.ViewModels;
using Library.Business.Interfaces;
using Library.Business.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public UsersController(IUserRepository userRepository,
            IMapper mapper,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;

            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            return _mapper.Map<IEnumerable<UserViewModel>>(await _userRepository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var oldAppUser = await _userRepository.FirstOrDefault(x => x.Id == userViewModel.Id);

                var user = await _userManager.FindByEmailAsync(oldAppUser.Email);
                var roles = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, roles);

                user.Email = userViewModel.Email;
                user.UserName = userViewModel.Email;
                user.NormalizedEmail = userViewModel.Email.ToUpper();
                user.NormalizedUserName = userViewModel.Email.ToUpper();

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                await _userManager.ResetPasswordAsync(user, token, userViewModel.Password);
                
                if (userViewModel.Administrator)
                {
                    await _userManager.AddToRoleAsync(user, "Administrator");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "CommonUser");
                }

                await _userManager.UpdateAsync(user);
                await _userRepository.Update(_mapper.Map<User>(userViewModel));
            }
            catch (DbUpdateConcurrencyException)
            {
                var userValid = await _userRepository.GetById(id);

                if (userValid == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserViewModel userViewModel)
        {
            var user = _mapper.Map<User>(userViewModel);
            await _userRepository.Add(user);
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var appUser = await _userRepository.FirstOrDefault(x => x.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(appUser.Email);
            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.DeleteAsync(user);

            await _userRepository.Remove(id);

            return appUser;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            UserViewModel userViewModel = new UserViewModel();
            userViewModel.Email = registerUser.Email;
            userViewModel.Password = registerUser.Password;
            userViewModel.Administrator = registerUser.Administrator;

            await _userRepository.Add(_mapper.Map<User>(userViewModel));

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                var newUserIdentity = await _userManager.FindByEmailAsync(userViewModel.Email);

                if (userViewModel.Administrator)
                {
                    await _userManager.AddToRoleAsync(newUserIdentity, "Administrator");
                }
                else
                {
                    await _userManager.AddToRoleAsync(newUserIdentity, "CommonUser");
                }

                await _signInManager.SignInAsync(user, false);
                return Ok(await BuildToken(user.Email));
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                return Ok(await BuildToken(loginUser.Email));
            }

            return BadRequest();
        }

        private async Task<UserTokenViewModel>  BuildToken(string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // tempo de expiração do token: 30 min
            // parte de token ainda não implementado fica necessário a adição de parte da lógica
            var expiration = TimeZoneInfo.ConvertTime
                (DateTime.Now,
                TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")
                ).AddMinutes(30);//  30 MINUTOS

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            //--------------------------------------------

            return
                new UserTokenViewModel()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expiration,
                    Id = GetCurrentUserId(email),
                    Email = email
                }
            ;
        }

        private int GetCurrentUserId(string email)
        {
            return _userRepository.GetWhere(u => u.Email == email).Result.First().Id;
        }

        private async Task<string> GetCurrentUserEmail()
        {
            IdentityUser applicationUser = await _userManager.GetUserAsync(User);
            return applicationUser?.Email;
        }

        private int FindUserId()
        {
            return GetCurrentUserId(GetCurrentUserEmail().Result);
        }

    }
}
