﻿using Duende.IdentityServer.Models;

namespace Frank.IdentityServer.Api;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("scope1"),
            new ApiScope("scope2"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = {new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())},

                AllowedScopes = {"scope1"}
            },

            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = {"https://localhost:7182/signin-oidc"},
                FrontChannelLogoutUri = "https://localhost:7182/signout-oidc",
                PostLogoutRedirectUris = {"https://localhost:7182/signout-callback-oidc"},

                AllowOfflineAccess = true,
                AllowedScopes = {"openid", "profile", "scope2"}
            },
            
            // Password grant client
            new Client
            {
                ClientId = "password.client",
                ClientName = "Password Grant Client",
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = {new Secret("A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6".Sha256())},
                AllowedScopes = {"openid", "profile", "scope1", "scope2", "offline_access"},
                AllowOfflineAccess = true
            }
        };
}