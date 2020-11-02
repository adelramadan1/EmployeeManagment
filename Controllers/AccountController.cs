using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagment.Models;
using EmployeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace EmployeeManagment.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> Logger;
        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.Logger = logger;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email,string token)
        {
            if(email==null || token==null)
            {
                ModelState.AddModelError("", "Inviald Password reset token");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user!=null)
                {
                    var result = await userManager.ResetPasswordAsync(user,model.Token,model.Password);
                    if(result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                    return View(model);

                }
                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword","Account",new { email=model.Email,token=token},Request.Scheme);
                    Logger.Log(LogLevel.Warning,passwordResetLink);
                    return View("ForgetPasswordConfirmation");
                }
                return View("ForgetPasswordConfirmation");
            }
            return View();
        }

        [HttpGet]
         [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()

            };
            return View(model);
        }
        [HttpPost]

        public async Task<IActionResult> Logout()
        {
          await  signInManager.SignOutAsync();

            return RedirectToAction("Index","Home");
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginView,string returnUrl )
        {
            loginView.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(loginView.Email);
                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, loginView.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not Confirmed yet");
                    return View(loginView);
                }

                var result= await signInManager.PasswordSignInAsync(loginView.Email, loginView.Password, loginView.RemeberMe, false);
            if(result.Succeeded)
                {
                   
                    if(!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    
                }
                ModelState.AddModelError(string.Empty,"Invalid Login Attempt");
            }
            
            return View(loginView);
        }
        [HttpPost]
        [AllowAnonymous]
        public  async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City=model.City
                };
                var result= await userManager.CreateAsync(user,model.Password);
                if(result.Succeeded)
                {
                    var Token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail","Account",new { userId=user.Id,token=Token},Request.Scheme);
                    Logger.Log(LogLevel.Warning,confirmationLink);
                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Adminstration");

                    }
                    ViewBag.ErrorTitle = "Registration Sccussfully";
                    ViewBag.ErrorMessage = "Before you can login, Please confirm your email by clicking on confirmation Link we have emailed you ";
                    return View("Error");
                   // await signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("Index", "Home");

                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if(userId==null || token==null)
            {
                return RedirectToAction("Index","Home");
            }
            var user = await userManager.FindByIdAsync(userId);
            if(user==null)
            {
                ViewBag.ErrorMessage = $"the user has id={userId} is not vaild";
                return View("NotFound");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "Email can't be Confirmed";
            return View("Error");
        }
        [AcceptVerbs("Get","Post")]
        public async Task<IActionResult> IsEmailInUse(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if(user==null)
            { 
                return Json(true); 
            }
            else
            {
                return Json($"Email {Email} is already in use. ");
            }

        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogins(string provider,string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback","Account",new { ReturnUrl=returnUrl});
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider,redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("/");
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if(remoteError!=null)
            {
                ModelState.AddModelError(string.Empty,$"Error from External provider :{remoteError}");
                return View("Login",model);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information");
                return View("Login", model);
            }
            var Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;
            if(Email!=null)
            {
                user = await userManager.FindByEmailAsync(Email);
                if(user!=null&&!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Login", model);
                }
            }
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if(signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            
            else
            {
               
                if(Email !=null)
                {
                     user = await userManager.FindByEmailAsync(Email);
                    if(user==null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await userManager.CreateAsync(user);
                        var Token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = Token }, Request.Scheme);
                        Logger.Log(LogLevel.Warning, confirmationLink);
                        if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                        {
                            return RedirectToAction("ListUsers", "Adminstration");

                        }
                        ViewBag.ErrorTitle = "Registration Sccussfully";
                        ViewBag.ErrorMessage = "Before you can login, Please confirm your email by clicking on confirmation Link we have emailed you ";
                        return View("Error");
                    }
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                ViewBag.ErrorTitle = $"Email claim not received from {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on adelramadan301@gmail.com";
                return View("Error");
            }
        }




    }
}