
using Microsoft.Extensions.Logging;

namespace Bcr.Ldap.Server;

class LdapStreamHandler : IStreamHandler
{
    private readonly ILogger<LdapStreamHandler> _logger;

    public LdapStreamHandler(ILogger<LdapStreamHandler> logger)
    {
        _logger = logger;
    }

    public Task ProcessAsync(Stream stream, CancellationToken stoppingToken)
    {
        var reader = new BerReader(stream);

        // LDAPMessage ::= SEQUENCE {
        var tag = reader.ReadTag();
        var length = reader.ReadLength();

        throw new NotImplementedException();
    }
}
