using TMPro;
using UnityEngine;

public class InputRebindManager : MonoBehaviour
{
    public TMP_Text[] keyLabels;

    private int waitingForLane = -1;

    void Awake()
    {
        PlayerPrefs.DeleteAll();
    }

    void Start()
    {
        InitializeDefaults();
        LoadBindings();
    }

    void Update()
    {
        if (waitingForLane >= 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                waitingForLane = -1;
                LoadBindings();
                return;
            }

            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    SaveKey(waitingForLane, key);
                    waitingForLane = -1;
                    break;
                }
            }
        }
    }

    public void StartRebind(int laneIndex)
    {
        waitingForLane = laneIndex;
        keyLabels[laneIndex].text = "Press key...";
    }

    void SaveKey(int laneIndex, KeyCode key)
    {
        // ❗ prevent duplicate bindings
        for (int i = 0; i < keyLabels.Length; i++)
        {
            string existing = PlayerPrefs.GetString("LaneKey_" + i, "");

            if (existing == key.ToString())
            {
                // reset previous lane using this key
                PlayerPrefs.SetString("LaneKey_" + i, GetDefaultKey(i).ToString());

                keyLabels[i].text = GetDefaultKey(i).ToString();
            }
        }

        PlayerPrefs.SetString("LaneKey_" + laneIndex, key.ToString());
        keyLabels[laneIndex].text = key.ToString();

        PlayerPrefs.Save();
    }

    void LoadBindings()
    {
        for (int i = 0; i < keyLabels.Length; i++)
        {
            string saved = PlayerPrefs.GetString("LaneKey_" + i, GetDefaultKey(i).ToString());

            keyLabels[i].text = saved;
        }
    }

    void InitializeDefaults()
    {
        for (int i = 0; i < keyLabels.Length; i++)
        {
            if (!PlayerPrefs.HasKey("LaneKey_" + i))
            {
                PlayerPrefs.SetString("LaneKey_" + i, GetDefaultKey(i).ToString());
            }
        }

        PlayerPrefs.Save();
    }

    KeyCode GetDefaultKey(int lane)
    {
        switch (lane)
        {
            case 0:
                return KeyCode.LeftArrow;
            case 1:
                return KeyCode.DownArrow;
            case 2:
                return KeyCode.UpArrow;
            case 3:
                return KeyCode.RightArrow;
        }

        return KeyCode.None;
    }
}
