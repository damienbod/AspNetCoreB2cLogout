# ASP.NET Core Session Timeout Razor Pages with Azure AD B2C

[![.NET](https://github.com/damienbod/AspNetCoreB2cLogout/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AspNetCoreB2cLogout/actions/workflows/dotnet.yml)

This repository shows how an ASP.NET Core Razor Page application could implement a automatic sign-out when a user does not use the application from n minutes. The application is secured using Azure AD B2C. To remove the session, the client must sign-out both on the ASP.NET Core application and the Azure AD B2C identity provider.

[ASP.NET Core Session Timeout Razor Pages with Azure AD B2C](https://damienbod.com)

# Links

https://learn.microsoft.com/en-us/azure/active-directory-b2c/openid-connect#send-a-sign-out-request

https://learn.microsoft.com/en-us/aspnet/core/razor-pages/filter

https://github.com/AzureAD/microsoft-identity-web
