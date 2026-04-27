using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public AudioSource hitSFX;
    public AudioSource missSFX;

    public static int hits;
    public static int misses;

    void Start()
    {
        Instance = this;

        hits = 0;
        misses = 0;
    }

public static void Hit()
{
    hits++;
    Instance.hitSFX.Play();
}

    public static void Miss()
    {
        misses++;
        Instance.missSFX.Play();
    }
    
public static float GetAccuracy(int totalNotes)
{
    if (totalNotes == 0) return 0f;

    return ((float)hits / totalNotes)*100;
}

    public static void ResetScore()
    {
        hits = 0;
        misses = 0;
    }
    public static float GetLiveAccuracy()
{
    int total = SongManager.Instance.GetTotalNotes();

    if (total == 0) return 0f;

    return (float)hits / total;
}
}