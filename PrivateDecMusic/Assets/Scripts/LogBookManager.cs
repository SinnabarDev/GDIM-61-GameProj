using System.Collections.Generic;
using UnityEngine;

public class LogBookManager : MonoBehaviour
{
    public static LogBookManager Instance;

    public GameObject panel;
    public Transform contentParent;
    public GameObject npcSectionPrefab;
    public GameObject openButton;
    public GameObject closeButton;

    private Dictionary<string, NPCLogSection> sections = new Dictionary<string, NPCLogSection>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Keep WHOLE logbook alive
            DontDestroyOnLoad(gameObject);

            if (panel != null)
                panel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        bool shouldHide = false;

        if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel.activeSelf)
        {
            shouldHide = true;
        }

        MinigameManager mg = FindFirstObjectByType<MinigameManager>();

        if (mg != null && mg.rhythmGameUI.activeSelf)
        {
            shouldHide = true;
        }

        if (closeButton.activeSelf)
        {
            openButton.SetActive(false);
        }
        else
        {
            openButton.SetActive(!shouldHide);
        }

        if (shouldHide)
        {
            panel.SetActive(false);
            return;
        }
    }

    public void AddClue(string npcName, string clue)
    {
        if (contentParent == null || npcSectionPrefab == null)
        {
            Debug.LogError("LogBook references missing.");
            return;
        }

        if (!sections.ContainsKey(npcName))
        {
            GameObject obj = Instantiate(npcSectionPrefab, contentParent);

            NPCLogSection section = obj.GetComponent<NPCLogSection>();

            section.Initialize(npcName);

            sections.Add(npcName, section);
        }

        sections[npcName].AddClue(clue);
    }
}
