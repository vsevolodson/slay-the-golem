using Game.Core.State;

namespace Game.Core.Commands
{
    public sealed class ChooseRewardCommand : ICommand
    {
        public CardId Card { get; }

        public ChooseRewardCommand(CardId card)
        {
            Card = card;
        }
    }
}