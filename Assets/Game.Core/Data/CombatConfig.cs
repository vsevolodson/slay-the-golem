using System;

namespace Game.Core.Data
{
    public sealed class CombatConfig
    {
        public CardCatalog Cards { get; }
        public CombatRules Rules { get; }
        public EnemyCatalog Enemies { get; }

        public CombatConfig(CardCatalog cards, EnemyCatalog enemies, CombatRules rules)
        {
            Cards = cards ?? throw new ArgumentNullException(nameof(cards));
            Enemies = enemies ?? throw new ArgumentNullException(nameof(enemies));
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }
    }
}