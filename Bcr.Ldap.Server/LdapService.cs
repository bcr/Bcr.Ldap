using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bcr.Ldap.Server;

public class LdapService : BackgroundService
{
    private readonly ILogger<LdapService> _logger;

    public LdapService(ILogger<LdapService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LdapService is starting.");
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // TODO: Add your background service logic here

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        finally
        {
            _logger.LogInformation("LdapService is exiting.");
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        // TODO: Add any cleanup logic here

        await base.StopAsync(stoppingToken);
    }
}