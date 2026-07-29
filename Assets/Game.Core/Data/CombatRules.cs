using System;

namespace Game.Core.Data
{
    public sealed class CombatRules
    {
        public int EnergyPerTurn { get; }
        public int CardsDrawnPerTurn { get; }

        public CombatRules(int energyPerTurn, int cardsDrawnPerTurn)
        {
            if (energyPerTurn < 0)
                throw new ArgumentOutOfRangeException(nameof(energyPerTurn), "energy per turn must not be negative");
            if (cardsDrawnPerTurn < 0)
                throw new ArgumentOutOfRangeException(nameof(cardsDrawnPerTurn), "cards drawn per turn must not be negative");

            EnergyPerTurn = energyPerTurn;
            CardsDrawnPerTurn = cardsDrawnPerTurn;
        }
    }
}