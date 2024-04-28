using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bcr.Ldap.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<LdapService>();

using var host = builder.Build();

host.Run();
