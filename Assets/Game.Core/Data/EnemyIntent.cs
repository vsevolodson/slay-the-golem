using System;

namespace Game.Core.Data
{
    public sealed class EnemyIntent
    {
        public int Damage { get; }
        public int Block { get; }

        public EnemyIntent(int damage, int block)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "intent damage must not be negative");
            if (block < 0)
                throw new ArgumentOutOfRangeException(nameof(block), "intent block must not be negative");

            Damage = damage;
            Block = block;
        }
    }
}