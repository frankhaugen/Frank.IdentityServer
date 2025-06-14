namespace Frank.IdentityServer.LoginApp.Models;

public class LoginConfiguration
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Scope { get; set; } = null!;
}