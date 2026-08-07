using System;

namespace Game.Core.Effects
{
    public sealed class DrawCardsEffect : IEffect
    {
        private readonly int _count;

        public DrawCardsEffect(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "card count must not be negative");

            _count = count;
        }

        public void Apply(EffectContext context)
        {
            DrawRules.Draw(context.State.Player, _count, context.State.Rng);
        }
    }
}
