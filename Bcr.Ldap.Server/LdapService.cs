using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Bcr.Ldap.Server;

public class LdapService : BackgroundService
{
    private readonly ILogger<LdapService> _logger;
    private readonly IServiceProvider _services;

    private const int LdapPort = 389;

    public LdapService(ILogger<LdapService> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();
        _logger.LogInformation("LdapService is starting.");

        // Create a TcpListener
        var listener = new TcpListener(IPAddress.Any, LdapPort);
        _logger.LogInformation("Listening for connections on {ConnectionInfo}", listener.LocalEndpoint);

        try
        {
            listener.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                // Listen for an incoming connection
                var client = await listener.AcceptTcpClientAsync(stoppingToken);
                _logger.LogInformation("Accepted connection from {ConnectionInfo}", client.Client.RemoteEndPoint);

                // Handle the connection in a separate task
                // !!! Who cleans up the task and removes it from the list?
                // !!! Who complains if the task fails?
                tasks.Add(Task.Run(async () =>
                {
                    using (client)
                    {
                        using (var stream = client.GetStream())
                        {
                            using (var scope = _services.CreateScope())
                            {
                                var handler = scope.ServiceProvider.GetRequiredService<IStreamHandler>();
                                await handler.ProcessAsync(stream, stoppingToken);
                            }
                        }
                    }
                }));
            }
        }
        finally
        {
            listener.Stop();
            _logger.LogInformation("LdapService is exiting.");
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        // TODO: Add any cleanup logic here

        await base.StopAsync(stoppingToken);
    }
}