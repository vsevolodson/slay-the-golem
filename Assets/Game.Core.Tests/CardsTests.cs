using System.Collections.Generic;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.Effects;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class CardsTests
    {
        private static readonly CardId Strike = new CardId("atack");
        private static readonly CardId Heavy = new CardId("heavy");
        private static readonly CardId Injection = new CardId("injection");
        private static readonly CardId Wall = new CardId("wall");
        private static readonly CardId PoisonCloud = new CardId("poison");
        private static readonly CardId Breath = new CardId("breath");

        [Test]
        public void Heavy_DealsFourteenDamageForTwoEnergy()
        {
            var combat = StartCombat(Heavy);

            var result = combat.Execute(new PlayCardCommand(Heavy));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(TestContent.RatBruiserHealth - 14, combat.State.Enemy.Combatant.Health);
            Assert.AreEqual(1, combat.State.Player.Energy);
        }

        [Test]
        public void Injection_DealsDamageAndAppliesPoison()
        {
            var combat = StartCombat(Injection);

            combat.Execute(new PlayCardCommand(Injection));

            Assert.AreEqual(TestContent.RatBruiserHealth - 4, combat.State.Enemy.Combatant.Health);
            Assert.AreEqual(3, combat.State.Enemy.Combatant.Poison);
            Assert.AreEqual(2, combat.State.Player.Energy);
        }

        [Test]
        public void Injection_AppliesPoisonThroughBlock()
        {
            var combat = StartCombat(Injection);
            combat.State.Enemy.Combatant.AddBlock(10);

            combat.Execute(new PlayCardCommand(Injection));

            Assert.AreEqual(TestContent.RatBruiserHealth, combat.State.Enemy.Combatant.Health);
            Assert.AreEqual(6, combat.State.Enemy.Combatant.Block);
            Assert.AreEqual(3, combat.State.Enemy.Combatant.Poison);
        }

        [Test]
        public void PoisonCloud_AppliesPoisonWithoutDamage()
        {
            var combat = StartCombat(PoisonCloud);

            combat.Execute(new PlayCardCommand(PoisonCloud));

            Assert.AreEqual(TestContent.RatBruiserHealth, combat.State.Enemy.Combatant.Health);
            Assert.AreEqual(2, combat.State.Enemy.Combatant.Poison);
            Assert.AreEqual(2, combat.State.Player.Energy);
        }

        [Test]
        public void Wall_GivesEightBlockToThePlayer()
        {
            var combat = StartCombat(Wall);

            combat.Execute(new PlayCardCommand(Wall));

            Assert.AreEqual(8, combat.State.Player.Combatant.Block);
            Assert.AreEqual(0, combat.State.Enemy.Combatant.Block);
            Assert.AreEqual(2, combat.State.Player.Energy);
        }

        [Test]
        public void Breath_CostsNothingAndDrawsFromTheDiscard()
        {
            var combat = StartCombat(Breath, Strike, Strike, Strike, Strike);
            combat.Execute(new PlayCardCommand(Strike));

            var result = combat.Execute(new PlayCardCommand(Breath));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, combat.State.Player.Energy);
            Assert.AreEqual(4, combat.State.Player.Hand.Count);
            Assert.AreEqual(0, combat.State.Player.DrawPile.Count);
            CollectionAssert.AreEqual(new[] { Breath }, combat.State.Player.DiscardPile);
        }

        [Test]
        public void Breath_IsPlayableWithoutEnergy()
        {
            var combat = StartCombat(Breath, Strike, Strike, Strike);
            for (var i = 0; i < 3; i++) combat.Execute(new PlayCardCommand(Strike));

            var result = combat.Execute(new PlayCardCommand(Breath));

            Assert.AreEqual(0, combat.State.Player.Energy);
            Assert.IsTrue(result.IsSuccess);
        }

        private static Combat StartCombat(params CardId[] deck) => Combat.StartNew(
            new CombatSetup(50, 50, new List<CardId>(deck),
                TestContent.RatBruiserId, TestContent.RatBruiserHealth, seed: 1UL),
            MakeConfig());

        private static CombatConfig MakeConfig() => new CombatConfig(
            new CardCatalog(new[]
            {
                new CardDefinition(Strike, 1, new IEffect[] { new DamageEffect(6) }),
                new CardDefinition(Heavy, 2, new IEffect[] { new DamageEffect(14) }),
                new CardDefinition(Injection, 1, new IEffect[] { new DamageEffect(4), new ApplyPoisonEffect(3) }),
                new CardDefinition(Wall, 1, new IEffect[] { new BlockEffect(8) }),
                new CardDefinition(PoisonCloud, 1, new IEffect[] { new ApplyPoisonEffect(2) }),
                new CardDefinition(Breath, 0, new IEffect[] { new DrawCardsEffect(2) })
            }),
            TestContent.RatBruiser(),
            new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5));
    }
}
