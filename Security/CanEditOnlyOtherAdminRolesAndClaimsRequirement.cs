using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagment.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsRequirement:AuthorizationHandler<ManageAdminRolesAndClamisRequirements>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClamisRequirements requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if(authFilterContext==null)
            
                return Task.CompletedTask;
            
            string adminIdBeingEdited = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value; 
            string loginedInAdminId = authFilterContext.HttpContext.Request.Query["userid"];
            if(context.User.IsInRole("Admin") &&context.User.HasClaim(c=>c.Type=="Edit Role" && c.Value=="true")
                && adminIdBeingEdited.ToLower()!=loginedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
