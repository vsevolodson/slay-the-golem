using System.Collections.Generic;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.Effects;
using Game.Core.Rng;
using Game.Core.Serialization;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class RandomnessTests
    {
        private static readonly CardId Strike = new CardId("strike");
        private static readonly CardId Defend = new CardId("defend");

        [Test]
        public void SameSeed_ProducesSameSequence()
        {
            var first = new XorShiftRng(12345UL);
            var second = new XorShiftRng(12345UL);

            for (var i = 0; i < 20; i++)
                Assert.AreEqual(first.Next(1000), second.Next(1000));
        }

        [Test]
        public void DifferentSeeds_ProduceDifferentSequences()
        {
            var first = new XorShiftRng(1UL);
            var second = new XorShiftRng(2UL);

            var same = true;
            for (var i = 0; i < 20; i++)
            {
                if (first.Next(1000) != second.Next(1000))
                    same = false;
            }

            Assert.IsFalse(same);
        }

        [Test]
        public void ZeroSeed_StillProducesVaryingNumbers()
        {
            var rng = new XorShiftRng(0UL);

            var first = rng.Next(1000);
            var second = rng.Next(1000);
            var third = rng.Next(1000);

            Assert.IsFalse(first == 0 && second == 0 && third == 0);
        }

        [Test]
        public void Shuffle_IsDeterministicForTheSameSeed()
        {
            var first = MakeDeck();
            var second = MakeDeck();

            ShuffleRules.Shuffle(first, new XorShiftRng(777UL));
            ShuffleRules.Shuffle(second, new XorShiftRng(777UL));

            CollectionAssert.AreEqual(first, second);
        }

        [Test]
        public void Shuffle_KeepsEveryCard()
        {
            var deck = MakeDeck();
            var before = new List<CardId>(deck);

            ShuffleRules.Shuffle(deck, new XorShiftRng(777UL));

            CollectionAssert.AreEquivalent(before, deck);
        }

        [Test]
        public void SameSeed_SameCommands_ProduceIdenticalState()
        {
            var first = PlayScriptedCombat(seed: 42UL);
            var second = PlayScriptedCombat(seed: 42UL);

            Assert.AreEqual(CoreJson.Serialize(first), CoreJson.Serialize(second));
        }

        [Test]
        public void DifferentSeed_ProducesDifferentDeckOrder()
        {
            var first = PlayScriptedCombat(seed: 42UL);
            var second = PlayScriptedCombat(seed: 43UL);

            Assert.AreNotEqual(CoreJson.Serialize(first), CoreJson.Serialize(second));
        }

        [Test]
        public void Rng_ContinuesTheSameSequenceAfterRoundTrip()
        {
            var original = CombatState.Create(new CombatSetup(50, 50, MakeDeck(), "rat_bruiser", 24, 99UL));
            original.Rng.Next(500);

            var restored = CoreJson.Deserialize<CombatState>(CoreJson.Serialize(original));

            for (var i = 0; i < 10; i++)
                Assert.AreEqual(original.Rng.Next(500), restored.Rng.Next(500));
        }

        private static List<CardId> MakeDeck()
        {
            var deck = new List<CardId>();
            for (var i = 0; i < 5; i++) deck.Add(Strike);
            for (var i = 0; i < 4; i++) deck.Add(Defend);

            return deck;
        }

        private static CombatState PlayScriptedCombat(ulong seed)
        {
            var config = new CombatConfig(
                new CardCatalog(new[]
                {
                    new CardDefinition(Strike, 1, new IEffect[] { new DamageEffect(6) }),
                    new CardDefinition(Defend, 1, new IEffect[] { new BlockEffect(5) })
                }),
                TestContent.RatBruiser(),
                new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5));

            var combat = Combat.StartNew(new CombatSetup(50, 50, MakeDeck(), "rat_bruiser", 24, seed), config);

            for (var turn = 0; turn < 3; turn++)
            {
                foreach (var card in new List<CardId>(combat.State.Player.Hand))
                    combat.Execute(new PlayCardCommand(card));

                combat.Execute(new EndTurnCommand());
            }

            return combat.State;
        }
    }
}