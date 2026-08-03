using System.Collections.Generic;
using Game.Core.State;
using Game.Core.Rng;

namespace Game.Core
{
    internal static class DrawRules
    {
        internal static void Draw(PlayerSide player, int count, XorShiftRng rng)
        {
            for (var i = 0; i < count; i++)
            {
                if (player.DrawPile.Count == 0)
                {
                    if (player.DiscardPile.Count == 0)
                        return;

                    RefillDrawPileFromDiscard(player, rng);
                }

                player.AddToHand(player.RemoveTopOfDrawPile());
            }
        }

        private static void RefillDrawPileFromDiscard(PlayerSide player, XorShiftRng rng)
        {
            var cards = new List<CardId>(player.DiscardPile);
            ShuffleRules.Shuffle(cards, rng);

            player.SetDrawPile(cards);
            player.ClearDiscardPile();
        }
    }
}