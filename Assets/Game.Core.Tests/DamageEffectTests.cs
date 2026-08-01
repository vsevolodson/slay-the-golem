using System.Collections.Generic;
using Game.Core.Effects;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class DamageEffectTests
    {
        private static CombatState MakeState()
        {
            var deck = new List<CardId> { new CardId("strike") };
            return CombatState.Create(new CombatSetup(50, 50, deck, "rat_bruiser", 24, 1UL));
        }

        [TestCase(6, 0, 18, 0)]
        [TestCase(8, 5, 21, 0)]
        [TestCase(4, 10, 24, 6)]
        public void Damage_IsAbsorbedByBlockFirst(int damage, int block, int expectedHealth, int expectedBlock)
        {
            var state = MakeState();
            state.Enemy.Combatant.AddBlock(block);

            new DamageEffect(damage).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(expectedHealth, state.Enemy.Combatant.Health);
            Assert.AreEqual(expectedBlock, state.Enemy.Combatant.Block);
        }

        [Test]
        public void Damage_FromTheEnemy_HitsThePlayer()
        {
            var state = MakeState();

            new DamageEffect(8).Apply(new EffectContext(state, Side.Enemy));

            Assert.AreEqual(42, state.Player.Combatant.Health);
            Assert.AreEqual(24, state.Enemy.Combatant.Health);
        }

        [Test]
        public void Block_IsGivenToTheActor()
        {
            var state = MakeState();

            new BlockEffect(5).Apply(new EffectContext(state, Side.Enemy));

            Assert.AreEqual(5, state.Enemy.Combatant.Block);
            Assert.AreEqual(0, state.Player.Combatant.Block);
        }

        [Test]
        public void EffectSystem_EndsCombatWhenTheEnemyDies()
        {
            var state = MakeState();
            var effects = new EffectSystem(state);

            effects.Apply(new DamageEffect(100), Side.Player);

            Assert.AreEqual(0, state.Enemy.Combatant.Health);
            Assert.AreEqual(CombatOutcome.PlayerWon, state.Outcome);
        }

        [Test]
        public void EffectSystem_StopsApplyingEffectsAfterTheCombatEnds()
        {
            var state = MakeState();
            var effects = new EffectSystem(state);

            effects.Apply(new IEffect[] { new DamageEffect(100), new BlockEffect(5) }, Side.Player);

            Assert.AreEqual(CombatOutcome.PlayerWon, state.Outcome);
            Assert.AreEqual(0, state.Player.Combatant.Block);
        }
    }
}