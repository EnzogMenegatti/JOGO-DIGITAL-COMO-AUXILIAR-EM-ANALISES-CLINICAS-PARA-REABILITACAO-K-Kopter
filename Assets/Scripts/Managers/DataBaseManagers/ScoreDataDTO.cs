using System;

[System.Serializable]
public class ScoreData
{
    public int scoreValue;
    public string phaseName;
    public string dateAndHour;

    public ScoreData() { }

    public ScoreData(int _score, string _phaseName)
    {
        scoreValue = _score;
        phaseName = _phaseName;
        dateAndHour = DateTime.Now.ToString("dd/MM/yyyy - HH:mm");
    }
}