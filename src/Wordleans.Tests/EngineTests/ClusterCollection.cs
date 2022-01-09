using Xunit;

namespace Wordleans.Tests.EngineTests;

[CollectionDefinition(ClusterCollection.Name)]
public class ClusterCollection : ICollectionFixture<ClusterFixture>
{    
    public const string Name = "Integration-Engine";
}