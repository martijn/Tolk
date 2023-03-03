using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tolk.FunctionApp;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s => s
        .AddSingleton<IProjectFactory, ProjectFactory>()
        .AddSingleton<IProjectBuilder, ProjectBuilder>())
    .Build();

host.Run();
