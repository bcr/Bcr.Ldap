namespace Bcr.Ldap.Server.Messages;

enum SearchScope
{
    BaseObject = 0,
    SingleLevel = 1,
    WholeSubtree = 2,
}

enum DerefAliases
{
    NeverDerefAliases = 0,
    DerefInSearching = 1,
    DerefFindingBaseObj = 2,
    DerefAlways = 3,
}

record SearchRequest(string BaseObject, SearchScope Scope, DerefAliases DerefAliases, int SizeLimit, int TimeLimit, bool TypesOnly)
{
    internal static async Task<SearchRequest> DecodeAsync(BerReader reader)
    {
        // baseObject LDAPDN,
        var baseDn = await reader.ReadExpectedLdapDN();
        // scope ENUMERATED {
        //     baseObject              (0),
        //     singleLevel             (1),
        //     wholeSubtree            (2) },
        var scope = await reader.ReadExpectedEnumeratedValue<SearchScope>();
        // derefAliases ENUMERATED {
        //     neverDerefAliases       (0),
        //     derefInSearching        (1),
        //     derefFindingBaseObj     (2),
        //     derefAlways             (3) },
        var derefAliases = await reader.ReadExpectedEnumeratedValue<DerefAliases>();
        // sizeLimit INTEGER (0 .. maxInt),
        var sizeLimit = await reader.ReadExpectedInteger();
        // timeLimit INTEGER (0 .. maxInt),
        var timeLimit = await reader.ReadExpectedInteger();
        // typesOnly BOOLEAN,
        var typesOnly = await reader.ReadExpectedBoolean();
        // filter Filter,
        // var filter = await reader.ReadFilter();
        await reader.ReadTag();
        await reader.SkipElement();
        // attributes AttributeDescriptionList }
        // var attributes = await reader.ReadAttributeDescriptionList();
        await reader.ReadTag();
        await reader.SkipElement();
        return new SearchRequest(baseDn, scope, derefAliases, sizeLimit, timeLimit, typesOnly);
    }
}