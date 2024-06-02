// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using static IdentityModel.OidcConstants;

namespace IdentityServer4.AspNetIdentity;

/// <summary>
/// IResourceOwnerPasswordValidator that integrates with ASP.NET Identity.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
/// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
/// <remarks>
/// Initializes a new instance of the <see cref="ResourceOwnerPasswordValidator{TUser}"/> class.
/// </remarks>
/// <param name="userManager">The user manager.</param>
/// <param name="signInManager">The sign in manager.</param>
/// <param name="logger">The logger.</param>
public class ResourceOwnerPasswordValidator<TUser>(
    UserManager<TUser> userManager,
    SignInManager<TUser> signInManager,
    ILogger<ResourceOwnerPasswordValidator<TUser>> logger) : IResourceOwnerPasswordValidator
    where TUser : class
{
    private readonly SignInManager<TUser> _signInManager = signInManager;
    private readonly UserManager<TUser> _userManager = userManager;
    private readonly ILogger<ResourceOwnerPasswordValidator<TUser>> _logger = logger;

    /// <summary>
    /// Validates the resource owner password credential
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public virtual async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var user = await _userManager.FindByNameAsync(context.UserName);
        if (user != null)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, true);
            if (result.Succeeded)
            {
                var sub = await _userManager.GetUserIdAsync(user);

                _logger.LogInformation("Credentials validated for username: {Username}", context.UserName);

                context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                return;
            }
            else if (result.IsLockedOut)
            {
                _logger.LogInformation("Authentication failed for username: {Username}, reason: locked out", context.UserName);
            }
            else if (result.IsNotAllowed)
            {
                _logger.LogInformation("Authentication failed for username: {Username}, reason: not allowed", context.UserName);
            }
            else
            {
                _logger.LogInformation("Authentication failed for username: {Username}, reason: invalid credentials", context.UserName);
            }
        }
        else
        {
            _logger.LogInformation("No user found matching username: {Username}", context.UserName);
        }

        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
    }
}
