using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

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

        lines = dialogueLines;
        index = 0;
        onComplete = onFinish;

        ShowLine();
    }

    void Update() // Listen for input to advance dialogue line
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    void ShowLine() // Line by line reading
    {
        dialogueText.text = lines[index];
    }

    void NextLine()
    {
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
        onComplete?.Invoke();
        dialoguePanel.SetActive(false);
    }
}
