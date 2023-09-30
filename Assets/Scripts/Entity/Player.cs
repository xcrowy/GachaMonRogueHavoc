using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    #region State References
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerEncounterState EncounterState { get; private set; }
    public PlayerInBattleState InBattleState { get; private set; }
    #endregion

    #region Data References
    [SerializeField] private PlayerData playerData;
    #endregion

    #region Physics References
    public Rigidbody2D RB { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }
    private Vector2 workspace;
    #endregion

    #region Input Handler
    public PlayerInputHandler InputHandler { get; private set; }
    #endregion

    protected override void Awake()
    {
        PlayerStateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, PlayerStateMachine, playerData, "Idle");
        MoveState = new PlayerMoveState(this, PlayerStateMachine, playerData, "Move");
        EncounterState = new PlayerEncounterState(this, PlayerStateMachine, playerData, "Idle");
        InBattleState = new PlayerInBattleState(this, PlayerStateMachine, playerData, "Idle");
    }

    protected override void Start()
    {
        base.Start();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();

        PlayerStateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        base.Update();
        CurrentVelocity = RB.velocity;
        PlayerStateMachine.CurrentState.LogicUpdate();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PlayerStateMachine.CurrentState.PhysicsUpdate();
    }

    public void SetVelocity(float x, float y)
    {
        workspace.Set(x, y);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public int ReturnRaycastHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, workspace);
        return hit.collider.gameObject.layer;
    }
}
