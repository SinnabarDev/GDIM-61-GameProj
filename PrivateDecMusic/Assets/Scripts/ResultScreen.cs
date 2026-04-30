using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ResultScreen : MonoBehaviour
{
    public GameObject panel;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI missText;
    public TextMeshProUGUI accuracyText;
    public NPCInteraction npcInteraction;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    if (!panel.activeSelf) return;

    if (Input.GetKeyDown(KeyCode.R))
    {
        Retry();
    }
    }
    public void ShowResults(int totalNotes)
    {
        panel.SetActive(true);

        int hits = ScoreManager.hits;
        int misses = ScoreManager.misses;

        float accuracy =
            ScoreManager.GetAccuracy(totalNotes);

        if (accuracy >= 85f)
        {titleText.text = "INTERROGATION SUCCESSFUL";}
        else
        {titleText.text = "INTERROGATION FAILED";}
        rankText.text = "RANK: " + GetRank(accuracy);
        hitText.text = "HITS: " + hits;
        missText.text = "MISSES: " + misses;
        accuracyText.text =
        "ACCURACY: " + accuracy.ToString("F1") + "%";
    }
     string GetRank(float accuracy)
    {
        if (accuracy >= 95f) return "S";
        if (accuracy >= 90f) return "A";
        if (accuracy >= 80f) return "B";
        if (accuracy >= 70f) return "C";
        return "D";
    }

    public void HideResults()
    {
        panel.SetActive(false);
    }
    public void Retry()
    {
    HideResults();
    ScoreManager.ResetScore();
    }
}