using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Numerics;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BlogApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Posts");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isUser = _userRepository.GetAll.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                if (isUser != null)
                {
                    // Burada formdan gelen verileri cookie'ye atama işlemlerini ve rol atama işlemlerini gerçekleştiriyoruz... 
                    var userClaims = new List<Claim>();

                    userClaims.Add(new Claim(ClaimTypes.NameIdentifier, isUser.UserId.ToString()));
                    userClaims.Add(new Claim(ClaimTypes.Name, isUser.Name ?? ""));
                    userClaims.Add(new Claim(ClaimTypes.GivenName, isUser.UserName ?? ""));
                    userClaims.Add(new Claim(ClaimTypes.UserData, isUser.Image ?? ""));

                    if (isUser.Email == "info@mehmetyildiz.com")
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
                    }

                    // Burada oluşturduğumuz claim'i Default olan cookie şemasına aktardık.
                    var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Burada "beni hatırla" property'si ekliyoruz. Ancak default true oldu.
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true// Beni hatırla check box'ı..
                    };

                    // Burada daha önce oluşturulan cookie'leri siliyoruz. 
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    // Login işlemini, oluşturduğumuz şemayı default şema olarak göstererek ve property'i ekleyerek gerçekleştirmiş oluyoruz. 
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Posts");
                }
                else
                {
                    ModelState.AddModelError("", "Your email or password is incorrect. Please check your information.");
                }
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image?.Length > 0)
                {
                    var allowedExtentions = new[] { ".jpg", ".jpeg", ".png" };
                    var extentions = Path.GetExtension(image.FileName.ToString());

                    if (!allowedExtentions.Contains(extentions))
                    {
                        ModelState.AddModelError("Image", "Please select a valid image format.");
                        return View(model);
                    }
                    var orginalName = Path.GetFileName(image.FileName);

                    orginalName = Regex.Replace(orginalName, @"[^a-zA-Z0-9]", "_");

                    var randomfileName = Guid.NewGuid().ToString();

                    var imageName = randomfileName + "_" + orginalName + extentions;

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    model.Image = imageName;
                }
                else
                {
                    model.Image = "avatar.jpg";
                }

                var user = await _userRepository.GetAll.FirstOrDefaultAsync(u => u.UserName == model.UserName || u.Email == model.Email);
                if (user == null)
                {
                    await _userRepository.CreateUser(new User
                    {
                        UserName = model.UserName,
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                        Image = model.Image
                    });
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Username and email were used.");
                }
            }

            return View(model);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }
            var user = await _userRepository
                       .GetAll
                       .Include(p => p.Posts)
                       .Include(c => c.Comments)
                       .ThenInclude(p => p.Post)
                       .FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userRepository.GetAll.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(new EditUserProfileViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Image = user.Image

            });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditUserProfileViewModel model, int id, IFormFile? image, string? oldImage)
        {
            if (model.UserId != id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    var allowedExtentions = new[] { ".jpg", ".jpeg", ".png" };
                    var extentions = Path.GetExtension(image.FileName.ToString());

                    if (!allowedExtentions.Contains(extentions))
                    {
                        ModelState.AddModelError("Image", "Please select a valid image format.");
                        return View(model);
                    }
                    var orginalName = Path.GetFileName(image.FileName);
                    var randomName = Guid.NewGuid().ToString();
                    var fileName = randomName + "_" + orginalName + extentions;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    if (oldImage != "avatar.jpg" && oldImage != null)
                    {
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }
                    model.Image = fileName;
                }
                else
                {
                    model.Image = oldImage;
                }

                var editUser = new User
                {
                    UserId = id,
                    UserName = model.UserName,
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Image = model.Image
                };
                await _userRepository.UpdateUser(editUser);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }
            model.Image = oldImage;
            return View(model);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditPassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userRepository.GetAll.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(new EditUserPasswordViewModel
            {
                UserId = user.UserId,
                Password = user.Password
            });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditPassword(EditUserPasswordViewModel model, int id)
        {
            var user = await _userRepository.GetAll.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null && model == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (user!.Password == model.OldPassword)
                {
                    if(user.Password == model.Password)
                    {
                        ModelState.AddModelError("", "The current password and the new password cannot be the same.");
                        return View(model);
                    }
                    var editPassword = new User
                    {
                        UserId = model.UserId,
                        Password = model.Password
                    };
                    await _userRepository.UpdateUserPassword(editPassword);
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect.");
                }
            }

            return View(model);
        }
    }
}
