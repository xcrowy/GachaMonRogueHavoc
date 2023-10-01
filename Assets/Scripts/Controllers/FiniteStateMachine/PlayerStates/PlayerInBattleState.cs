using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInBattleState : PlayerGroundedState
{
    public PlayerInBattleState(Entity entity, StateMachine stateMachine, EntityData entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
    {
    }
}
