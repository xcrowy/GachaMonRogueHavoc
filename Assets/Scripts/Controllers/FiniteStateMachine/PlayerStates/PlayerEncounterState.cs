using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEncounterState : PlayerGroundedState
{
    public PlayerEncounterState(Entity entity, StateMachine stateMachine, EntityData entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        player.InputHandler.UseInteractInput();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
