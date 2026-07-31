using System;
using Game.Core.State;

namespace Game.Core.Effects
{
    public interface IEffects
    {
        void apply(EffectContext context);
    }
}