using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Linq;
using System.Threading.Tasks;

public class GraphDataManager : MonoBehaviour
{
    // Esta função retorna uma lista de 10 números (os scores ou 0 para falhas)
    public async Task<int[]> GetLatestScoresForGraph()
    {
        int[] graphPoints = new int[10]; // Representa as 10 fases

        FirebaseManager firebase = FindObjectOfType<FirebaseManager>();
        string userId = firebase.User.UserId;
        string patientId = FirebaseManager.selectedPatientId;

        var snapshot = await firebase.DBreference.Child("users").Child(userId)
            .Child("patients").Child(patientId).Child("scores").GetValueAsync();

        if (!snapshot.Exists) return graphPoints;

        // Lista temporária para organizar os dados
        List<ScoreData> allScores = new List<ScoreData>();

        foreach (var child in snapshot.Children)
        {
            string json = child.GetRawJsonValue();
            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            allScores.Add(data);
        }

        // Filtra a última tentativa de cada fase (de 1 a 10)
        for (int i = 1; i <= 10; i++)
        {
            // Presume que suas cenas se chamam algo como "Fase_1", "Fase_2", etc.
            string phaseNameToSearch = "Fase_" + i; 

            // Pega o último registro cronológico dessa fase específica
            ScoreData latestAttempt = allScores.LastOrDefault(s => s.phaseName == phaseNameToSearch);

            if (latestAttempt != null)
            {
                // Guarda no array (índice 0 = Fase 1, índice 1 = Fase 2...)
                graphPoints[i - 1] = latestAttempt.scoreValue; 
            }
            else
            {
                // Se o paciente nunca jogou essa fase, podemos deixar 0 ou -1 para esconder a linha
                graphPoints[i - 1] = 0; 
            }
        }

        return graphPoints;
    }
}