using System.Collections.Generic;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.Effects;
using Game.Core.Serialization;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class RelicTests
    {
        private const string RustyArmor = "rusty_armor";

        private static readonly CardId Strike = new CardId("atack");

        [Test]
        public void RustyArmor_GivesBlockOnTheFirstTurn()
        {
            var combat = StartCombat(RustyArmor);

            Assert.AreEqual(4, combat.State.Player.Combatant.Block);
            Assert.AreEqual(1, combat.State.TurnNumber);
        }

        [Test]
        public void WithoutRelic_ThePlayerStartsWithoutBlock()
        {
            var combat = StartCombat(null);

            Assert.AreEqual(0, combat.State.Player.Combatant.Block);
        }

        [Test]
        public void RustyArmor_BlockAbsorbsTheFirstEnemyAttack()
        {
            var combat = StartCombat(RustyArmor);

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(46, combat.State.Player.Combatant.Health);
        }

        [Test]
        public void RustyArmor_BlockIsGoneOnTheNextTurn()
        {
            var combat = StartCombat(RustyArmor);

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(2, combat.State.TurnNumber);
            Assert.AreEqual(0, combat.State.Player.Combatant.Block);
        }

        [Test]
        public void RustyArmor_WorksInEveryFightOfTheRun()
        {
            var run = WinFirstFight();

            run.Execute(new ChooseRewardCommand(run.State.RewardOffer[0]));

            Assert.AreEqual(1, run.State.FightIndex);
            Assert.AreEqual(1, run.CurrentCombat.State.TurnNumber);
            Assert.AreEqual(4, run.CurrentCombat.State.Player.Combatant.Block);
        }

        [Test]
        public void RelicId_IsStoredInTheRunState()
        {
            var run = Run.StartNew(MakeRunConfig(), seed: 1UL);

            Assert.AreEqual(RustyArmor, run.State.RelicId);
        }

        [Test]
        public void RelicId_SurvivesSaveAndLoad()
        {
            var run = Run.StartNew(MakeRunConfig(), seed: 1UL);
            var json = RunSaveSerializer.Save(run.State);

            Assert.IsTrue(RunSaveSerializer.TryLoad(json, out var state));
            Assert.AreEqual(RustyArmor, state.RelicId);

            Assert.IsTrue(Run.TryResume(state, MakeRunConfig(), out var restored));
            Assert.AreEqual(4, restored.CurrentCombat.State.Player.Combatant.Block);
        }

        [Test]
        public void Resume_RejectsSaveWithAnUnknownRelic()
        {
            var run = Run.StartNew(MakeRunConfig(), seed: 1UL);

            Assert.IsFalse(Run.TryResume(run.State, MakeRunConfigWithoutRelics(), out _));
        }

        private static Run WinFirstFight()
        {
            var run = Run.StartNew(MakeRunConfig(), seed: 1UL);

            for (var guard = 0; guard < 20 && run.State.Phase == RunPhase.InCombat; guard++)
            {
                foreach (var card in new List<CardId>(run.CurrentCombat.State.Player.Hand))
                    run.Execute(new PlayCardCommand(card));

                if (run.State.Phase != RunPhase.InCombat)
                    break;

                run.Execute(new EndTurnCommand());
            }

            return run;
        }

        private static Combat StartCombat(string relicId) => Combat.StartNew(
            new CombatSetup(50, 50, Deck(10), TestContent.RatBruiserId, TestContent.RatBruiserHealth,
                seed: 1UL, relicId: relicId),
            MakeCombatConfig());

        private static RunConfig MakeRunConfig() => new RunConfig(
            MakeCombatConfig(),
            new[] { TestContent.RatBruiserId, TestContent.RatBruiserId },
            new[] { Strike },
            Deck(10),
            playerMaxHealth: 50,
            rewardChoices: 1,
            relicId: RustyArmor);

        private static RunConfig MakeRunConfigWithoutRelics() => new RunConfig(
            new CombatConfig(MakeCardCatalog(), TestContent.RatBruiser(), MakeRules()),
            new[] { TestContent.RatBruiserId, TestContent.RatBruiserId },
            new[] { Strike },
            Deck(10),
            playerMaxHealth: 50,
            rewardChoices: 1);

        private static CombatConfig MakeCombatConfig() => new CombatConfig(
            MakeCardCatalog(),
            TestContent.RatBruiser(),
            MakeRules(),
            new RelicCatalog(new[]
            {
                new RelicDefinition(RustyArmor, new IEffect[] { new BlockEffect(4) })
            }));

        private static CardCatalog MakeCardCatalog() => new CardCatalog(new[]
        {
            new CardDefinition(Strike, 1, new IEffect[] { new DamageEffect(6) })
        });

        private static CombatRules MakeRules() => new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5);

        private static List<CardId> Deck(int count)
        {
            var deck = new List<CardId>();
            for (var i = 0; i < count; i++) deck.Add(Strike);

            return deck;
        }
    }
}