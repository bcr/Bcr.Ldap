using Bcr.Ldap.Server.Messages;

namespace Bcr.Ldap.Server;

class LdapMessageWriter
{
    private readonly Stream _stream;
    private readonly CancellationToken _stoppingToken;

    public LdapMessageWriter(Stream stream, CancellationToken stoppingToken)
    {
        _stream = stream;
        _stoppingToken = stoppingToken;
    }

    public async Task WriteAsync(int messageID, LdapProtocolOp protocolOpTag, LdapResult result)
    {
        var derEncoder = new DerEncoder();

        // LDAPMessage ::= SEQUENCE {
        derEncoder.AddTag((byte) (BerReader.BerTag.Universal | BerReader.BerTag.Constructed | BerReader.BerTag.Sequence));

        // messageID MessageID,
        // MessageID ::= INTEGER (0 ..  maxInt)
        derEncoder.AddInteger(messageID);

        // protocolOp CHOICE {
        // BindResponse ::= [APPLICATION 1] LDAPResult
        derEncoder.AddTag((int) protocolOpTag);

        // LDAPResult ::= SEQUENCE {
        // derEncoder.AddTag((byte) (BerReader.BerTag.Universal | BerReader.BerTag.Constructed | BerReader.BerTag.Sequence));

        // resultCode ENUMERATED {
        derEncoder.AddEnumerated((int)result.ResultCode);

        // matchedDN LDAPDN,
        // LDAPDN ::= LDAPString
        derEncoder.AddLdapString(result.MatchedDN);

        // diagnosticMessage LDAPString
        derEncoder.AddLdapString(result.DiagnosticMessage);

        // Write to stream
        await derEncoder.WriteToStreamAsync(_stream, _stoppingToken);
    }
}