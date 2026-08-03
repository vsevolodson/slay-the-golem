using System.Collections.Generic;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.Effects;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class CardPlayTests
    {
        private static readonly CardId Strike = new CardId("strike");
        private static readonly CardId Defend = new CardId("defend");

        private static Combat StartCombat()
        {
            var config = new CombatConfig(
                new CardCatalog(new[]
                {
                    new CardDefinition(Strike, 1, new IEffect[] { new DamageEffect(6) }),
                    new CardDefinition(Defend, 1, new IEffect[] { new BlockEffect(5) })
                }),
                TestContent.RatBruiser(),
                new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5));

            var deck = new List<CardId> { Strike, Strike, Strike, Defend, Defend };

            return Combat.StartNew(new CombatSetup(50, 50, deck, "rat_bruiser", 24, 1UL), config);
        }

        [Test]
        public void Strike_DealsSixDamageToTheEnemy()
        {
            var combat = StartCombat();

            var result = combat.Execute(new PlayCardCommand(Strike));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(18, combat.State.Enemy.Combatant.Health);
        }

        [Test]
        public void Defend_GivesFiveBlockToThePlayer()
        {
            var combat = StartCombat();

            combat.Execute(new PlayCardCommand(Defend));

            Assert.AreEqual(5, combat.State.Player.Combatant.Block);
            Assert.AreEqual(24, combat.State.Enemy.Combatant.Health);
        }

        [Test]
        public void RejectedCard_AppliesNoEffects()
        {
            var combat = StartCombat();
            for (var i = 0; i < 3; i++) combat.Execute(new PlayCardCommand(Strike));
            var healthAfterThree = combat.State.Enemy.Combatant.Health;

            var result = combat.Execute(new PlayCardCommand(Defend));

            Assert.AreEqual(CommandRejection.NotEnoughEnergy, result.Rejection);
            Assert.AreEqual(healthAfterThree, combat.State.Enemy.Combatant.Health);
            Assert.AreEqual(0, combat.State.Player.Combatant.Block);
        }
    }
}