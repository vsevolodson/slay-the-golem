using System.Collections.Generic;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.Effects;
using Game.Core.State;
using NUnit.Framework;

namespace Game.Core.Tests
{
    public class RatBruiserTests
    {
        private static readonly CardId Strike = new CardId("strike");

        private static CombatConfig MakeConfig() => new CombatConfig(
            new CardCatalog(new[] { new CardDefinition(Strike, 1, new IEffect[] { new DamageEffect(6) }) }),
            TestContent.RatBruiser(),
            new CombatRules(energyPerTurn: 3, cardsDrawnPerTurn: 5));

        private static Combat StartCombat()
        {
            var deck = new List<CardId>();
            for (var i = 0; i < 10; i++) deck.Add(Strike);

            return Combat.StartNew(new CombatSetup(50, 50, deck, "rat_bruiser", 24, 7UL), MakeConfig());
        }

        [Test]
        public void FirstIntent_IsShownBeforeThePlayersFirstTurn()
        {
            var combat = StartCombat();

            Assert.AreEqual(8, combat.EnemyIntent.Damage);
            Assert.AreEqual(0, combat.EnemyIntent.Block);
            Assert.AreEqual(50, combat.State.Player.Combatant.Health);
        }

        [Test]
        public void Enemy_PerformsExactlyTheIntentItShowed()
        {
            var combat = StartCombat();
            var shown = combat.EnemyIntent;

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(50 - shown.Damage, combat.State.Player.Combatant.Health);
        }

        [Test]
        public void Intent_WalksThroughTheCycleAndRepeats()
        {
            var combat = StartCombat();

            combat.Execute(new EndTurnCommand());
            Assert.AreEqual(5, combat.EnemyIntent.Damage);
            Assert.AreEqual(5, combat.EnemyIntent.Block);

            combat.Execute(new EndTurnCommand());
            Assert.AreEqual(8, combat.EnemyIntent.Damage);
            Assert.AreEqual(0, combat.EnemyIntent.Block);
        }

        [Test]
        public void SecondIntent_AttacksAndGainsBlock()
        {
            var combat = StartCombat();

            combat.Execute(new EndTurnCommand());
            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(37, combat.State.Player.Combatant.Health);
            Assert.AreEqual(5, combat.State.Enemy.Combatant.Block);
        }

        [Test]
        public void EnemyBlock_AbsorbsThePlayersDamage()
        {
            var combat = StartCombat();
            combat.Execute(new EndTurnCommand());
            combat.Execute(new EndTurnCommand());

            combat.Execute(new PlayCardCommand(Strike));

            Assert.AreEqual(0, combat.State.Enemy.Combatant.Block);
            Assert.AreEqual(23, combat.State.Enemy.Combatant.Health);
        }

        [Test]
        public void EnemyBlock_IsClearedAtTheStartOfItsOwnTurn()
        {
            var combat = StartCombat();
            combat.Execute(new EndTurnCommand());
            combat.Execute(new EndTurnCommand());
            Assert.AreEqual(5, combat.State.Enemy.Combatant.Block);

            combat.Execute(new EndTurnCommand());

            Assert.AreEqual(0, combat.State.Enemy.Combatant.Block);
        }

        [Test]
        public void PassivePlayer_LosesOnTheEighthEnemyTurn()
        {
            var combat = StartCombat();
            var enemyTurns = 0;

            while (combat.State.Outcome == CombatOutcome.InProgress && enemyTurns < 50)
            {
                combat.Execute(new EndTurnCommand());
                enemyTurns++;
            }

            Assert.AreEqual(CombatOutcome.PlayerLost, combat.State.Outcome);
            Assert.AreEqual(8, enemyTurns);
            Assert.AreEqual(0, combat.State.Player.Combatant.Health);
        }

        [Test]
        public void FullCombat_PlayerWins()
        {
            var combat = StartCombat();

            for (var guard = 0; guard < 20 && combat.State.Outcome == CombatOutcome.InProgress; guard++)
            {
                foreach (var card in new List<CardId>(combat.State.Player.Hand))
                    combat.Execute(new PlayCardCommand(card));   // the engine rejects what cannot be played

                if (combat.State.Outcome != CombatOutcome.InProgress)
                    break;

                combat.Execute(new EndTurnCommand());
            }

            Assert.AreEqual(CombatOutcome.PlayerWon, combat.State.Outcome);
            Assert.AreEqual(0, combat.State.Enemy.Combatant.Health);
            Assert.Greater(combat.State.Player.Combatant.Health, 0);
        }
    }
}