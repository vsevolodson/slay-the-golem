using System;

namespace Game.Core.Effects
{
    public sealed class ApplyVulnerableEffect : IEffect
    {
        private readonly int _amount;

        public ApplyVulnerableEffect(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "vulnerable must not be negative");

            _amount = amount;
        }

        public void Apply(EffectContext context)
        {
            context.OpponentCombatant.AddVulnerable(_amount);
        }
    }
}