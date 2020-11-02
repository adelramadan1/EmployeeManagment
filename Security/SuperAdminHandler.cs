using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagment.Security
{
    public class SuperAdminHandler :AuthorizationHandler<ManageAdminRolesAndClamisRequirements>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClamisRequirements requirement)
        {
            if(context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
