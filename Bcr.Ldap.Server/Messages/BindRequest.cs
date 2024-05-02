namespace Bcr.Ldap.Server.Messages;

record BindRequest(int Version, string Name)
{
    internal static async Task<BindRequest> DecodeAsync(BerReader reader)
    {
        // version INTEGER (1 .. 127),
        var version = await reader.ReadExpectedInteger();
        // !!! May need to check version here
        // name LDAPDN,
        var name = await reader.ReadExpectedLdapDN();
        // authentication AuthenticationChoice }
        var tag = await reader.ReadTag();
        // !!! Implement authentication, skip the element for now
        await reader.SkipElement();
        return new BindRequest(version, name);
    }
}
