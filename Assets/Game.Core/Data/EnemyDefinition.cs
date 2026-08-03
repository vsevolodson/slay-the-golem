using System;
using System.Collections.Generic;

namespace Game.Core.Data
{
    public sealed class EnemyDefinition
    {
        private readonly List<EnemyIntent> _cycle;

        public string Id { get; }
        public int MaxHealth { get; }

        public IReadOnlyList<EnemyIntent> Cycle => _cycle;

        public EnemyDefinition(string id, int maxHealth, IEnumerable<EnemyIntent> cycle)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("enemy id must not be empty", nameof(id));
            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "enemy max health must be positive");
            if (cycle == null)
                throw new ArgumentNullException(nameof(cycle));

            _cycle = new List<EnemyIntent>(cycle);

            if (_cycle.Count == 0)
                throw new ArgumentException("enemy cycle must not be empty", nameof(cycle));

            foreach (var intent in _cycle)
            {
                if (intent == null)
                    throw new ArgumentException($"enemy '{id}' has a null intent", nameof(cycle));
            }

            Id = id;
            MaxHealth = maxHealth;
        }
    }
}