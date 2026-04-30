using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoor : MonoBehaviour
{
    [Header("Button Interaction")]
    public GameObject interactPromptPrefab;
    private GameObject currentPrompt;
    public bool unlocked = false;
    public string nextSceneName;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && unlocked && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void UnlockDoor()
    {
        unlocked = true;
        Debug.Log("Door Unlocked!");
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
        Debug.Log("ENTERED DOOR TRIGGER");
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }

        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
        }
    }
    public void TryUnlock(float accuracyPercent)
{
    if (accuracyPercent >= 85f)
    {
        UnlockDoor();
    }
}
}