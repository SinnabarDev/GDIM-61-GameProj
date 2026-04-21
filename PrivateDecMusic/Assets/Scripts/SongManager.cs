using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;

    public AudioSource audioSource;
    public Lane[] lanes;

    [Header("Timing")]
    public float songDelayInSeconds;
    public double marginOfError;
    public int inputDelayInMilliseconds;

    public bool isGameActive = false;
    private string midiFileName;

    [Header("Note Settings")]
    public float noteTime;
    public float noteSpawnY;
    public float noteTapY;
    public float noteDespawnY
    {
        get
        {
            return noteTapY - (noteSpawnY - noteTapY);
        }
    }
    public NPCInteraction npcScript;

    public static MidiFile midiFile;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    // -----------------------------
    // PUBLIC API (USED BY NPCs)
    // -----------------------------

    public void LoadSong(AudioClip clip, string midiFileName)
{
    audioSource.clip = clip;
    this.midiFileName = midiFileName;

    ReadMidi();
}

    public void StartSong()
    {
        isGameActive = true;

        audioSource.Play();
        StartCoroutine(WaitForMusicEnd());
    }

    // -----------------------------
    // MIDI LOADING
    // -----------------------------

    private void ReadMidi()
{
    if (string.IsNullOrEmpty(midiFileName))
    {
        Debug.LogError("No MIDI file assigned!");
        return;
    }

    string path = Path.Combine(Application.streamingAssetsPath, midiFileName);

    if (Application.streamingAssetsPath.StartsWith("http://") ||
        Application.streamingAssetsPath.StartsWith("https://"))
    {
        StartCoroutine(ReadFromWebsite(path));
    }
    else
    {
        ReadFromFile(path);
    }
}

    private IEnumerator ReadFromWebsite(string path)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }

            byte[] results = www.downloadHandler.data;

            using (var stream = new MemoryStream(results))
            {
                midiFile = MidiFile.Read(stream);
                GetDataFromMidi();
            }
        }
    }

    private void ReadFromFile(string path)
    {
        Debug.Log("Loading MIDI: " + path);

        midiFile = MidiFile.Read(path);
        GetDataFromMidi();
    }

    // -----------------------------
    // NOTE GENERATION
    // -----------------------------

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();

        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var lane in lanes)
            lane.SetTimeStamps(array);

        CancelInvoke(nameof(StartSong));
        Invoke(nameof(StartSong), songDelayInSeconds);
    }
    public int GetTotalNotes()
{
    int total = 0;

    foreach (var lane in lanes)
    {
        total += lane.timeStamps.Count;
    }

    return total;
}

    // -----------------------------
    // TIME HELPERS
    // -----------------------------

    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples /
               Instance.audioSource.clip.frequency;
    }
    public void EndSong()
    {
        isGameActive = false;
        CancelInvoke(nameof(StartSong));
        StopAllCoroutines();
        audioSource.Stop();
        foreach (var lane in lanes)
        {
            lane.ResetLane();
        }
    }
    IEnumerator WaitForMusicEnd()
    {
    // 1. Wait a frame to ensure AudioSource.Play() has registered
    yield return null; 

    // 2. Wait while it is playing
    yield return new WaitWhile(() => audioSource.isPlaying);

    // 3. Audio finished, end minigame
    npcScript.EndMinigame();
    }
}