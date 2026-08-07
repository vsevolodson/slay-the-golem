using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.State;

namespace Game.Core
{
    public sealed class Run
    {
        private readonly RunConfig _config;

        public RunState State { get; }
        public Combat CurrentCombat { get; private set; }

        private Run(RunState state, RunConfig config)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public static Run StartNew(RunConfig config, ulong seed)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var run = new Run(RunState.Create(config.PlayerMaxHealth, config.StartingDeck, seed), config);
            run.BeginNextFight();

            return run;
        }

        public static bool TryResume(RunState state, RunConfig config, out Run run)
        {
            run = null;

            if (state == null || config == null)
                return false;

            if (!CanResume(state, config))
                return false;

            try
            {
                var resumed = new Run(state, config);

                if (resumed.State.Phase == RunPhase.InCombat)
                    resumed.StartCombat();

                run = resumed;
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        private static bool CanResume(RunState state, RunConfig config)
        {
            if (state.Deck == null || state.RewardOffer == null || state.Rng == null)
                return false;

            if (state.MaxHealth <= 0)
                return false;

            if (state.Health < 0 || state.Health > state.MaxHealth)
                return false;

            if (state.Health == 0 && state.Phase != RunPhase.Lost)
                return false;

            if (state.FightIndex < 0 || state.FightIndex >= config.Fights.Count)
                return false;

            if (state.Phase == RunPhase.ChoosingReward && state.RewardOffer.Count == 0)
                return false;

            foreach (var card in state.Deck)
            {
                if (!config.Combat.Cards.Contains(card))
                    return false;
            }

            foreach (var card in state.RewardOffer)
            {
                if (!config.Combat.Cards.Contains(card))
                    return false;
            }

            return true;
        }

        public CommandResult Validate(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command is ChooseRewardCommand chooseReward)
                return ValidateChooseReward(chooseReward);

            if (State.Phase != RunPhase.InCombat)
                return CommandResult.Rejected(CommandRejection.WrongPhase);

            return CurrentCombat.Validate(command);
        }

        public CommandResult Execute(ICommand command)
        {
            var validation = Validate(command);
            if (!validation.IsSuccess)
                return validation;

            if (command is ChooseRewardCommand chooseReward)
            {
                ApplyChooseReward(chooseReward);
                return CommandResult.Success();
            }

            var result = CurrentCombat.Execute(command);
            SyncFinishedCombat();

            return result;
        }

        private CommandResult ValidateChooseReward(ChooseRewardCommand command)
        {
            if (State.Phase != RunPhase.ChoosingReward)
                return CommandResult.Rejected(CommandRejection.WrongPhase);

            if (!State.RewardOffer.Contains(command.Card))
                return CommandResult.Rejected(CommandRejection.CardNotOffered);

            return CommandResult.Success();
        }

        private void ApplyChooseReward(ChooseRewardCommand command)
        {
            State.AddCard(command.Card);
            State.ClearRewardOffer();
            State.AdvanceFight();

            BeginNextFight();
        }

        private void BeginNextFight()
        {
            State.SetCombatSeed(State.Rng.NextUInt64());
            State.SetPhase(RunPhase.InCombat);

            StartCombat();
        }

        private void SyncFinishedCombat()
        {
            var outcome = CurrentCombat.State.Outcome;
            if (outcome == CombatOutcome.InProgress)
                return;

            State.SetHealth(CurrentCombat.State.Player.Combatant.Health);

            if (outcome == CombatOutcome.PlayerLost)
            {
                State.SetPhase(RunPhase.Lost);
                return;
            }

            if (State.FightIndex + 1 >= _config.Fights.Count)
            {
                State.SetPhase(RunPhase.Won);
                return;
            }

            State.SetPhase(RunPhase.ChoosingReward);
            State.SetRewardOffer(PickRewardOffer());
        }

        private List<CardId> PickRewardOffer()
        {
            var pool = new List<CardId>(_config.RewardPool);
            ShuffleRules.Shuffle(pool, State.Rng);

            return pool.GetRange(0, Math.Min(_config.RewardChoices, pool.Count));
        }

        private void StartCombat()
        {
            var enemy = _config.Combat.Enemies.Get(_config.Fights[State.FightIndex]);

            var setup = new CombatSetup(
                State.MaxHealth,
                State.Health,
                State.Deck,
                enemy.Id,
                enemy.MaxHealth,
                State.CombatSeed);

            CurrentCombat = Combat.StartNew(setup, _config.Combat);
        }
    }
}