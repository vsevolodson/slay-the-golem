using System;

namespace Game.Core.Effects
{
    public sealed class ApplyPoisonEffect : IEffect
    {
        private readonly int _amount;

        public ApplyPoisonEffect(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "poison must not be negative");

            _amount = amount;
        }

        public void Apply(EffectContext context)
        {
            context.OpponentCombatant.AddPoison(_amount);
        }
    }
}