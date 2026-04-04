using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject rhythmGameUI; // your DDR UI
    public GameObject player;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartMinigame();
        }
    }

    void StartMinigame()
    {
        rhythmGameUI.SetActive(true);

        // Disable player movement
        player.GetComponent<PlayerMovement>().enabled = false;

        // Start the song
        SongManager.Instance.StartSong();
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