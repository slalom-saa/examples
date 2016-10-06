namespace Authentication.IdentityServer.Models
{
    public enum VerifyCodeStatus
    {
        None,
        Success,
        LockedOut,
        Invalid
    }
}