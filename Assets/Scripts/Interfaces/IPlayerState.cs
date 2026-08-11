using System.Collections.Generic;

public interface IPlayerState : IState
{
    HashSet<PlayerStateEnums> InputHash { get; }
    HashSet<PlayerStateEnums> LogicHash { get; }
}
