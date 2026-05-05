using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class MinigameManager : MonoBehaviour //forked singleton from old NPC script
{
    [Header("UI / Systems")]
    public GameObject rhythmGameUI;
    public GameObject polygraph;
    public ResultScreen resultsScreen;
    public LevelDoor door;

    [Header("References")]
    public GameObject player;
    private NPCData currentNPCData;
    private NPCInteraction currentNPC;
    private float lastAccuracy;
    private bool isPlaying = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isPlaying) //force quit minigame for testing purposes
        {
            EndMinigame();
        }
        if (resultsScreen.panel.activeSelf && Input.GetKeyDown(KeyCode.E)) //continue after results input
        {
            ContinueAfterResults();
        }
        if (
            resultsScreen.panel.activeSelf
            && Input.GetKey(KeyCode.RightShift)
            && Input.GetKey(KeyCode.LeftShift) //cheat code to guarantee passing the minigame for testing purposes
        )
        {
            lastAccuracy = 9999999.99f;
            Debug.Log("Cheat Code Activated");
        }
    }

    public void StartMinigame(NPCInteraction npc, NPCData data)
    {
        isPlaying = true;
        currentNPC = npc;
        currentNPCData = data;

        rhythmGameUI.SetActive(true); //enable rhythm game UI
        polygraph.SetActive(true);

        ScoreManager.ResetScore(); //reset score for new game
        SongManager.Instance.LoadSong(data.song, data.midiFileName); //load the song and midi file for the minigame
    }

    public void EndMinigame()
    {
        isPlaying = false;
        rhythmGameUI.SetActive(false);
        polygraph.SetActive(false);
        int totalNotes = SongManager.Instance.GetTotalNotes(); //calculate accuracy based on total notes and score
        float accuracy = ScoreManager.GetAccuracy(totalNotes);

        lastAccuracy = accuracy;
        SongManager.Instance.EndSong(); //stop the song and clean up
        resultsScreen.ShowResults(totalNotes);
    }

    public void ContinueAfterResults()
    {
        Debug.Log(lastAccuracy);

        door.TryUnlock(lastAccuracy); //try to unlock the door based on accuracy

        if (lastAccuracy > 85f)
        {
            currentNPC.anim.SetBool("isHypno", false); //update NPC animation based on performance

            currentNPC.StartHintDialogue(OnConversationEnd);
        }
        else
        {
            OnConversationEnd(); //if failed, just end the conversation and let the player try again
        }

        resultsScreen.HideResults();
    }

    public void togglePlayerMovement()
    {
        player.GetComponent<PlayerMovement>().enabled = !player
            .GetComponent<PlayerMovement>()
            .enabled;
    }

    private void OnConversationEnd()
    {
        togglePlayerMovement();
        StartCoroutine(ResetInteractionLock()); //unlock NPC interaction after conversation ends brief delay
    }

    private IEnumerator ResetInteractionLock()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        currentNPC.UnlockInteraction();
    }
}
