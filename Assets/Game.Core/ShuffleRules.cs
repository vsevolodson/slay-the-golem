using System.Collections.Generic;
using Game.Core.Rng;

namespace Game.Core
{
    internal static class ShuffleRules
    {
        internal static void Shuffle<T>(IList<T> items, XorShiftRng rng)
        {
            for (var i = items.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);

                var swapped = items[i];
                items[i] = items[j];
                items[j] = swapped;
            }
        }
    }
}