
using Microsoft.Extensions.Logging;

namespace Bcr.Ldap.Server;

class LdapStreamHandler : IStreamHandler
{
    enum LdapRequestType
    {
        BindRequest = 0x60,
        UnbindRequest = 0x42,
        SearchRequest = 0x63,
        SearchResultEntry = 0x64,
        SearchResultDone = 0x65,
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

    private readonly ILogger<LdapStreamHandler> _logger;

    public LdapStreamHandler(ILogger<LdapStreamHandler> logger)
    {
        _logger = logger;
    }

    public async Task ProcessAsync(Stream stream, CancellationToken stoppingToken)
    {
        var reader = new BerReader(stream, stoppingToken);

        // LDAPMessage ::= SEQUENCE {
        await reader.ExpectTag(BerReader.BerTag.Universal | BerReader.BerTag.Constructed | BerReader.BerTag.Sequence);
        var length = await reader.ReadLength();

        // messageID MessageID,
        // MessageID ::= INTEGER (0 ..  maxInt)
        // maxInt INTEGER ::= 2147483647 -- (2^^31 - 1) --
        await reader.ExpectTag(BerReader.BerTag.Universal | BerReader.BerTag.Primitive | BerReader.BerTag.Integer);
        var messageID = await reader.ReadInteger();

        var tag = await reader.ReadTag();
        if ((LdapRequestType) tag == LdapRequestType.BindRequest)
        {
            _logger.LogInformation("BindRequest");
        }
        throw new NotImplementedException();
    }
}
