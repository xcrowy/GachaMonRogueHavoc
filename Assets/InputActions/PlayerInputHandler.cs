using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool InteractInput { get; private set; }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();
        NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        int checkIfEnemy = GetComponent<Player>().ReturnRaycastHit();

        if (context.started && checkIfEnemy == LayerMask.NameToLayer("EnemyHost"))
            InteractInput = true;
    }

    public void UseInteractInput() => InteractInput = false;
}
