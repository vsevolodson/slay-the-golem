using System;
using Newtonsoft.Json;

namespace Game.Core.State
{
    public sealed class Combatant
    {
        [JsonProperty] public int MaxHealth { get; private set; }
        [JsonProperty] public int Health { get; private set; }
        [JsonProperty] public int Block { get; private set; }

        public Combatant(int maxHealth, int health)
        {
            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "max health must be positive");
            if (health <= 0 || health > maxHealth)
                throw new ArgumentOutOfRangeException(nameof(health), "health must be berween 1 and max health");

            MaxHealth = maxHealth;
            Health = health;
            Block = 0;
        }

        [JsonConstructor]
        private Combatant() { }

        public bool IsDead => Health <= 0;

        internal void ReduceHealth(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "damage must not be negative");

            Health = Math.Max(0, Health - amount);
        }

        internal void AddBlock(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "block must not be negative");

            Block += amount;
        }

        internal void ReduceBlock(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "reduce block must not be negative");

            Block = Math.Max(0, Block - amount);
        }

        internal void ClearBlock()
        {
            Block = 0;
        }
    }
}
