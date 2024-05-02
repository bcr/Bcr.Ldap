namespace Bcr.Ldap.Server;

enum LdapProtocolOp
{
    BindRequest = 0x60,
    BindResponse = 0x61,
    UnbindRequest = 0x42,
    SearchRequest = 0x63,
    SearchResultEntry = 0x64,
    SearchResultDone = 0x65,
    SearchResultReference = 0x73,
    ModifyRequest = 0x66,
    ModifyResponse = 0x67,
    AddRequest = 0x68,
    AddResponse = 0x69,
    DelRequest = 0x4A,
    DelResponse = 0x6B,
    ModifyDNRequest = 0x6C,
    ModifyDNResponse = 0x6D,
    CompareRequest = 0x6E,
    CompareResponse = 0x6F,
    AbandonRequest = 0x50,
    ExtendedRequest = 0x77,
    ExtendedResponse = 0x78,
    IntermediateResponse = 0x79,
}
