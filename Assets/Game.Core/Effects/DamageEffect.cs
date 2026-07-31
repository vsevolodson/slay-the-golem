using System;

namespace Game.Core.Effects
{
    public sealed class DamageEffect : IEffect
    {
        private readonly int _amount;

        public DamageEffect(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "damage must not be negative");

            _amount = amount;
        }

        public void Apply(EffectContext context)
        {
            var target = context.OpponentCombotant;

            var damage = _amount;

            var absorbed = Math.Min(target.Block, damage);
            target.ReduceBlock(absorbed);
            target.ReduceHealth(damage - absorbed);
        }
    }
}