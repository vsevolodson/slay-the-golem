using System;
using System.Collections.Generic;

namespace Game.Core.State
{
    public sealed class CombatSetup
    {
        public int PlayerMaxHealth { get; }
        public int PlayerHealth { get; }
        public IReadOnlyList<CardId> StartingDeck { get; }
        public string EnemyId { get; }
        public int EnemyMaxHealth { get; }
        public ulong Seed { get; }
        public string RelicId { get; }

        public CombatSetup(int playerMaxHealth, int playerHealth, IReadOnlyList<CardId> startingDeck,
            string enemyId, int enemyMaxHealth, ulong seed, string relicId = null)
        {
            if (playerMaxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(playerMaxHealth), "Max health must be positive.");
            if (playerHealth <= 0 || playerHealth > playerMaxHealth)
                throw new ArgumentOutOfRangeException(nameof(playerHealth), "Health must be between 1 and max health.");
            if (string.IsNullOrWhiteSpace(enemyId))
                throw new ArgumentException("Enemy id must not be empty.", nameof(enemyId));
            if (enemyMaxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(enemyMaxHealth), "Enemy max health must be positive.");

            PlayerMaxHealth = playerMaxHealth;
            PlayerHealth = playerHealth;
            StartingDeck = startingDeck ?? throw new ArgumentNullException(nameof(startingDeck));
            EnemyId = enemyId;
            EnemyMaxHealth = enemyMaxHealth;
            Seed = seed;
            RelicId = relicId;
        }
    }
}
