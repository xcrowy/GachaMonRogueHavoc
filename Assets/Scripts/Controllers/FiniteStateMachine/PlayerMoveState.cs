using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    protected int lastX, lastY;
    public PlayerMoveState(Entity entity, StateMachine stateMachine, EntityData entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
    {
        
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        SetPreviousPlayerAnimationDirection();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        SetPlayerAnimationDirection();

        // No Diagonal Movement
        if (xInput != 0)
            yInput = 0;

        // Switch to Idle State
        if (xInput == 0f && yInput == 0f)
            stateMachine.ChangeState(player.IdleState);

        if (xInput != 0f || yInput != 0f)
            AssignPreviousPlayerAnimationDirection();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.SetVelocity(playerData.MoveSpeed * xInput, playerData.MoveSpeed * yInput);
    }

    private void SetPlayerAnimationDirection()
    {
        player.Anim.SetFloat("MoveHorizontal", xInput);
        player.Anim.SetFloat("MoveVertical", yInput);
    }

    private void AssignPreviousPlayerAnimationDirection()
    {
        lastX = xInput;
        lastY = yInput;
    }

    private void SetPreviousPlayerAnimationDirection()
    {
        player.Anim.SetFloat("IdleHorizontal", lastX);
        player.Anim.SetFloat("IdleVertical", lastY);
    }
}
