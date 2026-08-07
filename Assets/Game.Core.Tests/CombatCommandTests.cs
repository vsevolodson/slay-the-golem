using System;
using System.Collections.Generic;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.State;
using Game.Core.Effects;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class CombatCommandTests
    {
        private static readonly CardId Strike = new CardId("strike");
        private static readonly CardId Defend = new CardId("defend");

        private static CombatConfig MakeConfig() => new CombatConfig(
            new CardCatalog(new[] { new CardDefinition(Strike, 1, new IEffect[] { new BlockEffect(0) }), new CardDefinition(Defend, 1, new IEffect[] { new DamageEffect(0)}) }),
            TestContent.RatBruiser(),
            new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5));

        private static CombatSetup MakeSetup(IReadOnlyList<CardId> deck) =>
            new CombatSetup(50, 50, deck, "rat_bruiser", 24, 1UL);

        private static Combat StartCombat(int deckSize = 10)
        {
            var deck = new List<CardId>();
            for (var i = 0; i < deckSize; i++) deck.Add(Strike);

            return Combat.StartNew(MakeSetup(deck), MakeConfig());
        }

        [Test]
        public void StartNew_BeginsFirstPlayerTurn()
        {
            var combat = StartCombat();

            Assert.AreEqual(1, combat.State.TurnNumber);
            Assert.AreEqual(5, combat.State.Player.Hand.Count);
            Assert.AreEqual(5, combat.State.Player.DrawPile.Count);
            Assert.AreEqual(3, combat.State.Player.Energy);
            Assert.AreEqual(Side.Player, combat.State.ActiveSide);
        }

        [Test]
        public void StartNew_RejectsDeckWithCardMissingFromCatalog()
        {
            var deck = new List<CardId> { new CardId("wrong") };

            Assert.Throws<ArgumentException>(() => Combat.StartNew(MakeSetup(deck), MakeConfig()));
        }

        [Test]
        public void PlayCard_SpendsEnergyAndDiscardsTheCard()
        {
            var combat = StartCombat();

            var result = combat.Execute(new PlayCardCommand(Strike));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, combat.State.Player.Energy);
            Assert.AreEqual(4, combat.State.Player.Hand.Count);
            CollectionAssert.AreEqual(new[] { Strike }, combat.State.Player.DiscardPile);
        }

        [Test]
        public void PlayCard_WithoutEnergy_IsRejectedAndChangesNothing()
        {
            var combat = StartCombat();
            for (var i = 0; i < 3; i++) combat.Execute(new PlayCardCommand(Strike));

            var handBefore = combat.State.Player.Hand.Count;
            var result = combat.Execute(new PlayCardCommand(Strike));

            Assert.AreEqual(CommandRejection.NotEnoughEnergy, result.Rejection);
            Assert.AreEqual(0, combat.State.Player.Energy);
            Assert.AreEqual(handBefore, combat.State.Player.Hand.Count);
            Assert.AreEqual(3, combat.State.Player.DiscardPile.Count);
        }

        [Test]
        public void PlayCard_NotInHand_IsRejected()
        {
            var combat = StartCombat();

            var result = combat.Execute(new PlayCardCommand(Defend));

            Assert.AreEqual(CommandRejection.CardNotInHand, result.Rejection);
            Assert.AreEqual(3, combat.State.Player.Energy);
        }

        [Test]
        public void PlayCard_WhenItIsNotThePlayersTurn_IsRejected()
        {
            var combat = StartCombat();
            combat.State.SetActiveSide(Side.Enemy);

            var result = combat.Execute(new PlayCardCommand(Strike));

            Assert.AreEqual(CommandRejection.NotYourTurn, result.Rejection);
            Assert.AreEqual(5, combat.State.Player.Hand.Count);
        }

        [Test]
        public void Validate_DoesNotChangeAnything()
        {
            var combat = StartCombat();

            var result = combat.Validate(new PlayCardCommand(Strike));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(3, combat.State.Player.Energy);
            Assert.AreEqual(5, combat.State.Player.Hand.Count);
        }

        [Test]
        public void EndTurn_DiscardsHandAndStartsTheNextTurn()
        {
            var combat = StartCombat();
            combat.Execute(new PlayCardCommand(Strike));

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(2, combat.State.TurnNumber);
            Assert.AreEqual(5, combat.State.Player.Hand.Count);
            Assert.AreEqual(3, combat.State.Player.Energy);
            Assert.AreEqual(Side.Player, combat.State.ActiveSide);
        }

        [Test]
        public void BeginTurn_ClearsBlockOfBothSides()
        {
            var combat = StartCombat();
            combat.State.Player.Combatant.AddBlock(8);
            combat.State.Enemy.Combatant.AddBlock(5);

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(0, combat.State.Player.Combatant.Block);
            Assert.AreEqual(0, combat.State.Enemy.Combatant.Block);
        }

        [Test]
        public void Draw_RefillsDrawPileFromDiscard()
        {
            var combat = StartCombat(deckSize: 6);

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(5, combat.State.Player.Hand.Count);
            Assert.AreEqual(1, combat.State.Player.DrawPile.Count);
            Assert.AreEqual(0, combat.State.Player.DiscardPile.Count);
        }

        [Test]
        public void Draw_WhenBothPilesAreEmpty_TakesWhatIsThere()
        {
            var combat = StartCombat(deckSize: 3);

            Assert.AreEqual(3, combat.State.Player.Hand.Count);
            Assert.AreEqual(0, combat.State.Player.DrawPile.Count);
        }
    }
}