
using System.Reflection.Metadata;
using Bcr.Ldap.Server.Messages;
using Microsoft.Extensions.Logging;

namespace Bcr.Ldap.Server;

class LdapStreamHandler : IStreamHandler
{
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

    enum LdapAuthenticationType
    {
        Simple = 0x80,
        SASL = 0xA3,
    }

    private readonly ILogger<LdapStreamHandler> _logger;

    public LdapStreamHandler(ILogger<LdapStreamHandler> logger)
    {
        _logger = logger;
    }

    private async Task HandleBindRequest(int messageID, BindRequest request, LdapMessageWriter writer)
    {
        var response = new LdapResult(LdapResultCode.Success, string.Empty, string.Empty);
        await writer.WriteAsync(messageID, (int) LdapProtocolOp.BindResponse, response);
    }

    private async Task HandleSearchRequest(int messageID, SearchRequest searchRequest, LdapMessageWriter writer)
    {
        _logger.LogInformation("Search request: {SearchRequest}", searchRequest);
        var response = new LdapResult(LdapResultCode.UnwillingToPerform, string.Empty, "Search not implemented");
        _logger.LogInformation("Search response: {Response}", response);
        await writer.WriteAsync(messageID, (int) LdapProtocolOp.BindResponse, response);
    }

    public async Task ProcessAsync(Stream stream, CancellationToken stoppingToken)
    {
        var reader = new BerReader(stream, stoppingToken);
        var writer = new LdapMessageWriter(stream, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // LDAPMessage ::= SEQUENCE {
                await reader.ExpectTag(BerReader.BerTag.Universal | BerReader.BerTag.Constructed | BerReader.BerTag.Sequence);

                // messageID MessageID,
                // MessageID ::= INTEGER (0 ..  maxInt)
                // maxInt INTEGER ::= 2147483647 -- (2^^31 - 1) --
                var messageID = await reader.ReadExpectedInteger();

                var tag = (LdapProtocolOp) await reader.ReadTag();

                switch (tag)
                {
                    case LdapProtocolOp.BindRequest:
                        // BindRequest ::= [APPLICATION 0] SEQUENCE {
                        var request = await BindRequest.DecodeAsync(reader);
                        await HandleBindRequest(messageID, request, writer);
                        break;
                    case LdapProtocolOp.SearchRequest:
                        // SearchRequest ::= [APPLICATION 3] SEQUENCE {
                        var searchRequest = await SearchRequest.DecodeAsync(reader);
                        await HandleSearchRequest(messageID, searchRequest, writer);
                        break;
                    default:
                        _logger.LogWarning("Unsupported LDAP message type: {Tag}", tag);
                        // Handle other commands here
                        break;
                }
            }
            catch (EndOfStreamException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing LDAP message");
                break;
            }
        }
    }
}
