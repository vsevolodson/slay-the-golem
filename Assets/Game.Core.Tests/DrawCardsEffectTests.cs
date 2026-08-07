using System.Collections.Generic;
using Game.Core.Effects;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class DrawCardsEffectTests
    {
        private static readonly CardId Strike = new CardId("strike");
        private static readonly CardId Defend = new CardId("defend");

        [Test]
        public void DrawCards_TakesFromTheDrawPile()
        {
            var state = MakeState(drawPileSize: 3);

            new DrawCardsEffect(2).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(2, state.Player.Hand.Count);
            Assert.AreEqual(1, state.Player.DrawPile.Count);
        }

        [Test]
        public void DrawCards_RefillsTheDrawPileFromTheDiscard()
        {
            var state = MakeState(drawPileSize: 1);
            state.Player.AddToDiscard(Defend);
            state.Player.AddToDiscard(Defend);

            new DrawCardsEffect(2).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(2, state.Player.Hand.Count);
            Assert.AreEqual(1, state.Player.DrawPile.Count);
            Assert.AreEqual(0, state.Player.DiscardPile.Count);
        }

        [Test]
        public void DrawCards_WhenBothPilesAreEmpty_TakesWhatIsThere()
        {
            var state = MakeState(drawPileSize: 1);

            new DrawCardsEffect(2).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(1, state.Player.Hand.Count);
            Assert.AreEqual(0, state.Player.DrawPile.Count);
        }

        private static CombatState MakeState(int drawPileSize)
        {
            var deck = new List<CardId>();
            for (var i = 0; i < drawPileSize; i++) deck.Add(Strike);

            return CombatState.Create(new CombatSetup(50, 50, deck,
                TestContent.RatBruiserId, TestContent.RatBruiserHealth, seed: 1UL));
        }
    }
}
