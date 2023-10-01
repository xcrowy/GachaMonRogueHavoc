using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHost : Entity
{
    #region Object References
    public GameObject sign;
    public Sprite icon;
    #endregion

    #region Player References
    public Player Player { get; set; }
    private PlayerStateMachine playerStateMachine;
    private PlayerInputHandler inputHandler;
    #endregion

    #region Dialogue References
    public DialogueController dialogueController;
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
            inputHandler = Player.GetComponent<PlayerInputHandler>();
            Player.SetVelocity(0f, 0f);

            if (inputHandler.InteractInput)
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
        dialogueController.gameObject.SetActive(true);
        dialogueController.SetDialogueLines(dialogueLines);
        dialogueController.SetPlayerInstance(Player);
        dialogueController.SetCharacterIcon(icon);
    }

}
