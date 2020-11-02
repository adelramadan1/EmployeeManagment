using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagment.Models;
using EmployeeManagment.ViewModels.Adminstration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace EmployeeManagment.Controllers
{
    [Authorize(Roles ="Admin,Super Admin")]
    //[Authorize(Roles = "Super Admin")]

    public class AdminstrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdminstrationController> logger;

        public AdminstrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager,
            ILogger<AdminstrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userid)
        {

            var user = await userManager.FindByIdAsync(userid);
            ViewBag.userid = userid;

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id={userid} not found";
                return View("NotFound");
            }
            var existingUserClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = user.Id
            };
            foreach (Claim claim in ClaimStore.allClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                  ClaimType=claim.Type
                };
                if(existingUserClaims.Any(c=>c.Type==claim.Type && c.Value=="true"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model,string userid)
        {

            var user = await userManager.FindByIdAsync(userid);
            ViewBag.userid = userid;

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id={userid} not found";
                return View("NotFound");
            }
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);
            if(!result.Succeeded)
            {
                ModelState.AddModelError("","can not remove all claims");
                return View(model);
            }
            result = await userManager.AddClaimsAsync(user,model.Claims.Select(c=>new Claim(c.ClaimType,c.IsSelected?"true":"false")));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "can not add selected claim");
                return View(model);

            }
            return RedirectToAction("EditUser",new { id = userid });

        }
        [HttpGet]
        [Authorize(Policy = "EditPolicy")]
        public async Task<IActionResult> ManageUserRoles(string userid)
        {

            var user = await userManager.FindByIdAsync(userid);
            ViewBag.userid = userid;

            if (user==null)
            {
                ViewBag.ErrorMessage = $"User with Id={userid} not found";
                return View("NotFound");
            }
            
                var model = new List<UserRolesViewModel>();
                foreach (var role in await roleManager.Roles.ToListAsync())
                {
                    var userRolesViewModel = new UserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };
                    if(await userManager.IsInRoleAsync(user,role.Name))
                    {
                        userRolesViewModel.IsSelected = true;
                    }
                    else
                    {
                        userRolesViewModel.IsSelected = false;

                    }
                    model.Add(userRolesViewModel);
                }
            
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "EditPolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userid)
        {

            var user = await userManager.FindByIdAsync(userid);
            ViewBag.userid = userid;

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id={userid} not found";
                return View("NotFound");
            }
            var roles =await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user,roles);
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Can not remove all roles to user");
                return View(model);
            }
            result = await userManager.AddToRolesAsync(user,model.Where(r=>r.IsSelected).Select(r=>r.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can not add selected roles to user");
                return View(model);
            }
            return RedirectToAction("EditUser",new { id=userid});
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleCreateViewModel model)
        {
             if(ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole { Name = model.RoleName };
                IdentityResult result = await roleManager.CreateAsync(role);
                if(result.Succeeded)
                {
                 return  RedirectToAction("ListRoles","Adminstration");
                   // ViewBag.Inserted ="New Role Is Created Succssfully";
                }
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }

                return View(model);

            }
            return View();
        }
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles; 
            return View(roles);
        }

        [HttpGet]
        //[Authorize(Policy = "EditPolicy")]
        public  async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
           if(role==null)
            {
                ViewBag.ErrorMessage = $"Role with Id={id} is not found";
                return View("NotFound");
            }
            var mode = new RoleEditViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach(var user in await userManager.Users.ToListAsync())
            {
                if (await userManager.IsInRoleAsync(user,role.Name))
                {
                    mode.Users.Add(user.UserName);
                }
            }
            return View(mode);
        }
        [HttpPost]
        //[Authorize(Policy="EditPolicy")]
        public async Task<IActionResult> EditRole(RoleEditViewModel model)
        {
                 if(ModelState.IsValid)
            {
                IdentityRole role = await roleManager.FindByIdAsync(model.Id);
               
                    if (role == null)
                    {
                        ViewBag.ErrorMessage = $"Role with Id={model.Id} is not found";
                        return View("NotFound");
                    }
                    else
                    {
                        role.Name = model.RoleName;
                        IdentityResult result = await roleManager.UpdateAsync(role);
                        if(result.Succeeded)
                        {
                          return  RedirectToAction("ListRoles", "Adminstration");
                        }
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }


                    }
               
            }
            return View(model);

        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            ViewBag.roleId = roleId;
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={roleId} is not found";
                return View("NotFound");
            }
            var model = new List<UsersRoleViewModel>();
             foreach (var User in await userManager.Users.ToListAsync())
                {
                var userRoleViewModel = new UsersRoleViewModel
                {
                    UserId = User.Id,
                    UserName = User.UserName
                };
                if(await userManager.IsInRoleAsync(User,role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;

                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UsersRoleViewModel> models,string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            ViewBag.roleId = roleId;
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={roleId} is not found";
                return View("NotFound");
            }
            for (int i = 0; i < models.Count(); i++)
            {
                var User = await userManager.FindByIdAsync(models[i].UserId);
                IdentityResult result = null;
                if(models[i].IsSelected && (! await userManager.IsInRoleAsync(User,role.Name)) )
                {
                    result = await userManager.AddToRoleAsync(User, role.Name);
                }
                else if(!models[i].IsSelected && (await userManager.IsInRoleAsync(User, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(User, role.Name);

                }
                else
                {
                    continue;
                }
                if(result.Succeeded)
                {
                    if(i<(models.Count-1))
                    {
                        continue;

                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });

        }
        [HttpGet]
        //[Authorize(Policy = "EditPolicy")]
        public IActionResult ListUsers()
        {
            var Users = userManager.Users;
            return View(Users);
        }
         [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"this user has Id={id} is not Found";
                return View("NotFound");
            }
            var userCliams = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                City = user.City,
                UserClaims = userCliams.Select(c =>c.Type + " : "+ c.Value).ToList(),
                UserRoles = userRoles.ToList()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"this user has Id={model.Id} is not Found";
                return View("NotFound");
            }
            else
            {
                user.UserName = model.Username;
                user.Email = model.Email;
                user.City = model.City;
            }
            IdentityResult result = await userManager.UpdateAsync(user);
            if(result.Succeeded)
            {
                return RedirectToAction("ListUsers","Adminstration");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"this user has Id={id} is not Found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListUsers","Adminstration");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            return View("ListUsers");
        }
        [HttpPost]
        //[Authorize(Policy= "DeletePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role==null)
            {
                ViewBag.ErrorMessage = $"this role has Id ={ id} is not Found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles", "Adminstration");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {

                    logger.LogError($"Exception occured : { ex} ");
                    ViewBag.ErrorTitle = $"{role.Name} role in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted";
                    return View("Error");
                }
            }
           
        }
     }
}