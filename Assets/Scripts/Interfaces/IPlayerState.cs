using System.Collections.Generic;

public interface IPlayerState
{
    HashSet<PlayerStateEnums> InputHash { get; }
    HashSet<PlayerStateEnums> LogicHash { get; }

    void Enter();
    void Update();
    void FixedUpdate();
    void Exit();
}
