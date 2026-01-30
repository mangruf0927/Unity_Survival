using System.Collections.Generic;

public interface IPlayerState
{
    HashSet<PlayerStateEnums> inputHash { get; }
    HashSet<PlayerStateEnums> logicHash { get; }

    void Enter();
    void Update();
    void FixedUpdate();
    void Exit();
}
