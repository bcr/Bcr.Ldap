namespace Bcr.Ldap.Server;

public interface IStreamHandler
{
    Task ProcessAsync(Stream stream, CancellationToken stoppingToken);
}
