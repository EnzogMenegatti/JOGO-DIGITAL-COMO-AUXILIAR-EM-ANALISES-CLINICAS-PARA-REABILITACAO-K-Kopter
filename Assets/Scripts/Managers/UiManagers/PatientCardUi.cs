using UnityEngine;
using UnityEngine.UI; // Necessário para acessar os Botões
using TMPro;

public class PatientCardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text nameText;
    public TMP_Text ageText;
    public TMP_Text heightWeightText;
    public TMP_Text notesText;
    
    [Header("Actions")]
    public UnityEngine.UI.Button exportPdfButton;

    private PatientData currentPatientData;

    public void SetupCard(PatientData patient)
    {
        currentPatientData = patient;

        nameText.text = patient.patientName;
        ageText.text = patient.patientAge + " anos";
        heightWeightText.text = patient.patientHeight + "m | " + patient.patientWeight + "kg";
        notesText.text = patient.patientNotes;

        if (exportPdfButton != null)
        {
            exportPdfButton.onClick.RemoveAllListeners(); 
            
            exportPdfButton.onClick.AddListener(ExportThisPatient); 
        }
    }

    private void ExportThisPatient()
    {
        UnityEngine.Debug.Log($"Iniciando exportação de PDF para: {currentPatientData.patientName}");
        
        // Chama o seu gerador de PDF passando os dados salvos neste card específico!
        PDFExportManager.GeneratePatientReport(currentPatientData);
    }
}