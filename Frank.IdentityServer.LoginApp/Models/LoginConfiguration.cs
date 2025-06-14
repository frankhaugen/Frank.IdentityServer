namespace Frank.IdentityServer.LoginApp.Models;

public class LoginConfiguration
{
    public string Authority { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Scope { get; set; } = null!;
}