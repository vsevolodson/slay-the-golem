using System;
using Newtonsoft.Json;
using Game.Core.Rng;

namespace Game.Core.State
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CombatState
    {
        [JsonProperty] public PlayerSide Player { get; private set; }
        [JsonProperty] public EnemySide Enemy { get; private set; }

        [JsonProperty] public int TurnNumber { get; private set; }
        [JsonProperty] public Side ActiveSide { get; private set; }
        [JsonProperty] public CombatOutcome Outcome { get; private set; }

        [JsonProperty] public XorShiftRng Rng { get; private set; }

        private CombatState(PlayerSide player, EnemySide enemy, ulong seed)
        {
            Player = player;
            Enemy = enemy;
            Rng = new XorShiftRng(seed);
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
