using System;
using System.Collections.Generic;
using Game.Core.State;

namespace Game.Core.Data
{
    public sealed class RunConfig
    {
        private readonly List<string> _fights;
        private readonly List<CardId> _rewardPool;
        private readonly List<CardId> _startingDeck;

        public CombatConfig Combat { get; }
        public int PlayerMaxHealth { get; }
        public int RewardChoices { get; }
        public string RelicId { get; }

        public IReadOnlyList<string> Fights => _fights;
        public IReadOnlyList<CardId> RewardPool => _rewardPool;
        public IReadOnlyList<CardId> StartingDeck => _startingDeck;

        public RunConfig(CombatConfig combat, IEnumerable<string> fights, IEnumerable<CardId> rewardPool,
            IEnumerable<CardId> startingDeck, int playerMaxHealth, int rewardChoices, string relicId = null)
        {
            Combat = combat ?? throw new ArgumentNullException(nameof(combat));

            _fights = new List<string>(fights ?? throw new ArgumentNullException(nameof(fights)));
            _rewardPool = new List<CardId>(rewardPool ?? throw new ArgumentNullException(nameof(rewardPool)));
            _startingDeck = new List<CardId>(startingDeck ?? throw new ArgumentNullException(nameof(startingDeck)));

            if (_fights.Count == 0)
                throw new ArgumentException("a run must have at least one fight", nameof(fights));
            if (playerMaxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(playerMaxHealth));
            if (rewardChoices <= 0)
                throw new ArgumentOutOfRangeException(nameof(rewardChoices));
            if (!string.IsNullOrEmpty(relicId) && !combat.Relics.Contains(relicId))
                throw new ArgumentException($"relic '{relicId}' is not in the catalog", nameof(relicId));

            PlayerMaxHealth = playerMaxHealth;
            RewardChoices = rewardChoices;
            RelicId = relicId;
        }
    }
}