using System;
using Orleans.ChaosMonkey;
using Orleans.Hosting;
using Orleans.TestingHost;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace Wordleans.Tests.EngineTests;

public class ChaosMonkeyCluster : ISiloConfigurator
{
    public static bool Enabled { get; set; } = false;

    public void Configure(ISiloBuilder siloBuilder)
    {
        if (!Enabled)
        {
            return;
        }

        var chaos = new SiloChaosConfigurator();
        chaos.Configure(siloBuilder);
    }
}

public class ClusterFixture : IDisposable
{
    private const string OutputTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{SourceContext}] {EventId} [{Level}] {Message}{NewLine}{Exception}";

    public TestCluster? Cluster { get; private set; }

    public ClusterFixture(IMessageSink sink)
    {
        SetupLogger(cfg =>
        {
            cfg.WriteTo.TestOutput
            (
                sink,
                LogEventLevel.Debug,
                OutputTemplate
            );
        });
        SetupCluster();
    }

    public ClusterFixture(ITestOutputHelper output)
    {
        SetupLogger(cfg =>
        {
            cfg.WriteTo.TestOutput
            (
                output,
                LogEventLevel.Debug,
                OutputTemplate
            );
        });
        SetupCluster();
    }

    private void SetupCluster()
    {
        Cluster = new TestClusterBuilder(3)
            .AddSiloBuilderConfigurator<TestEngineSiloConfigurator>()
            .AddSiloBuilderConfigurator<ChaosMonkeyCluster>()
            .AddClientBuilderConfigurator<TestClientConfigurator>()
            .Build();

        Cluster.Deploy();
        Log.Information("---------------- Cluster Ready ---------------- ");
    }

    private static void SetupLogger(Action<LoggerConfiguration> configure)
    {
        var cfg = new LoggerConfiguration();
        configure(cfg);
        Log.Logger = cfg
            .MinimumLevel.Information()
            .MinimumLevel.Override("Wordleans.Tests.EngineTests", LogEventLevel.Debug)
            .MinimumLevel.Override("Orleans", LogEventLevel.Warning)
            .MinimumLevel.Override("Orleans.Runtime.Silo", LogEventLevel.Error)
            .MinimumLevel.Override("Runtime.GrainDirectory.AdaptiveDirectoryCacheMaintainer", LogEventLevel.Warning)
            .MinimumLevel.Override("Runtime.Messaging.GatewayClientCleanupAgent", LogEventLevel.Warning)
            .CreateLogger();
    }

    public void Dispose()
    {
        Log.Information("---------------- Stopping Cluster ---------------- ");
        if (Cluster != null)
        {
            Cluster.StopAllSilos();
        }

        Log.CloseAndFlush();
    }
}