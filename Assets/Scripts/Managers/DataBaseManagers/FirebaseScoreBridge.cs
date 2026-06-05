using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseScoreBridge : MonoBehaviour
{
    private void Start()
    {
        if (CollisionScript.Instance != null)
        {
            CollisionScript.Instance.onLanded += HandleLandingEvent;
        }
    }

    private void OnDestroy()
    {
        if (CollisionScript.Instance != null)
        {
            CollisionScript.Instance.onLanded -= HandleLandingEvent;
        }
    }

    private void HandleLandingEvent(object sender, CollisionScript.OnLandedEventArgs e)
    {
        string phaseId = SceneManager.GetActiveScene().name;
            
    Debug.Log($"Session ended. Saving score: {e.score} (Type: {e.landingTypes}) for phase: {phaseId}");

    FirebaseManager firebase = Object.FindFirstObjectByType<FirebaseManager>();
        if (firebase != null)
        {
            // Se for uma falha (WrongLanding, etc), o e.score já será 0 graças ao seu CollisionScript!
            firebase.SaveGameScore(e.score, phaseId);
        }
        else
        {
            Debug.LogError("FirebaseManager not found in the current scene!");
        }
    }
}