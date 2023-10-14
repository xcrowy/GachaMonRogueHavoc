using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Entity entity;
    protected StateMachine stateMachine;
    protected EntityData entityData;
    protected float animStartTime;

    private string animBoolName;

    public State(Entity entity, StateMachine stateMachine, EntityData entityData, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.entityData = entityData;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        DoChecks();
        entity.Anim.SetBool(animBoolName, true);
        animStartTime = Time.time;
    }

    public virtual void Exit()
    {
        entity.Anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
}
