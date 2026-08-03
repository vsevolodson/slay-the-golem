using System;
using System.Collections.Generic;
using Game.Core.Data;
using Game.Core.State;
using Game.Core.Effects;

namespace Game.Core
{
    internal sealed class TurnFlow
    {
        private static readonly PoisonTickEffect PoisonTick = new PoisonTickEffect();

        private readonly CombatState _state;
        private readonly CombatRules _rules;
        private readonly EffectSystem _effects;

        internal TurnFlow(CombatState state, CombatRules rules)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _effects = new EffectSystem(state);
        }

        internal void BeginPlayerTurn()
        {
            _state.SetActiveSide(Side.Player);
            _state.AdvanceTurnNumber();

            _state.Player.Combatant.ClearBlock();

            _effects.Apply(PoisonTick, Side.Player);
            if (_state.Outcome != CombatOutcome.InProgress)
                return;

            _state.Player.Combatant.ReduceVulnerable(1);
            DrawRules.Draw(_state.Player, _rules.CardsDrawnPerTurn);
            _state.Player.SetEnergy(_rules.EnergyPerTurn);
        }

        internal void EndPlayerTurn()
        {
            foreach (var card in new List<CardId>(_state.Player.Hand))
            {
                _state.Player.RemoveFromHand(card);
                _state.Player.AddToDiscard(card);
            }
        }

        internal void RunEnemyTurn()
        {
            _state.SetActiveSide(Side.Enemy);

            _state.Enemy.Combatant.ClearBlock();

            _effects.Apply(PoisonTick, Side.Enemy);
            if (_state.Outcome != CombatOutcome.InProgress)
                return

            _state.Enemy.Combatant.ReduceVulnerable(1);
        }
    }
}