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

    return (float)hits / totalNotes;
}

    public static void ResetScore()
    {
        hits = 0;
        misses = 0;
    }
}