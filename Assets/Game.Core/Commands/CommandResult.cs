using System;

namespace Game.Core.Commands
{
    public readonly struct CommandResult
    {
        public CommandRejection Rejection { get; }

        private CommandResult(CommandRejection rejection)
        {
            Rejection = rejection;
        }

        public bool IsSuccess => Rejection == CommandRejection.None;

        public static CommandResult Success() => new CommandResult(CommandRejection.None);

        public static CommandResult Rejected(CommandRejection rejection)
        {
            if (rejection == CommandRejection.None)
                throw new ArgumentException("rejection reason must be specified", nameof(rejection));

            return new CommandResult(rejection);
        }
    }
}