using System;
using Game.Core.State;

namespace Game.Core.Effects
{
    public sealed class EffectContext
    {
        internal EffectContext(CombatState state, Side actor)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            Actor = actor;
            Opponent = actor == Side.Player ? Side.Enemy : Side.Player;
        }

        public CombatState State { get; }
        public Side Actor { get; }
        public Side Opponent { get; }

        public Combatant ActorCombatant => State.GetCombatant(Actor);
        public Combatant OpponentCombatant => State.GetCombatant(Opponent);
    }
}