using System;
using System.Collections.Generic;
using Game.Core.State;
using Game.Core.Effects;

namespace Game.Core.Data
{
    public sealed class CardDefinition
    {
        private readonly List<IEffect> _effects;

        public CardId Id { get; }
        public int Cost { get; }

        public IReadOnlyList<IEffect> Effects => _effects;

        public CardDefinition(CardId id, int cost, IEnumerable<IEffect> effects)
        {
            if (cost < 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "card cost must not be negative");
            if (effects == null)
                throw new ArgumentNullException(nameof(effects));

            _effects = new List<IEffect>(effects);

            foreach (var effect in _effects)
            {
                if (effect == null)
                    throw new ArgumentException($"card '{id}' has a null effect", nameof(effects));
            }

            Id = id;
            Cost = cost;
        }
    }
}