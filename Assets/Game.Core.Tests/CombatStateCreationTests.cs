using System.Collections.Generic;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class CombatStateCreationTests
    {
        private static CombatSetup MakeSetup()
        {
            var deck = new List<CardId>();
            for (var i = 0; i < 5; i++) deck.Add(new CardId("strike"));
            for (var i = 0; i < 4; i++) deck.Add(new CardId("defend"));
            deck.Add(new CardId("toss"));

            return new CombatSetup(50, 50, deck, "rat_bruiser", 24, 12345UL);
        }

        [Test]
        public void Create_PutsWholeDeckIntoDrawPile()
        {
            var state = CombatState.Create(MakeSetup());

            Assert.AreEqual(10, state.Player.DrawPile.Count);
            Assert.AreEqual(0, state.Player.Hand.Count);
            Assert.AreEqual(0, state.Player.DiscardPile.Count);
        }

        [Test]
        public void Create_SetsStartingValues()
        {
            var state = CombatState.Create(MakeSetup());

            Assert.AreEqual(50, state.Player.Combatant.Health);
            Assert.AreEqual(24, state.Enemy.Combatant.Health);
            Assert.AreEqual(0, state.Player.Combatant.Block);
            Assert.AreEqual(0, state.Player.Energy);
            Assert.AreEqual(0, state.TurnNumber);
            Assert.AreEqual(CombatOutcome.InProgress, state.Outcome);
        }

        [Test]
        public void GetCombatant_ReturnsRequestedSide()
        {
            var state = CombatState.Create(MakeSetup());

            Assert.AreSame(state.Player.Combatant, state.GetCombatant(Side.Player));
            Assert.AreSame(state.Enemy.Combatant, state.GetCombatant(Side.Enemy));
        }

        [Test]
        public void Create_RejectsHealthAboveMax()
        {
            var deck = new List<CardId> { new CardId("strike") };

            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new CombatSetup(50, 51, deck, "rat_bruiser", 24, 1UL));
        }
    }
}