using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SavedData
{
    public string Level;
    public int Score;
    public int NumLife;

    public SavedData(string level, int score, int numLife)
    {
        Level = level;
        Score = score;
        NumLife = numLife;
    }

    public void Save(SavedData data)
    {
        Level = data.Level;
        Score = data.Score;
        NumLife = data.NumLife;
    }
}