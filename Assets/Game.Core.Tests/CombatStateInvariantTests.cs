using System;
using System.Collections.Generic;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class CombatStateInvariantTests
    {
        private static CombatState MakeState()
        {
            var deck = new List<CardId> { new CardId("strike"), new CardId("defend"), new CardId("toss") };
            return CombatState.Create(new CombatSetup(50, 50, deck, "rat_bruiser", 24, 1UL));
        }

        [Test]
        public void Health_NeverDropsBelowZero()
        {
            var state = MakeState();

            state.Enemy.Combatant.ReduceHealth(100);

            Assert.AreEqual(0, state.Enemy.Combatant.Health);
            Assert.IsTrue(state.Enemy.Combatant.IsDead);
        }

        [Test]
        public void Block_Accumulates_AndNeverDropsBelowZero()
        {
            var state = MakeState();

            state.Player.Combatant.AddBlock(5);
            state.Player.Combatant.AddBlock(8);
            Assert.AreEqual(13, state.Player.Combatant.Block);

            state.Player.Combatant.ReduceBlock(100);
            Assert.AreEqual(0, state.Player.Combatant.Block);
        }

        [Test]
        public void ClearBlock_LeavesNothingForTheNextTurn()
        {
            var state = MakeState();
            state.Player.Combatant.AddBlock(8);

            state.Player.Combatant.ClearBlock();

            Assert.AreEqual(0, state.Player.Combatant.Block);
        }

        [Test]
        public void DrawnCardLeavesDrawPileAndEntersHand()
        {
            var state = MakeState();

            var card = state.Player.RemoveTopOfDrawPile();
            state.Player.AddToHand(card);

            Assert.AreEqual(new CardId("toss"), card);
            Assert.AreEqual(2, state.Player.DrawPile.Count);
            CollectionAssert.AreEqual(new[] { card }, state.Player.Hand);
        }

        [Test]
        public void RemoveTopOfDrawPile_ThrowsWhenPileIsEmpty()
        {
            var state = MakeState();
            for (var i = 0; i < 3; i++) state.Player.RemoveTopOfDrawPile();

            Assert.Throws<InvalidOperationException>(() => state.Player.RemoveTopOfDrawPile());
        }

        [Test]
        public void RemoveFromHand_ThrowsWhenCardIsNotThere()
        {
            var state = MakeState();

            Assert.Throws<InvalidOperationException>(() => state.Player.RemoveFromHand(new CardId("strike")));
        }

        [Test]
        public void SpendEnergy_ThrowsAndKeepsEnergyIntact()
        {
            var state = MakeState();
            state.Player.SetEnergy(1);

            Assert.Throws<InvalidOperationException>(() => state.Player.SpendEnergy(2));
            Assert.AreEqual(1, state.Player.Energy);
        }
    }
}