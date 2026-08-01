using System.Collections.Generic;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.Effects;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class VulnerableTests
    {
        private static readonly CardId Strike = new CardId("strike");
        private static readonly CardId Bash = new CardId("bash");

        private static CombatState MakeState()
        {
            var deck = new List<CardId> { Strike };
            return CombatState.Create(new CombatSetup(50, 50, deck, "rat_bruiser", 24, 1UL));
        }

        [TestCase(6, 9)]
        [TestCase(8, 12)]
        [TestCase(7, 10)]
        [TestCase(5, 7)]
        public void Damage_AgainstVulnerable_IsMultipliedAndFloored(int baseDamage, int expected)
        {
            var state = MakeState();
            state.Enemy.Combatant.AddVulnerable(1);

            new DamageEffect(baseDamage).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(24 - expected, state.Enemy.Combatant.Health);
        }

        [Test]
        public void Multiplier_AppliesBeforeBlockAbsorbs()
        {
            var state = MakeState();
            state.Enemy.Combatant.AddVulnerable(1);
            state.Enemy.Combatant.AddBlock(5);

            new DamageEffect(6).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(0, state.Enemy.Combatant.Block);
            Assert.AreEqual(20, state.Enemy.Combatant.Health);
        }

        [Test]
        public void ApplyVulnerable_Stacks()
        {
            var state = MakeState();

            new ApplyVulnerableEffect(2).Apply(new EffectContext(state, Side.Player));
            new ApplyVulnerableEffect(2).Apply(new EffectContext(state, Side.Player));

            Assert.AreEqual(4, state.Enemy.Combatant.Vulnerable);
        }

        [Test]
        public void Bash_DoesNotBoostItself_ButBoostsTheNextAttack()
        {
            var combat = StartCombat();

            combat.Execute(new PlayCardCommand(Bash));
            Assert.AreEqual(16, combat.State.Enemy.Combatant.Health);

            combat.Execute(new PlayCardCommand(Strike));
            Assert.AreEqual(7, combat.State.Enemy.Combatant.Health);
        }

        [Test]
        public void Vulnerable_FadesAtTheStartOfTheOwnersTurn()
        {
            var combat = StartCombat();
            combat.Execute(new PlayCardCommand(Bash));
            Assert.AreEqual(2, combat.State.Enemy.Combatant.Vulnerable);

            combat.Execute(new EndTurnCommand());
            Assert.AreEqual(1, combat.State.Enemy.Combatant.Vulnerable);

            combat.Execute(new EndTurnCommand());
            Assert.AreEqual(0, combat.State.Enemy.Combatant.Vulnerable);
        }

        private static Combat StartCombat()
        {
            var config = new CombatConfig(
                new CardCatalog(new[]
                {
                    new CardDefinition(Strike, 1, new IEffect[] { new DamageEffect(6) }),
                    new CardDefinition(Bash, 2, new IEffect[] { new DamageEffect(8), new ApplyVulnerableEffect(2) })
                }),
                new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5));

            var deck = new List<CardId> { Strike, Bash, Strike, Bash, Strike };

            return Combat.StartNew(new CombatSetup(50, 50, deck, "rat_bruiser", 24, 1UL), config);
        }
    }
}