using UnityEngine;

[System.Serializable]
public class PatientData
{
    public string patientName;
    public string doctorName;
    public string patientAge;
    public string patientHeight;
    public string patientWeight;
    public string patientNotes;
    public string patientSex;

    public PatientData() { }

    public PatientData(string _doctorName, string _patientName, string _patientSex, string _patientAge, string _patientHeight, string _patientWeight, string _patientNotes)
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