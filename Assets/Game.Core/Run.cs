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

        public static Run Resume(RunState state, RunConfig config)
        {
            var run = new Run(state, config);

            if (run.State.Phase == RunPhase.InCombat)
                run.StartCombat();

            return run;
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