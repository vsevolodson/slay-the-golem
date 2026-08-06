namespace Game.Core.Commands
{
    public enum CommandRejection
    {
        None,
        CombatIsOver,
        NotYourTurn,
        CardNotInHand,
        NotEnoughEnergy,
        UnknownCommand,
        WrongPhase,
        CardNotOffered
    }
}