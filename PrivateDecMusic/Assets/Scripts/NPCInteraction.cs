using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class NPCInteraction : MonoBehaviour
{
    public static NPCInteraction currentNPC;
    public ResultScreen resultsScreen;
    public GameObject rhythmGameUI;
    public GameObject polygraph;
    public GameObject player;

    [Header("NPC Data")]
    public NPCData npcData;
    private DialogueJSON dialogueData;

    private bool playerInRange = false;
    private bool hasStartedDialogue = false;
    private bool hasEndedDialouge = true;
    private bool hasTalkedBefore = false;
    [Header("Button Interaction")]
    public GameObject interactPromptPrefab;
    private GameObject currentPrompt;
    public LevelDoor door;
    private float lastaccuracy = 0f;
    void Start()
    {
        StartCoroutine(LoadDialogue());
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)&&!hasStartedDialogue)
        {
            hasStartedDialogue = true;
            currentNPC = this;
            StartDialogue();
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.Escape) && hasEndedDialouge)
        {
            EndMinigame();
        }
        if (resultsScreen.panel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            ContinueAfterResults(lastaccuracy);
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

        Debug.Log("Loaded dialogue");

    }
}

public void StartDialogue()
{
    string[] dialogueToUse;
    hasEndedDialouge = false;

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
        dialogueToUse,
        OnDialogueFinished
    );
}

    public void OnDialogueFinished()
    {
        hasEndedDialouge = true;
        StartMinigame();
    }

    void StartMinigame()
    {
        rhythmGameUI.SetActive(true);
        polygraph.SetActive(true);

        player.GetComponent<PlayerMovement>().enabled = false;
        ScoreManager.ResetScore();
        SongManager.Instance.LoadSong(npcData.song, npcData.midiFileName);
    }

public void EndMinigame()
{
    rhythmGameUI.SetActive(false);
    polygraph.SetActive(false);

    int totalNotes = SongManager.Instance.GetTotalNotes();
    float accuracy = ScoreManager.GetAccuracy(totalNotes);
    Debug.Log(accuracy);
    lastaccuracy = accuracy;

    SongManager.Instance.EndSong();

    Debug.Log("Showing results screen");

    resultsScreen.ShowResults(totalNotes);
}
private void ContinueAfterResults(float lastaccuracy)
{
    Debug.Log(lastaccuracy);
    door.TryUnlock(lastaccuracy);
    if (lastaccuracy>89f)
    {
    DialogueManager.Instance.StartDialogue(dialogueData.hintDialogue, ConvEnd);
    }
    else
    {ConvEnd();}
    resultsScreen.HideResults();
}
public void ConvEnd()
{
    player.GetComponent<PlayerMovement>().enabled = true;
    StartCoroutine(ResetInteractionLock());
}
private IEnumerator ResetInteractionLock()
{
    yield return new WaitForEndOfFrame();
    yield return new WaitForSeconds(0.1f);

    hasStartedDialogue = false;
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