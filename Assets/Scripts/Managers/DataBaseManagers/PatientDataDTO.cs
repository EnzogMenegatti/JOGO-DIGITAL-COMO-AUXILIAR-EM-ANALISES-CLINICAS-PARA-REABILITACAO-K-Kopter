using UnityEngine;

[System.Serializable]
public class PatientData
{
    public string patientName;
    public string doctorName;
    public int patientAge;
    public float patientHeight;
    public float patientWeight;
    public string patientNotes;
    public string patientSex;

    public PatientData() { }

    public PatientData(string _doctorName, string _patientName, string _patientSex, int _patientAge, float _patientHeight, float _patientWeight, string _patientNotes)
    {
        doctorName = _doctorName;
        patientName = _patientName;
        patientSex = _patientSex;
        patientAge = _patientAge;
        patientHeight = _patientHeight;
        patientWeight = _patientWeight;
        patientNotes = _patientNotes;
    }
}