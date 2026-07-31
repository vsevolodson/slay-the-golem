using System;
using System.Collections.Generic;
using Game.Core.State;

namespace Game.Core.Effects
{
    internal sealed class EffectSystem
    {
        private readonly CombatState _state;

        internal EffectSystem(CombatState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        internal void Apply(IEffect effect, Side actor)
        {
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            if (_state.Outcome != CombatOutcome.InProgress)
                return;

            effect.Apply(new EffectContext(_state, actor));
            UpdateOutcome();
        }

        private void UpdateOutcome()
        {
            if (_state.Player.Combatant.IsDead)
                _state.SetOutcome(CombatOutcome.PlayerLost);
            else if (_state.Enemy.Combatant.IsDead)
                _state.SetOutcome(CombatOutcome.PlayerWon);

        }
    }
}