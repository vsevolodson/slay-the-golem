using Game.Core.State;

namespace Game.Core.Commands
{
    public sealed class PlayCardCommand : ICommand
    {
        public CardId Card { get; }

        public PlayCardCommand(CardId card)
        {
            Card = card;
        }
    }
}