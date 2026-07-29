using System;
using Newtonsoft.Json;

namespace Game.Core.State
{
    public sealed class CombatState
    {
        [JsonProperty] public PlayerSide Player { get; private set; }
        [JsonProperty] public EnemySide Enemy { get; private set; }

        [JsonProperty] public int TurnNumber { get; private set; }
        [JsonProperty] public Side ActiveSide { get; private set; }
        [JsonProperty] public CombatOutcome Outcome { get; private set; }

        [JsonProperty] public ulong Seed { get; private set; }

        private CombatState(PlayerSide player, EnemySide enemy, ulong seed)
        {
            Player = player;
            Enemy = enemy;
            Seed = seed;
            TurnNumber = 0;
            ActiveSide = Side.Player;
            Outcome = CombatOutcome.InProgress;
        }

        [JsonConstructor]
        private CombatState()
        {
        }

        public static CombatState Create(CombatSetup setup)
        {
            if (setup == null)
                throw new ArgumentNullException(nameof(setup));

            var player = new PlayerSide(
                new Combatant(setup.PlayerMaxHealth, setup.PlayerHealth),
                setup.StartingDeck);

            var enemy = new EnemySide(
                new Combatant(setup.EnemyMaxHealth, setup.EnemyMaxHealth),
                setup.EnemyId);

            return new CombatState(player, enemy, setup.Seed);
        }

        public Combatant GetCombatant(Side side)
        {
            switch (side)
            {
                case Side.Player: return Player.Combatant;
                case Side.Enemy: return Enemy.Combatant;
                default: throw new ArgumentOutOfRangeException(nameof(side));
            }
        }

        internal void SetActiveSide(Side side) => ActiveSide = side;

        internal void AdvanceTurnNumber() => TurnNumber++;

        internal void SetOutcome(CombatOutcome outcome) => Outcome = outcome;
    }
}
