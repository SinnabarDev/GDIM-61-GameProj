using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCLogSection : MonoBehaviour
{
    public TMP_Text npcNameText;
    public GameObject clueContainer;
    public TMP_Text clueText;

    private string clues = "";

    public void Initialize(string npcName)
    {
        npcNameText.text = npcName;

        clues = "";
        clueText.text = "";

        clueContainer.SetActive(false);

        RecalculateAll();
    }

    public void Toggle()
    {
        clueContainer.SetActive(!clueContainer.activeSelf);

        RecalculateAll();
    }

    public void AddClue(string clue)
    {
        if (clues.Contains(clue))
            return;

        clues += "• " + clue + "\n\n";

        clueText.text = clues;

        RecalculateAll();
    }

    void RecalculateAll()
    {
        Canvas.ForceUpdateCanvases();

        Transform t = transform;

        while (t != null)
        {
            RectTransform rect = t.GetComponent<RectTransform>();

            if (rect != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

            t = t.parent;
        }

        Canvas.ForceUpdateCanvases();
    }
}
