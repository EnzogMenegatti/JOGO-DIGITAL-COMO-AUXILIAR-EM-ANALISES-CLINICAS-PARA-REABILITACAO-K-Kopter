using UnityEngine;
using TMPro;

public class PatientCardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text nameText;
    public TMP_Text ageText;
    public TMP_Text heightWeightText;
    public TMP_Text notesText;

    public void SetupCard(PatientData patient)
    {
        nameText.text = patient.patientName;
        ageText.text = patient.patientAge.ToString();
        heightWeightText.text = patient.patientHeight.ToString() + "m | " + patient.patientWeight.ToString() + "kg";
        notesText.text = patient.patientNotes;
    }
}