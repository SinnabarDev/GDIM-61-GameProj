using NUnit.Framework;
using UnityEngine;
using System.Collections;

public class MinigameManager : MonoBehaviour
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

        if (Input.GetKeyDown(KeyCode.Escape) && isPlaying)
        {
            EndMinigame();
        }
        if (resultsScreen.panel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            ContinueAfterResults();
        }
    }
    public void StartMinigame(NPCInteraction npc, NPCData data)
    {
        isPlaying = true;
        currentNPC = npc;
        currentNPCData = data;

        rhythmGameUI.SetActive(true);
        polygraph.SetActive(true);

        ScoreManager.ResetScore();
        SongManager.Instance.LoadSong(data.song, data.midiFileName);
    }

    public void EndMinigame()
    {
        isPlaying = false;
        rhythmGameUI.SetActive(false);
        polygraph.SetActive(false);
        int totalNotes = SongManager.Instance.GetTotalNotes();
        float accuracy = ScoreManager.GetAccuracy(totalNotes);

        lastAccuracy = accuracy;
        SongManager.Instance.EndSong();
        resultsScreen.ShowResults(totalNotes);
    }

    public void ContinueAfterResults()
    {
        Debug.Log(lastAccuracy);

        door.TryUnlock(lastAccuracy);

        if (lastAccuracy > 85f)
        {
            currentNPC.anim.SetBool("isHypno", false);

            currentNPC.StartHintDialogue(OnConversationEnd);
        }
        else
        {
            OnConversationEnd();
        }

        resultsScreen.HideResults();
    }

    public void togglePlayerMovement()
    {
        player.GetComponent<PlayerMovement>().enabled = !player.GetComponent<PlayerMovement>().enabled;
    }
    private void OnConversationEnd()
    {
        togglePlayerMovement();
        StartCoroutine(ResetInteractionLock());
    }
    private IEnumerator ResetInteractionLock()
{
    yield return new WaitForEndOfFrame();
    yield return new WaitForSeconds(0.1f);
    currentNPC.UnlockInteraction();
}
}