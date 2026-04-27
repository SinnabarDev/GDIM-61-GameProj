using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    //public TMP_Text nameText;

    private string[] lines;
    private int index;
    private System.Action onComplete;


    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(string[] dialogueLines, System.Action onFinish)
    {
        dialoguePanel.SetActive(true);

        //nameText.text = npcName;

        lines = dialogueLines;
        index = 0;
        onComplete = onFinish;

        ShowLine();
    }

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    void ShowLine()
    {
        dialogueText.text = lines[index];
    }

    void NextLine()
    {
        Debug.Log($"INDEX: {index}");
    Debug.Log($"LINES NULL? {lines == null}");
    Debug.Log($"LINES LENGTH: {(lines != null ? lines.Length : -1)}");
    Debug.Log($"TEXT REF: {dialogueText}");
        index++;

        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        onComplete?.Invoke();
    }
}