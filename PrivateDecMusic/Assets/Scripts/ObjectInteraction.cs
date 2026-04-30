using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ObjectInteraction : MonoBehaviour
{
    public static ObjectInteraction currentNPC;
    public GameObject player;

    [Header("Object Data")]
    public NPCData ObjData;
    private DialogueJSON dialogueData;

    private bool playerInRange = false;
    private bool hasStartedDialogue = false;
    private bool hasTalkedBefore = false;
    [Header("Button Interaction")]
    public GameObject interactPromptPrefab;
    private GameObject currentPrompt;
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
    }

private IEnumerator LoadDialogue()
{
    string path = Path.Combine(Application.streamingAssetsPath, ObjData.dialogueFileName);

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
      ConvEnd();
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