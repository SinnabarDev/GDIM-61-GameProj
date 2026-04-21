using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;

    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    Queue<double> inputBuffer = new Queue<double>();

    int spawnIndex = 0;
    int inputIndex = 0;

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
                    metricTimeSpan.Minutes * 60f +
                    metricTimeSpan.Seconds +
                    metricTimeSpan.Milliseconds / 1000.0;

                timeStamps.Add(time);
            }
        }
    }

    void Update()
    {
        if (!SongManager.Instance || !SongManager.Instance.isGameActive)
        return;
        double audioTime = SongManager.GetAudioSourceTime() -
            (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

        double margin = SongManager.Instance.marginOfError;

        // -----------------------------
        // SPAWN NOTES
        // -----------------------------
        while(spawnIndex < timeStamps.Count &&
               audioTime >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
        {
            var noteObj = Instantiate(notePrefab, transform);
            var note = noteObj.GetComponent<Note>();

            note.assignedTime = (float)timeStamps[spawnIndex];
            notes.Add(note);

            spawnIndex++;
        }
        // -----------------------------
        // INPUT BUFFER (FIX)
        // -----------------------------
        if (Input.GetKeyDown(input))
        {
            inputBuffer.Enqueue(audioTime);
        }

        // -----------------------------
        // PROCESS INPUTS
        // -----------------------------
        while (inputBuffer.Count > 0 && inputIndex < timeStamps.Count)
        {
            double inputTime = inputBuffer.Peek();
            double noteTime = timeStamps[inputIndex];

            double diff = Math.Abs(inputTime - noteTime);

            if (diff <= margin)
            {
                // HIT
                Hit();
                Destroy(notes[inputIndex].gameObject);

                inputBuffer.Dequeue();
                inputIndex++;
            }
            else if (inputTime < noteTime - margin)
            {
                // TOO EARLY → discard input
                inputBuffer.Dequeue();
            }
            else
            {
                // TOO LATE → miss note
                Miss();
                inputIndex++;
            }
        }

        // -----------------------------
        // MISS HANDLING (no input)
        // -----------------------------
        while (inputIndex < timeStamps.Count &&
               audioTime > timeStamps[inputIndex] + margin)
        {
            Miss();
            inputIndex++;
        }
    }

    private void Hit()
    {
        ScoreManager.Hit();
    }

    private void Miss()
    {
        ScoreManager.Miss();
    }
    public void ResetLane()
{
    // 1. Destroy spawned notes
    foreach (Transform child in transform)
    {
        Destroy(child.gameObject);
    }

    // 2. Clear runtime state
    notes.Clear();
    timeStamps.Clear();
    inputBuffer.Clear();

    // 3. Reset indexes (CRITICAL)
    spawnIndex = 0;
    inputIndex = 0;
}
}