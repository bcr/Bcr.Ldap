
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

    enum LdapResultCode
    {
        Success = 0,
        OperationsError = 1,
        ProtocolError = 2,
        TimeLimitExceeded = 3,
        SizeLimitExceeded = 4,
        CompareFalse = 5,
        CompareTrue = 6,
        AuthMethodNotSupported = 7,
        StrongerAuthRequired = 8,
        Referral = 10,
        AdminLimitExceeded = 11,
        UnavailableCriticalExtension = 12,
        ConfidentialityRequired = 13,
        SaslBindInProgress = 14,
        NoSuchAttribute = 16,
        UndefinedAttributeType = 17,
        InappropriateMatching = 18,
        ConstraintViolation = 19,
        AttributeOrValueExists = 20,
        InvalidAttributeSyntax = 21,
        NoSuchObject = 32,
        AliasProblem = 33,
        InvalidDNSyntax = 34,
        AliasDereferencingProblem = 36,
        InappropriateAuthentication = 48,
        InvalidCredentials = 49,
        InsufficientAccessRights = 50,
        Busy = 51,
        Unavailable = 52,
        UnwillingToPerform = 53,
        LoopDetect = 54,
        NamingViolation = 64,
        ObjectClassViolation = 65,
        NotAllowedOnNonLeaf = 66,
        NotAllowedOnRDN = 67,
        EntryAlreadyExists = 68,
        ObjectClassModsProhibited = 69,
        AffectsMultipleDSAs = 71,
        Other = 80,
    }

    private readonly ILogger<LdapStreamHandler> _logger;

    public LdapStreamHandler(ILogger<LdapStreamHandler> logger)
    {
        _logger = logger;
    }

    private void HandleBindRequest(BindRequest request)
    {
        _logger.LogInformation("BindRequest: {BindRequest}", request);
    }

    public async Task ProcessAsync(Stream stream, CancellationToken stoppingToken)
    {
        var reader = new BerReader(stream, stoppingToken);

        // LDAPMessage ::= SEQUENCE {
        await reader.ExpectTag(BerReader.BerTag.Universal | BerReader.BerTag.Constructed | BerReader.BerTag.Sequence);

        // messageID MessageID,
        // MessageID ::= INTEGER (0 ..  maxInt)
        // maxInt INTEGER ::= 2147483647 -- (2^^31 - 1) --
        await reader.ExpectTag(BerReader.BerTag.Universal | BerReader.BerTag.Primitive | BerReader.BerTag.Integer);
        var messageID = await reader.ReadInteger();

        var tag = await reader.ReadTag();

        // BindRequest ::= [APPLICATION 0] SEQUENCE {
        if ((LdapProtocolOp) tag == LdapProtocolOp.BindRequest)
        {
            var request = await BindRequest.DecodeAsync(reader);
            HandleBindRequest(request);
        }
        throw new NotImplementedException();
    }
}
