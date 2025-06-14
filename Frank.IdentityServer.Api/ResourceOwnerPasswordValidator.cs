using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace Frank.IdentityServer.Api;

internal class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        if (context.UserName == "alice" && context.Password == "alice")
        {
            context.Result = new GrantValidationResult(subject: "1", authenticationMethod: "custom");
        }
        else if (context.UserName == "bob" && context.Password == "bob")
        {
            context.Result = new GrantValidationResult(subject: "2", authenticationMethod: "custom");
        }
        else
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password.");
        }

        return Task.CompletedTask;
    }
}