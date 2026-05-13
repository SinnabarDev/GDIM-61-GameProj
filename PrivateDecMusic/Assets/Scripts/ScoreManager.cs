using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public AudioSource hitSFX;
    public AudioSource missSFX;

    public GameObject hitVFXPrefab;

    public Slider accuracyGauge;

    public static int hits;
    public static int misses;

    void Start()
    {
        Instance = this;

        hits = 0;
        misses = 0;

        UpdateGauge();
    }

    public static void Hit(Vector3 position)
    {
        hits++;
        Instance.hitSFX.Play();

        if (Instance.hitVFXPrefab != null)
        {
            Instantiate(Instance.hitVFXPrefab, position, Quaternion.identity);
        }

        Instance.UpdateGauge();
    }

    public static void Miss()
    {
        misses++;
        Instance.missSFX.Play();

        Instance.UpdateGauge();
    }

    public static float GetAccuracy(int totalNotes)
    {
        if (totalNotes == 0)
            return 0f;

        return ((float)hits / totalNotes) * 100f;
    }

    public static void ResetScore()
    {
        hits = 0;
        misses = 0;

        if (Instance != null)
            Instance.UpdateGauge();
    }

    public static float GetLiveAccuracy()
    {
        int total = SongManager.Instance.GetTotalNotes();

        if (total == 0)
            return 0f;

        return ((float)hits / total) * 100f;
    }

    void UpdateGauge()
    {
        if (accuracyGauge == null)
            return;

        accuracyGauge.value = GetLiveAccuracy();
    }
}
