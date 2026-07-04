using Falck.Infrastructure.Authentication;

namespace Falck.Tests.Infrastructure;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void Hash_ThenVerify_RoundTrips()
    {
        var hash = _hasher.Hash("Sup3rSecret!");

        Assert.True(_hasher.Verify("Sup3rSecret!", hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = _hasher.Hash("Sup3rSecret!");

        Assert.False(_hasher.Verify("wrong-password", hash));
    }

    [Fact]
    public void Hash_IsSalted_ProducesDifferentHashesForSameInput()
    {
        Assert.NotEqual(_hasher.Hash("same-input"), _hasher.Hash("same-input"));
    }

    [Fact]
    public void VerifyDecoy_AlwaysReturnsFalse_ButRunsToKeepConstantTime()
    {
        // The decoy never matches any real password; its purpose is timing,
        // not authentication.
        Assert.False(_hasher.VerifyDecoy("anything"));
        Assert.False(_hasher.VerifyDecoy(string.Empty));
    }
}
