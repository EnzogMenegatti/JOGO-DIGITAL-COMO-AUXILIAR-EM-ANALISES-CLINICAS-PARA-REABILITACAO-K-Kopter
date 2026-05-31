using UnityEngine;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;

public class PDFExportManager : MonoBehaviour
{
    public static void GeneratePatientReport(PatientData patient)
    {
        // 1. Removida a duplicação. Agora o documento é criado apenas uma vez.
        PdfDocument document = new PdfDocument();
        document.Info.Title = $"Relatório Médico - {patient.patientName}";

        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Fontes
        XFont titleFont = new XFont("Arial", 18, XFontStyle.Bold);
        XFont bodyFont = new XFont("Arial", 12, XFontStyle.Regular);

        // Cabeçalho
        gfx.DrawString("Ficha Médica do Paciente", titleFont, XBrushes.DarkBlue, new XRect(0, 40, page.Width, 50), XStringFormats.TopCenter);

        // Dados do Paciente
        int startY = 100;
        int spacing = 30;

        // 2. Corrigida a matemática do espaçamento (startY + spacing, etc.)
        gfx.DrawString($"Doutor responsável: {patient.doctorName}", bodyFont, XBrushes.Black, 50, startY);
        gfx.DrawString($"Nome: {patient.patientName}", bodyFont, XBrushes.Black, 50, startY + spacing);
        gfx.DrawString($"Sexo: {patient.patientSex}", bodyFont, XBrushes.Black, 50, startY + (spacing * 2));
        gfx.DrawString($"Idade: {patient.patientAge} anos", bodyFont, XBrushes.Black, 50, startY + (spacing * 3));
        gfx.DrawString($"Altura: {patient.patientHeight}m", bodyFont, XBrushes.Black, 50, startY + (spacing * 4));
        gfx.DrawString($"Peso: {patient.patientWeight}kg", bodyFont, XBrushes.Black, 50, startY + (spacing * 5));
        
        gfx.DrawString("Anotações:", new XFont("Arial", 12, XFontStyle.Bold), XBrushes.Black, 50, startY + (spacing * 7));
        
        XRect notesRect = new XRect(50, startY + (spacing * 8), page.Width - 100, 200);
        gfx.DrawString(patient.patientNotes, bodyFont, XBrushes.Black, notesRect, XStringFormats.TopLeft);

        // Rodapé
        gfx.DrawString($"Responsável: Dr(a). {patient.doctorName}", new XFont("Arial", 10, XFontStyle.Italic), XBrushes.Gray, new XRect(0, page.Height - 50, page.Width, 50), XStringFormats.Center);

        string fileName = $"Ficha_{patient.patientName.Replace(" ", "_")}.pdf";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        
        document.Save(filePath);
        UnityEngine.Debug.Log($"PDF exportado com sucesso para: {filePath}");
    }
}