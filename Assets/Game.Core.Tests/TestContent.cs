using Game.Core.Data;

namespace Game.Core.Tests
{
    internal static class TestContent
    {
        internal const string RatBruiserId = "rat_bruiser";
        internal const int RatBruiserHealth = 24;

        // The cycle from the spec: attack 8, then attack 5 with block 5.
        internal static EnemyCatalog RatBruiser() => new EnemyCatalog(new[]
        {
            new EnemyDefinition(RatBruiserId, RatBruiserHealth,
                new[] { new EnemyIntent(damage: 8, block: 0), new EnemyIntent(damage: 5, block: 5) })
        });
    }
}