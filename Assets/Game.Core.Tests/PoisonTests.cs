using System.Collections.Generic;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.Effects;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class PoisonTests
    {
        private static readonly CardId Strike = new CardId("strike");

        private static CombatState MakeState()
        {
            var deck = new List<CardId> { Strike };
            return CombatState.Create(new CombatSetup(50, 50, deck, "rat_bruiser", 24, 1UL));
        }

        [Test]
        public void ApplyPoison_Stacks()
        {
            var state = MakeState();

            new ApplyPoisonEffect(3).Apply(new EffectContext(state, Side.Player));
            new ApplyPoisonEffect(2).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(5, state.Enemy.Combatant.Poison);
        }

        [Test]
        public void ApplyPoison_IgnoresBlock()
        {
            var state = MakeState();
            state.Enemy.Combatant.AddBlock(10);

            new ApplyPoisonEffect(3).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(3, state.Enemy.Combatant.Poison);
            Assert.AreEqual(10, state.Enemy.Combatant.Block);
            Assert.AreEqual(24, state.Enemy.Combatant.Health);
        }

        [Test]
        public void PoisonTick_HitsHealthPastBlock_AndFadesByOne()
        {
            var state = MakeState();
            state.Enemy.Combatant.AddPoison(3);
            state.Enemy.Combatant.AddBlock(10);
            var effects = new EffectSystem(state);

            effects.Apply(new PoisonTickEffect(), Side.Enemy);

            Assert.AreEqual(21, state.Enemy.Combatant.Health);
            Assert.AreEqual(10, state.Enemy.Combatant.Block);
            Assert.AreEqual(2, state.Enemy.Combatant.Poison);
        }

        [Test]
        public void PoisonTick_WithoutPoison_DoesNothing()
        {
            var state = MakeState();
            var effects = new EffectSystem(state);

            effects.Apply(new PoisonTickEffect(), Side.Enemy);

            Assert.AreEqual(24, state.Enemy.Combatant.Health);
            Assert.AreEqual(0, state.Enemy.Combatant.Poison);
        }

        [Test]
        public void EnemyDyingFromPoison_EndsCombat_BeforeItsAction()
        {
            var combat = StartCombat();
            combat.State.Enemy.Combatant.ReduceHealth(23);
            combat.State.Enemy.Combatant.AddPoison(3);

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(CombatOutcome.PlayerWon, combat.State.Outcome);
            Assert.AreEqual(1, combat.State.TurnNumber);
        }

        [Test]
        public void PlayerDyingFromPoison_GetsNoTurn()
        {
            var combat = StartCombat();
            combat.State.Player.Combatant.ReduceHealth(40);
            combat.State.Player.Combatant.AddPoison(5);

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(CombatOutcome.PlayerLost, combat.State.Outcome);
            Assert.AreEqual(0, combat.State.Player.Hand.Count);
            Assert.AreEqual(0, combat.State.Player.Energy);
        }

        private static Combat StartCombat()
        {
            var config = new CombatConfig(
                new CardCatalog(new[]
                {
                    new CardDefinition(Strike, 1, new IEffect[] { new DamageEffect(6) })
                }),
                TestContent.RatBruiser(),
                new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5));

            var deck = new List<CardId> { Strike, Strike, Strike, Strike, Strike };

            return Combat.StartNew(new CombatSetup(50, 50, deck, "rat_bruiser", 24, 1UL), config);
        }
    }
}