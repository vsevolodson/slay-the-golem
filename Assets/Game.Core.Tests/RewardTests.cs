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
    public class RewardTests
    {
        private static readonly CardId Strike = new CardId("strike");

        private static readonly CardId[] RewardPool =
        {
            new CardId("wall"),
            new CardId("injection"),
            new CardId("poison"),
            new CardId("heavy"),
            new CardId("breath")
        };

        [Test]
        public void Reward_OffersThreeDistinctCardsFromThePool()
        {
            var run = WinFirstFight(seed: 1UL);

            Assert.AreEqual(RunPhase.ChoosingReward, run.State.Phase);
            Assert.AreEqual(3, run.State.RewardOffer.Count);
            CollectionAssert.AllItemsAreUnique(run.State.RewardOffer);

            foreach (var card in run.State.RewardOffer)
                CollectionAssert.Contains(RewardPool, card);
        }

        [Test]
        public void Reward_IsTheSameForTheSameSeed()
        {
            var first = WinFirstFight(seed: 42UL);
            var second = WinFirstFight(seed: 42UL);

            CollectionAssert.AreEqual(first.State.RewardOffer, second.State.RewardOffer);
        }

        [Test]
        public void Reward_SurvivesSaveAndLoad()
        {
            var run = WinFirstFight(seed: 7UL);
            var json = RunSaveSerializer.Save(run.State);

            Assert.IsTrue(RunSaveSerializer.TryLoad(json, out var state));
            Assert.IsTrue(Run.TryResume(state, MakeConfig(), out var restored));

            Assert.AreEqual(RunPhase.ChoosingReward, restored.State.Phase);
            CollectionAssert.AreEqual(run.State.RewardOffer, restored.State.RewardOffer);
        }

        [Test]
        public void Loading_DoesNotChangeTheSavedState()
        {
            var run = WinFirstFight(seed: 7UL);
            var json = RunSaveSerializer.Save(run.State);

            RunSaveSerializer.TryLoad(json, out var state);
            Run.TryResume(state, MakeConfig(), out var restored);

            Assert.AreEqual(json, RunSaveSerializer.Save(restored.State));
        }

        [Test]
        public void ChosenCard_GoesIntoTheDeck()
        {
            var run = WinFirstFight(seed: 1UL);
            var chosen = run.State.RewardOffer[0];

            var result = run.Execute(new ChooseRewardCommand(chosen));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(11, run.State.Deck.Count);
            CollectionAssert.Contains(run.State.Deck, chosen);
            Assert.AreEqual(0, run.State.RewardOffer.Count);
        }

        private static Run WinFirstFight(ulong seed)
        {
            var run = Run.StartNew(MakeConfig(), seed);

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

        private static RunConfig MakeConfig()
        {
            var cards = new List<CardDefinition>
            {
                new CardDefinition(Strike, 1, new IEffect[] { new DamageEffect(6) })
            };

            foreach (var card in RewardPool)
                cards.Add(new CardDefinition(card, 1, new IEffect[] { new BlockEffect(1) }));

            var combat = new CombatConfig(
                new CardCatalog(cards),
                TestContent.RatBruiser(),
                new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5));

            var deck = new List<CardId>();
            for (var i = 0; i < 10; i++) deck.Add(Strike);

            return new RunConfig(
                combat,
                new[] { TestContent.RatBruiserId, TestContent.RatBruiserId },
                RewardPool,
                deck,
                playerMaxHealth: 50,
                rewardChoices: 3);
        }
    }
}
