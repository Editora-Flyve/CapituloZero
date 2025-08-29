using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace CapituloZero.IdentityTests.Helpers;

[CollectionDefinition("postgres")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "xUnit collection fixture pattern")]
public sealed class PostgresCollection : ICollectionFixture<PostgresContainerFixture>
{
}
