using System;
using Game.Core.State;

namespace Game.Core.Data
{
    public sealed class CardDefinition
    {
        public CardId Id { get; }
        public int Cost { get; }

        public CardDefinition(CardId id, int cost)
        {
            if (cost < 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "card cost must not be negative");

            Id = id;
            Cost = cost;
        }
    }
}