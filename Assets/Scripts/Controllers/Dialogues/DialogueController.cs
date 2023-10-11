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
    public GameObject dialogueCanvasPrefab;
    #endregion

    #region Dialogue References
    private List<string> dialogueLines;
    private int currentLineIndex = 0;
    private bool isDisplayingText = false;
    [SerializeField] private float textSpeed = 0.5f;
    #endregion

    #region Player References
    private Player player;
    #endregion

    #region Getters/Setters
    public GameObject DialogueCanvas { get; private set; }
    public Image CharacterIcon { get; private set; }
    public Sprite CharacterIconReference { get; private set; }
    public TextMeshProUGUI DialogueText { get; private set; }
    public EnemyHost EnemyHost { get; private set; }
    public EnemyHost EnemyHostReference { get; private set; }
    #endregion

    private void Start()
    {
        DialogueCanvas = Instantiate(dialogueCanvasPrefab);
        Transform dialogueInterface = DialogueCanvas.transform.GetChild(0);

        CharacterIcon = dialogueInterface.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        CharacterIcon.sprite = CharacterIconReference;
        DialogueText = dialogueInterface.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        this.EnemyHost = EnemyHostReference;

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
            DialogueText.text = "";
            StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
        }
    }

    private IEnumerator TypeText(string text)
    {
        isDisplayingText = true;
        foreach (char letter in text)
        {
            DialogueText.text += letter;
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
        Destroy(DialogueCanvas);
        player.PlayerStateMachine.ChangeState(player.InBattleState);
        GameObject battleSystemObject = Instantiate(battleInterface);
        BattleSystem battleSystem = battleSystemObject.GetComponent<BattleSystem>();
        battleSystem.Initialize(EnemyHost);

        this.gameObject.SetActive(false);
    }

    private void CompleteText()
    {
        DialogueText.text = dialogueLines[currentLineIndex];
        StopAllCoroutines();
        isDisplayingText = false;
    }

    public void SetDialogueLines(List<string> lines) => dialogueLines = lines;
    public void SetPlayerInstance(Player player) => this.player = player;
    public void SetCharacterIcon(Sprite sprite) => CharacterIconReference = sprite;
    public void SetEnemyHost(EnemyHost enemyHost) => EnemyHostReference = enemyHost;
}
