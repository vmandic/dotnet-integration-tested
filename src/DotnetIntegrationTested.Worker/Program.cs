using DotnetIntegrationTested.Worker;

var host = Startup.CreateDefaultBuilder(args);

var app = host.Build();
await app.RunAsync();
