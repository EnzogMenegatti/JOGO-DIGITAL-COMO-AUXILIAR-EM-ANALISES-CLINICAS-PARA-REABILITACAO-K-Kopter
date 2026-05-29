using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;


public class LandedUi : MonoBehaviour
{

[SerializeField] private TextMeshProUGUI labelTextMesh;
[SerializeField] private TextMeshProUGUI scoreTextMesh;
[SerializeField] private UnityEngine.UI.Button RestartButton;
[SerializeField] private UnityEngine.UI.Button MainMenuButton;
[SerializeField] private UnityEngine.UI.Button ContinueButton;


    void Start()
    {   
        CollisionScript.Instance.onLanded += Collision_onLanded;
        Hide();
    }

    private void Awake()
    {
    }

    public void Collision_onLanded(object sender, CollisionScript.OnLandedEventArgs e)//recebe os parametros do invocador de evento;
    {
        if (e.landingTypes == CollisionScript.LandingTypes.Sucess)
        {
            labelTextMesh.text = "SUCESSFUL LANDING!";
        }
        else
        {
            labelTextMesh.text = "<color=#ff0000>CRASHED!</color>";
        }
            scoreTextMesh.text = 
            Mathf.RoundToInt (e.landingSpeed)*10f + "\n" +
            Mathf.RoundToInt (e.dotVector) + "\n" +
            "x" + Mathf.RoundToInt (e.scoreMultipler) + "\n" +
            Mathf.RoundToInt (ScoreManager.Instance.ReturnScore());
            Show();
        }

    private void Show()
    {
        gameObject.SetActive(true);   
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    }
