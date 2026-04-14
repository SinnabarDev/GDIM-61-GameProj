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
    private bool hasInteracted = false;
    private bool hasStartedDialogue = false;

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

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
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
    DialogueManager.Instance.StartDialogue(
        npcData.npcName,
        dialogueData.firstDialogue,
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

        SongManager.Instance.LoadSong(npcData.song, npcData.midiFileName);
        SongManager.Instance.StartSong();
    }

    void EndMinigame()
    {
        rhythmGameUI.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;

        SongManager.Instance.EndSong();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }
}