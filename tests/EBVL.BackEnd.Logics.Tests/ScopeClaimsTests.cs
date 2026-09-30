using System.Security.Claims;
using EBVL.BackEnd.Infrastructure.Authorization;
using Xunit;

namespace EBVL.BackEnd.Logics.Tests;

public sealed class ScopeClaimsTests
{
    [Theory]
    [InlineData("scp", "ebvl.audit.index ebvl.audit.view", "ebvl.audit.view")]
    [InlineData("scope", "ebvl.audit.index,ebvl.audit.view", "ebvl.audit.view")]
    [InlineData("permission", "[\"ebvl.audit.view\"]", "ebvl.audit.view")]
    public void ContainsAnyAcceptsSupportedClaimFormats(string claimType, string value, string expectedScope)
    {
        var principal = CreatePrincipal(new Claim(claimType, value));

        var result = ScopeClaims.ContainsAny(principal, expectedScope);

        Assert.True(result);
    }

    [Fact]
    public void ContainsAnyRejectsScopeWithDifferentCasing()
    {
        var principal = CreatePrincipal(new Claim("scp", "EBVL.AUDIT.VIEW"));

        var result = ScopeClaims.ContainsAny(principal, "ebvl.audit.view");

        Assert.False(result);
    }

    [Fact]
    public void GetValuesRemovesDuplicatesAcrossClaimTypes()
    {
        var principal = CreatePrincipal(
            new Claim("scp", "ebvl.audit.view"),
            new Claim("permission", "ebvl.audit.view"));

        var values = ScopeClaims.GetValues(principal);

        Assert.Equal(["ebvl.audit.view"], values);
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}
