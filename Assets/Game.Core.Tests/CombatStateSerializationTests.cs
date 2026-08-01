using System.Collections.Generic;
using Game.Core.Serialization;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class CombatStateSerializationTests
    {
        private static CombatState MakeState()
        {
            var deck = new List<CardId> { new CardId("strike"), new CardId("defend"), new CardId("toss") };
            var state = CombatState.Create(new CombatSetup(50, 42, deck, "rat_bruiser", 24, 777UL));

            state.Player.SetEnergy(3);
            state.Player.Combatant.AddBlock(5);
            state.Player.AddToHand(state.Player.RemoveTopOfDrawPile());
            state.Player.AddToDiscard(new CardId("defend"));
            state.Enemy.Combatant.ReduceHealth(7);
            state.Enemy.SetIntentIndex(1);
            state.AdvanceTurnNumber();
            state.SetActiveSide(Side.Enemy);
            state.Player.Combatant.AddPoison(2);
            state.Enemy.Combatant.AddVulnerable(1);

            return state;
        }

        [Test]
        public void RoundTrip_ProducesIdenticalJson()
        {
            var json = CoreJson.Serialize(MakeState());

            var restoredJson = CoreJson.Serialize(CoreJson.Deserialize<CombatState>(json));

            Assert.AreEqual(json, restoredJson);
        }

        [Test]
        public void RoundTrip_KeepsPilesAndNumbers()
        {
            var original = MakeState();

            var restored = CoreJson.Deserialize<CombatState>(CoreJson.Serialize(original));

            CollectionAssert.AreEqual(original.Player.DrawPile, restored.Player.DrawPile);
            CollectionAssert.AreEqual(original.Player.Hand, restored.Player.Hand);
            CollectionAssert.AreEqual(original.Player.DiscardPile, restored.Player.DiscardPile);
            Assert.AreEqual(original.Player.Energy, restored.Player.Energy);
            Assert.AreEqual(original.Player.Combatant.Block, restored.Player.Combatant.Block);
            Assert.AreEqual(original.Enemy.Combatant.Health, restored.Enemy.Combatant.Health);
            Assert.AreEqual(original.Enemy.IntentIndex, restored.Enemy.IntentIndex);
            Assert.AreEqual(original.TurnNumber, restored.TurnNumber);
            Assert.AreEqual(original.ActiveSide, restored.ActiveSide);
            Assert.AreEqual(original.Seed, restored.Seed);
        }
    }
}