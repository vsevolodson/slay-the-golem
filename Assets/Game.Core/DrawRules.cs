using System.Collections.Generic;
using Game.Core.State;

namespace Game.Core
{
    internal static class DrawRules
    {
        internal static void Draw(PlayerSide player, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (player.DrawPile.Count == 0)
                {
                    if (player.DiscardPile.Count == 0)
                        return;

                    RefillDrawPileFromDiscard(player);
                }

                player.AddToHand(player.RemoveTopOfDrawPile());
            }
        }

        private static void RefillDrawPileFromDiscard(PlayerSide player)
        {
            var cards = new List<CardId>(player.DiscardPile);
            cards.Reverse();

            player.SetDrawPile(cards);
            player.ClearDiscardPile();
        }
    }
}