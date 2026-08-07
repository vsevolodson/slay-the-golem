using System;
using System.Collections.Generic;
using Game.Core.Effects;

namespace Game.Core.Data
{
    public sealed class RelicDefinition
    {
        private readonly List<IEffect> _combatStartEffects;

        public string Id { get; }

        public IReadOnlyList<IEffect> CombatStartEffects => _combatStartEffects;

        public RelicDefinition(string id, IEnumerable<IEffect> combatStartEffects)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("relic id must not be empty", nameof(id));
            if (combatStartEffects == null)
                throw new ArgumentNullException(nameof(combatStartEffects));

            _combatStartEffects = new List<IEffect>(combatStartEffects);

            foreach (var effect in _combatStartEffects)
            {
                if (effect == null)
                    throw new ArgumentException($"relic '{id}' has a null effect", nameof(combatStartEffects));
            }

            Id = id;
        }
    }
}