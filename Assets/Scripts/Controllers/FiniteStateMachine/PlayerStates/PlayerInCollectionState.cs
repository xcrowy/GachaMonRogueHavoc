using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCollectionState : PlayerGroundedState
{
    public PlayerInCollectionState(Entity entity, StateMachine stateMachine, EntityData entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
    {
    }
}
