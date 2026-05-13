using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;

    public GameObject notePrefab;
    public GameObject missText;

    [Header("Arrow Feedback")]
    public Transform arrowSprite;
    public float throbScale = 1.25f;
    public float throbDuration = 0.08f;

    private Vector3 originalScale;
    private Coroutine throbRoutine;

    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    Queue<double> inputBuffer = new Queue<double>();

    int spawnIndex = 0;
    int inputIndex = 0;

    void Start()
    {
        missText.SetActive(false);
        input = LoadKey();

        if (arrowSprite != null)
            originalScale = arrowSprite.localScale;
    }

    KeyCode LoadKey()
    {
        string keyString = PlayerPrefs.GetString("LaneKey_" + GetLaneIndex(), input.ToString());

        // 🔴 SAFE PARSE (prevents crashes)
        if (Enum.TryParse(keyString, out KeyCode result))
        {
            return result;
        }

        // fallback if corrupted
        return input;
    }

    int GetLaneIndex()
    {
        switch (noteRestriction)
        {
            case Melanchall.DryWetMidi.MusicTheory.NoteName.F:
                return 0; // Left

            case Melanchall.DryWetMidi.MusicTheory.NoteName.G:
                return 1; // Down

            case Melanchall.DryWetMidi.MusicTheory.NoteName.A:
                return 2; // Up

            case Melanchall.DryWetMidi.MusicTheory.NoteName.B:
                return 3; // Right
        }

        return 0;
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        timeStamps.Clear();

        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(
                    note.Time,
                    SongManager.midiFile.GetTempoMap()
                );

                double time =
                    metricTimeSpan.Minutes * 60f
                    + metricTimeSpan.Seconds
                    + metricTimeSpan.Milliseconds / 1000.0;

                timeStamps.Add(time);
            }
        }
    }

    void Update()
    {
        if (!SongManager.Instance || !SongManager.Instance.isGameActive)
            return;

        double audioTime =
            SongManager.GetAudioSourceTime()
            - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

        double margin = SongManager.Instance.marginOfError;

        while (
            spawnIndex < timeStamps.Count
            && audioTime >= timeStamps[spawnIndex] - SongManager.Instance.noteTime
        )
        {
            var noteObj = Instantiate(notePrefab, transform);
            var note = noteObj.GetComponent<Note>();

            note.assignedTime = (float)timeStamps[spawnIndex];
            notes.Add(note);

            spawnIndex++;
        }

        if (Input.GetKeyDown(input))
        {
            inputBuffer.Enqueue(audioTime);
            ThrobArrow();
        }

        while (inputBuffer.Count > 0 && inputIndex < timeStamps.Count)
        {
            double inputTime = inputBuffer.Peek();
            double noteTime = timeStamps[inputIndex];

            double diff = Math.Abs(inputTime - noteTime);

            if (diff <= margin)
            {
                Vector3 hitPos = notes[inputIndex].transform.position;

                Hit(hitPos);

                Destroy(notes[inputIndex].gameObject);

                inputBuffer.Dequeue();
                inputIndex++;
            }
            else if (inputTime < noteTime - margin)
            {
                inputBuffer.Dequeue();
            }
            else
            {
                Miss();
                inputIndex++;
            }
        }

        while (inputIndex < timeStamps.Count && audioTime > timeStamps[inputIndex] + margin)
        {
            Miss();
            inputIndex++;
        }
    }

    private void Hit(Vector3 position)
    {
        ScoreManager.Hit(position);
    }

    private void Miss()
    {
        ScoreManager.Miss();
        StartCoroutine(ShowMissText());
    }

    public void ResetLane()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        notes.Clear();
        timeStamps.Clear();
        inputBuffer.Clear();
        missText.SetActive(false);

        spawnIndex = 0;
        inputIndex = 0;
    }

    private System.Collections.IEnumerator ShowMissText()
    {
        if (missText == null)
            yield break;

        missText.SetActive(true);
        yield return new WaitForSeconds(1f);
        missText.SetActive(false);
    }

    private void ThrobArrow()
    {
        if (arrowSprite == null)
            return;

        if (throbRoutine != null)
            StopCoroutine(throbRoutine);

        throbRoutine = StartCoroutine(DoThrob());
    }

    private System.Collections.IEnumerator DoThrob()
    {
        arrowSprite.localScale = originalScale * throbScale;
        yield return new WaitForSeconds(throbDuration);
        arrowSprite.localScale = originalScale;
    }
}
