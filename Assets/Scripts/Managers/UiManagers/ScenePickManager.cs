using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePickerManager : MonoBehaviour
{
    public void ScenePicker(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
