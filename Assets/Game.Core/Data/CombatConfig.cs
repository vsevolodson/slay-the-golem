using System;

namespace Game.Core.Data
{
    public sealed class CombatConfig
    {
        public CardCatalog Cards { get; }
        public CombatRules Rules { get; }

        public CombatConfig(CardCatalog cards, CombatRules rules)
        {
            Cards = cards ?? throw new ArgumentNullException(nameof(cards));
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }
    }
}