using Microsoft.AspNetCore.DataProtection;

namespace FixtureForge.Infrastructure.Security;

public class ConnectionStringProtector
{
    private readonly IDataProtector _protector;

    public ConnectionStringProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("FixtureForge.ConnectionStrings.v1");
    }

    public string Protect(string plainText) => _protector.Protect(plainText);
    public string Unprotect(string protectedText) => _protector.Unprotect(protectedText);
}