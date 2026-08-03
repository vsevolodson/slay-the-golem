using System;
using Game.Core.Effects;

namespace Game.Unity.Config
{
    public enum EffectKind
    {
        Damage,
        Block,
        ApplyPoison,
        ApplyVulnerable
    }

    [Serializable]
    public sealed class EffectEntry
    {
        public EffectKind Kind;
        public int Amount;

        public IEffect ToCoreEffect()
        {
            switch (Kind)
            {
                case EffectKind.Damage: return new DamageEffect(Amount);
                case EffectKind.Block: return new BlockEffect(Amount);
                case EffectKind.ApplyPoison: return new ApplyPoisonEffect(Amount);
                case EffectKind.ApplyVulnerable: return new ApplyVulnerableEffect(Amount);
                default: throw new NotSupportedException($"unknown effect kind '{Kind}'");
            }
        }
    }
}