using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Guess.API.Authorization
{
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            // get permission set on token
            var permissions = context.User.Claims.Where(c => c.Type == "permissions" && c.Issuer == requirement.Issuer);

            // Succeed if the permissions array contains the required permission
            if (permissions.Any(s => s.Value == requirement.Permission))
                context.Succeed(requirement);

            return Task.CompletedTask;
         }
    }
}
