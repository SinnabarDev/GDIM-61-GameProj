using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Scriptable Objects/NPCData")]
public class NPCData : ScriptableObject
{
    public string npcName;
    [Header("Dialogue")]
    public string dialogueFileName;
    
    [Header("Minigame")]
    public AudioClip song;
    public string midiFileName;
}
