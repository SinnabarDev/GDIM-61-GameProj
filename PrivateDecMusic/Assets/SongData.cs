using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "Rhythm/Song Data")]
public class SongData : ScriptableObject
{
    public AudioClip audioClip;
    public string midiFileName;
}
