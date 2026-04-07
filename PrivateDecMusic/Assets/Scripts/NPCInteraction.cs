using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject rhythmGameUI;
    public GameObject player;

    [Header("This NPC's Song")]
    public SongData npcSong;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartMinigame();
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            EndMinigame();
        }
    }

    void StartMinigame()
    {
        rhythmGameUI.SetActive(true);

        // Disable player movement
        player.GetComponent<PlayerMovement>().enabled = false;

        // 🔥 LOAD THE SONG FIRST
        SongManager.Instance.LoadSong(npcSong);

        // Optional safety: ensure MIDI is ready before playing
        SongManager.Instance.GetDataFromMidi();

        // Start audio
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