namespace Game.Core.Effects
{
    internal sealed class PoisonTickEffect : IEffect
    {
        public void Apply(EffectContext context)
        {
            var owner = context.ActorCombatant;

            if (owner.Poison <= 0)
                return;

            owner.ReduceHealth(owner.Poison);
            owner.ReducePoison(1);
        }
    }
}