using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePickerManager : MonoBehaviour
{
    [SerializeField] private FirebaseManager firebaseManager;
    
    public void ScenePicker(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
        firebaseManager.auth.SignOut();
    }

    public void StartLevel(string levelName)
    {
        if (string.IsNullOrEmpty(FirebaseManager.selectedPatientId))
        {
            Debug.LogWarning("Selecione um paciente no Dropdown antes de iniciar a fase!");
            return; 
        }

        Debug.Log($"Iniciando sessão para o paciente {FirebaseManager.selectedPatientId}. Carregando: {levelName}");

        SceneManager.LoadScene(levelName);
    }
}
