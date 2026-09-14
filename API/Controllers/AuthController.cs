using API.Dtos;
using AutoMapper;
using Core.Entity;
using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly IMapper _mapper;

        public AuthController(
            UserManager<User> userManager,
            ITokenRepository tokenRepository,
            IMapper mapper)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
        }

        // GET: جلب جميع المستخدمين
        [HttpGet]
        public async Task<ActionResult<List<UserDtoRequest>>> GetallUserasync()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            var response = _mapper.Map<List<UserDtoRequest>>(users);
            return Ok(response);
        }

        // POST: تسجيل الدخول
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Email);

            if (identityUser is not null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(identityUser, request.Password);

                if (checkPasswordResult)
                {
                    var roles = await _userManager.GetRolesAsync(identityUser);
                    var jwtToken = _tokenRepository.CreateJwtToken(identityUser, roles.ToList());

                    var response = new LoginResponseDto()
                    {
                        Email = request.Email,
                        Roles = roles.ToList(),
                        Token = jwtToken,
                        Id = identityUser.Id
                    };

                    return Ok(response);
                }
            }

            ModelState.AddModelError("", "Email or Password Incorrect");
            return ValidationProblem(ModelState);
        }

        // POST: إنشاء حساب جديد
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var user = _mapper.Map<User>(request);
            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRoleAsync(user, "Reader");

                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return ValidationProblem(ModelState);
        }

        // POST: تغيير كلمة المرور (محمي بتسجيل الدخول)
        [HttpPost]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Email);

            if (identityUser == null)
            {
                return NotFound("المستخدم غير موجود.");
            }

            var identityResult = await _userManager.ChangePasswordAsync(
                identityUser, 
                request.CurrentPassword, 
                request.NewPassword
            );

            if (identityResult.Succeeded)
            {
                return Ok(new { message = "تم تغيير كلمة المرور بنجاح." });
            }

            if (identityResult.Errors.Any())
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return ValidationProblem(ModelState);
        }
    }
}