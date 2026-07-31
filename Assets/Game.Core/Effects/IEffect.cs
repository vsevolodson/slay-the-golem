using System;
using Game.Core.State;

namespace Game.Core.Effects
{
    public interface IEffect
    {
        void Apply(EffectContext context);
    }
}