using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.State;
using Game.Core.Effects;

namespace Game.Core
{
    public sealed class Combat
    {
        private readonly CombatConfig _config;
        private readonly TurnFlow _turnFlow;
        private readonly EffectSystem _effects;
        private readonly EnemyDefinition _enemy;

        public CombatState State { get; }
        public EnemyIntent EnemyIntent => _enemy.Cycle[State.Enemy.IntentIndex];

        private Combat(CombatState state, CombatConfig config)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            RequireKnownCards(state, config.Cards);

            _enemy = config.Enemies.Get(state.Enemy.EnemyId);
            _turnFlow = new TurnFlow(state, config.Rules, _enemy);
            _effects = new EffectSystem(state);
        }

        public static Combat StartNew(CombatSetup setup, CombatConfig config)
        {
            var combat = new Combat(CombatState.Create(setup), config);

            combat.ShuffleStartingDeck();
            combat._turnFlow.BeginPlayerTurn();

            return combat;
        }

        public static Combat Resume(CombatState state, CombatConfig config) => new Combat(state, config);

        public CommandResult Validate(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (State.Outcome != CombatOutcome.InProgress)
                return CommandResult.Rejected(CommandRejection.CombatIsOver);

            switch (command)
            {
                case PlayCardCommand playCard: return ValidatePlayCard(playCard);
                case EndTurnCommand _: return ValidateEndTurn();
                default: return CommandResult.Rejected(CommandRejection.UnknownCommand);
            }
        }

        public CommandResult Execute(ICommand command)
        {
            var validation = Validate(command);
            if (!validation.IsSuccess)
                return validation;

            switch (command)
            {
                case PlayCardCommand playCard:
                    ApplyPlayCard(playCard);
                    break;
                case EndTurnCommand _:
                    ApplyEndTurn();
                    break;
                default:
                    throw new InvalidOperationException(
                        $"command '{command.GetType().Name}' passed validation but has no application step");
            }

            return CommandResult.Success();
        }

        private CommandResult ValidatePlayCard(PlayCardCommand command)
        {
            if (State.ActiveSide != Side.Player)
                return CommandResult.Rejected(CommandRejection.NotYourTurn);

            if (!State.Player.Hand.Contains(command.Card))
                return CommandResult.Rejected(CommandRejection.CardNotInHand);

            if (State.Player.Energy < _config.Cards.Get(command.Card).Cost)
                return CommandResult.Rejected(CommandRejection.NotEnoughEnergy);

            return CommandResult.Success();
        }

        private CommandResult ValidateEndTurn()
        {
            if (State.ActiveSide != Side.Player)
                return CommandResult.Rejected(CommandRejection.NotYourTurn);

            return CommandResult.Success();
        }

        private void ApplyPlayCard(PlayCardCommand command)
        {
            var definition = _config.Cards.Get(command.Card);

            State.Player.SpendEnergy(definition.Cost);
            State.Player.RemoveFromHand(command.Card);
            _effects.Apply(definition.Effects, Side.Player);
            State.Player.AddToDiscard(command.Card);
        }

        private void ApplyEndTurn()
        {
            _turnFlow.EndPlayerTurn();

            _turnFlow.RunEnemyTurn();
            if (State.Outcome != CombatOutcome.InProgress)
                return;

            _turnFlow.BeginPlayerTurn();
        }

        private static void RequireKnownCards(CombatState state, CardCatalog cards)
        {
            RequireKnownCards(state.Player.DrawPile, cards);
            RequireKnownCards(state.Player.Hand, cards);
            RequireKnownCards(state.Player.DiscardPile, cards);
        }

        private static void RequireKnownCards(IReadOnlyList<CardId> pile, CardCatalog cards)
        {
            foreach (var card in pile)
            {
                if (!cards.Contains(card))
                    throw new ArgumentException($"card '{card}' is not in the catalog");
            }
        }

        private void ShuffleStartingDeck()
        {
            var deck = new List<CardId>(State.Player.DrawPile);
            ShuffleRules.Shuffle(deck, State.Rng);

            State.Player.SetDrawPile(deck);
        }
    }
}