using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
    protected Player player;
    protected PlayerStateMachine playerStateMachine;
    protected PlayerData playerData;
    protected string animBoolName;

    public PlayerState(Entity entity, StateMachine stateMachine, EntityData entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
    {
        player = entity.GetComponent<Player>();
        playerStateMachine = (PlayerStateMachine)stateMachine;
        playerData = (PlayerData)entityData;
        this.animBoolName = animBoolName;
    }
}
