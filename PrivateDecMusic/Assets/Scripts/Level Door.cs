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
            SceneManager.LoadScene(nextSceneName); //loads next scene when player interacts with door
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

            if (interactPromptPrefab != null && currentPrompt == null & unlocked)
            {
                currentPrompt = Instantiate(interactPromptPrefab, transform); //create prompt prefab as child of door
                currentPrompt.transform.localPosition = new Vector3(0, 5f, 0); // above NPC
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }

        if (currentPrompt != null)
        {
            Destroy(currentPrompt); //remove prompt when player leaves range
        }
    }

    public void TryUnlock(float accuracyPercent) //attempt to unlock door based on minigame performance
    {
        if (accuracyPercent >= 85f)
        {
            UnlockDoor();
        }
    }
}
