using UnityEngine;
using System.IO;

public class NPCInteraction : MonoBehaviour
{
    public static NPCInteraction currentNPC;

    public GameObject rhythmGameUI;
    public GameObject player;

    [Header("NPC Data")]
    public NPCData npcData;
    private DialogueJSON dialogueData;

    private bool playerInRange = false;
    private bool hasStartedDialogue = false;
    private bool hasTalkedBefore = false;
[Header("Button Interaction")]
    public GameObject interactPromptPrefab;
private GameObject currentPrompt;
public LevelDoor door;
    void Start()
    {
        LoadDialogue();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)&&
        !hasStartedDialogue)
        {
            hasStartedDialogue = true;
            currentNPC = this;
            StartDialogue();
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.Escape))
        {
            EndMinigame();
        }
    }

    void LoadDialogue()
    {
        Debug.Log(npcData);
        string path = Path.Combine(Application.streamingAssetsPath, npcData.dialogueFileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            dialogueData = JsonUtility.FromJson<DialogueJSON>(json);
        }
        else
        {
            Debug.LogError("Dialogue file not found: " + path);
        }
    }

public void StartDialogue()
{
    string[] dialogueToUse;

    if (!hasTalkedBefore)
    {
        dialogueToUse = dialogueData.firstDialogue;
        hasTalkedBefore = true;
    }
    else
    {
        dialogueToUse = dialogueData.repeatDialogue;
    }

    DialogueManager.Instance.StartDialogue(
        npcData.npcName,
        dialogueToUse,
        OnDialogueFinished
    );
}

    public void OnDialogueFinished()
    {
        StartMinigame();
    }

    void StartMinigame()
    {
        rhythmGameUI.SetActive(true);

        player.GetComponent<PlayerMovement>().enabled = false;
        ScoreManager.ResetScore();
        SongManager.Instance.LoadSong(npcData.song, npcData.midiFileName);
        SongManager.Instance.StartSong();
    }

public void EndMinigame()
{
    rhythmGameUI.SetActive(false);
    player.GetComponent<PlayerMovement>().enabled = true;
    int totalNotes = SongManager.Instance.GetTotalNotes();
    float accuracy = ScoreManager.GetAccuracy(totalNotes) * 100f;
    door.TryUnlock(accuracy);
    SongManager.Instance.EndSong();
}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPromptPrefab != null && currentPrompt == null)
        {
            currentPrompt = Instantiate(interactPromptPrefab, transform);

            currentPrompt.transform.localPosition = new Vector3(0, 5f, 0); // above NPC
        }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
        }
    }
}
}