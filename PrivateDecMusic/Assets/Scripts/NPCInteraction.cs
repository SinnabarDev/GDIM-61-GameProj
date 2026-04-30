using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class NPCInteraction : MonoBehaviour
{
    public static NPCInteraction currentNPC;

    [Header("NPC Data")]
    public NPCData npcData;
    private DialogueJSON dialogueData;

    [Header("References")]
    public GameObject interactPromptPrefab;
    public Animator anim;

    private GameObject currentPrompt;
    public bool playerInRange = false;
    private bool hasStartedDialogue = false;
    private bool hasTalkedBefore = false;
    public GameObject player;

    [SerializeField] private MinigameManager minigameManager;

    void Start()
    {
        StartCoroutine(LoadDialogue());
        anim = GetComponent<Animator>();
        anim.SetBool("isHypno", true);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !hasStartedDialogue)
        {
            hasStartedDialogue = true;
            currentNPC = this;
            StartDialogue();
            minigameManager.togglePlayerMovement();
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

            dialogueData = JsonUtility.FromJson<DialogueJSON>(www.downloadHandler.text);
        }
    }

    public void StartDialogue()
    {
        string[] dialogueToUse = !hasTalkedBefore
            ? dialogueData.firstDialogue
            : dialogueData.repeatDialogue;

        DialogueManager.Instance.StartDialogue(dialogueToUse, OnDialogueFinished);
        hasTalkedBefore = true;
    }
    public void UnlockInteraction()
    {
        hasStartedDialogue = false;
    }
    public void StartHintDialogue(System.Action onComplete)
{
    if (dialogueData == null)
    {
        Debug.LogError("Dialogue not loaded");
        onComplete?.Invoke();
        return;
    }

    DialogueManager.Instance.StartDialogue(dialogueData.hintDialogue, onComplete);
}

    private void OnDialogueFinished()
    {
        if (minigameManager != null)
        {
            minigameManager.StartMinigame(this, npcData);
        }
    }

    public void EnablePrompt()
    {
        if (interactPromptPrefab != null && currentPrompt == null)
        {
            currentPrompt = Instantiate(interactPromptPrefab, transform);
            currentPrompt.transform.localPosition = new Vector3(0, 5f, 0);
        }
    }

    public void DisablePrompt()
    {
        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;
        EnablePrompt();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
        DisablePrompt();
    }
}