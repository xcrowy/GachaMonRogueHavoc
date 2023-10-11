using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    #region Object References
    public GameObject battleInterface;
    public GameObject dialogueInterface;
    public Image characterIcon;
    #endregion

    #region Dialogue References
    public TextMeshProUGUI dialogueText;
    private List<string> dialogueLines;
    private int currentLineIndex = 0;
    private bool isDisplayingText = false;
    [SerializeField] private float textSpeed = 0.5f;
    #endregion

    #region Player References
    private Player player;
    #endregion

    #region EnemyHost Reference
    public EnemyHost enemyHost { get; private set; }
    #endregion

    private void Start()
    {
        dialogueInterface.SetActive(true);
        StartDialogue();
    }

    private void Update()
    {
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            if (isDisplayingText)
                CompleteText();
            else
                ContinueText();
        }
    }

    private void StartDialogue()
    {
        currentLineIndex = 0;
        DisplayLines();
    }

    private void DisplayLines()
    {
        if(currentLineIndex < dialogueLines.Count)
        {
            dialogueText.text = "";
            StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
        }
    }

    private IEnumerator TypeText(string text)
    {
        isDisplayingText = true;
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isDisplayingText = false;
    }

    private void ContinueText()
    {
        if (isDisplayingText)
            CompleteText();
        else
        {
            currentLineIndex++;

            if (currentLineIndex < dialogueLines.Count)
                DisplayLines();
            else
                EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueInterface.SetActive(false);
        player.PlayerStateMachine.ChangeState(player.InBattleState);
        GameObject battleSystemObject = Instantiate(battleInterface);
        BattleSystem battleSystem = battleSystemObject.GetComponent<BattleSystem>();
        battleSystem.Initialize(enemyHost);
    }

    private void CompleteText()
    {
        dialogueText.text = dialogueLines[currentLineIndex];
        StopAllCoroutines();
        isDisplayingText = false;
    }

    public void SetDialogueLines(List<string> lines) => dialogueLines = lines;
    public void SetPlayerInstance(Player player) => this.player = player;
    public void SetCharacterIcon(Sprite sprite) => characterIcon.sprite = sprite;
    public void SetEnemyHost(EnemyHost enemyHost) => this.enemyHost = enemyHost;
}
