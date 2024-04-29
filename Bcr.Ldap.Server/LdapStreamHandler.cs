
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
        _logger.LogError("ProcessAsync not implemented.");
        throw new NotImplementedException();
    }
}
