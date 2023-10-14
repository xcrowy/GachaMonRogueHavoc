using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHost : Entity
{
    #region Object References
    public GameObject sign;
    public Sprite icon;
    #endregion

    #region Player References
    public Player Player { get; set; }
    private PlayerStateMachine playerStateMachine;
    #endregion

    #region Dialogue References
    public GameObject dialogueControllerPrefab;
    [SerializeField] private List<string> dialogueLines = new();
    #endregion

    #region Party System References
    public List<Enemy> enemyPartyMembers = new();
    #endregion

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            sign.SetActive(true);
            Player = collision.GetComponent<Player>();
            playerStateMachine = Player.PlayerStateMachine;
            Player.SetVelocity(0f, 0f);

            if (Keyboard.current[Key.Space].isPressed && playerStateMachine.CurrentState != Player.EncounterState)
            {
                playerStateMachine.ChangeState(Player.EncounterState);
                InitializeDialogueController();
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        sign.SetActive(false);
    }

    private void InitializeDialogueController()
    {
        GameObject dialogueControllerObject = Instantiate(dialogueControllerPrefab);
        DialogueController dialogueController = dialogueControllerObject.GetComponent<DialogueController>();
        dialogueController.SetDialogueLines(dialogueLines);
        dialogueController.SetPlayerInstance(Player);
        dialogueController.SetCharacterIcon(icon);
        dialogueController.SetEnemyHost(this);
    }

}
