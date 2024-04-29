# .NET LDAP Server

## How I Created Things

```
➜  Bcr.Ldap dotnet new console -o Bcr.Ldap.Server
➜  Bcr.Ldap git:(main) ✗ cd Bcr.Ldap.Server 
➜  Bcr.Ldap.Server git:(main) ✗ dotnet add package Microsoft.Extensions.Hosting --version 8.0.0

➜  Bcr.Ldap git:(main) ✗ dotnet new xunit -o Bcr.Ldap.Server.Tests
➜  Bcr.Ldap git:(main) ✗ dotnet sln add Bcr.Ldap.Server.Tests 
```

## Random Links

https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
https://docs.ldap.com/specs/rfc4511.txt
