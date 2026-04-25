using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
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
        StartCoroutine(LoadDialogue());
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

private IEnumerator LoadDialogue()
{
    string path = Path.Combine(Application.streamingAssetsPath, npcData.dialogueFileName);

    using (UnityWebRequest www = UnityWebRequest.Get(path))
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Dialogue load failed: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;

        dialogueData = JsonUtility.FromJson<DialogueJSON>(json);

        Debug.Log("Loaded dialogue for " + npcData.npcName);

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
    }

public void EndMinigame()
{
    rhythmGameUI.SetActive(false);
    hasStartedDialogue = false;
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