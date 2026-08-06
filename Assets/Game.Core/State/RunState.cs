using System;
using System.Collections.Generic;
using Game.Core.Rng;
using Newtonsoft.Json;

namespace Game.Core.State
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RunState
    {
        [JsonProperty] private List<CardId> _deck;
        [JsonProperty] private List<CardId> _rewardOffer;

        [JsonProperty] public int MaxHealth { get; private set; }
        [JsonProperty] public int Health { get; private set; }
        [JsonProperty] public int FightIndex { get; private set; }
        [JsonProperty] public RunPhase Phase { get; private set; }
        [JsonProperty] public XorShiftRng Rng { get; private set; }
        [JsonProperty] public ulong CombatSeed { get; private set; }

        public IReadOnlyList<CardId> Deck => _deck;
        public IReadOnlyList<CardId> RewardOffer => _rewardOffer;

        private RunState(int maxHealth, IEnumerable<CardId> deck, ulong seed)
        {
            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "max health must be positive");
            if (deck == null)
                throw new ArgumentNullException(nameof(deck));

            MaxHealth = maxHealth;
            Health = maxHealth;
            FightIndex = 0;
            Phase = RunPhase.InCombat;
            Rng = new XorShiftRng(seed);
            CombatSeed = Rng.NextUInt64();

            _deck = new List<CardId>(deck);
            _rewardOffer = new List<CardId>();
        }

        [JsonConstructor]
        private RunState()
        {
        }

        public static RunState Create(int maxHealth, IEnumerable<CardId> startingDeck, ulong seed) =>
            new RunState(maxHealth, startingDeck, seed);

        internal void SetHealth(int health) => Health = Math.Max(0, health);

        internal void AddCard(CardId card) => _deck.Add(card);

        internal void SetPhase(RunPhase phase) => Phase = phase;

        internal void AdvanceFight() => FightIndex++;

        internal void SetRewardOffer(IEnumerable<CardId> cards) => _rewardOffer = new List<CardId>(cards);

        internal void ClearRewardOffer() => _rewardOffer.Clear();

        internal void SetCombatSeed(ulong seed) => CombatSeed = seed;
    }
}