using System;

namespace Game.Core.Effects
{
    public sealed class BlockEffect : IEffect
    {
        private readonly int _amount;

        public BlockEffect(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "block must not be negative");

            _amount = amount;
        }

        public void Apply(EffectContext context)
        {
            context.ActorCombatant.AddBlock(_amount);
        }
    }
}