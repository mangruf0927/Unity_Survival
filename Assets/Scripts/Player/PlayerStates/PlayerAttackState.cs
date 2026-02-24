using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private readonly PlayerController playerController;
    private readonly PlayerStateMachine stateMachine;

    public PlayerAttackState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public HashSet<PlayerStateEnums> InputHash { get; } = new HashSet<PlayerStateEnums>()
    {
    };

    public HashSet<PlayerStateEnums> LogicHash { get; } = new HashSet<PlayerStateEnums>()
    {
        PlayerStateEnums.IDLE,
        PlayerStateEnums.MOVE,
        PlayerStateEnums.RUN,
    };

    public void Enter()
    {
        if(playerController.currentWeapon.weaponType == WeaponTypeEnums.MELEE)
            playerController.animator.SetTrigger("OnMeleeAttack");
        else if(playerController.currentWeapon.weaponType == WeaponTypeEnums.RANGED)
            playerController.animator.SetTrigger("OnRangedAttack");


        playerController.currentWeapon.EnterAttack();
        playerController.currentWeapon.Attack();
    }

    public void Update()
    {
        var state = playerController.animator.GetCurrentAnimatorStateInfo(0);

        if((state.IsName("attack_melee") || state.IsName("attack_ranged")) && state.normalizedTime >= 1.0f)
        {
            if(playerController.GetDirection() == Vector2.zero)
                stateMachine.ChangeLogicState(PlayerStateEnums.IDLE);
            else
                stateMachine.ChangeLogicState(playerController.IsRun() ? PlayerStateEnums.RUN : PlayerStateEnums.MOVE);
        }
    }

    public void FixedUpdate()
    {
        
    }

    public void Exit()
    {
        playerController.currentWeapon.ExitAttack();
    }
}
