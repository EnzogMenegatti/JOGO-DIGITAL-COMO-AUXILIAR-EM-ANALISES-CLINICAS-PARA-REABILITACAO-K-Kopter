using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{   

    public static ScoreManager Instance {get; private set;}
   
    float time;
    int score;
    bool isTimeEnable;
    [SerializeField] private ColliderTriggerScript colliderTriggerScript;
    [SerializeField] private CollisionScript collisionScript;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(isTimeEnable){
        time += Time.deltaTime;
        }
        AddScore(Mathf.RoundToInt(Time.deltaTime * 5f));
    }

    private void Start()
    {     
        if (colliderTriggerScript != null) 
            colliderTriggerScript.onPickUp += Collider_onPickup;
            
        if (collisionScript != null) 
            collisionScript.onLanded += Collision_onLanded;
            
        if (LanderController.Instance != null) 
            LanderController.Instance.onStateChanged += Lander_onStateChanged;
    }

    private void OnDestroy() 
    {
        if (colliderTriggerScript != null) 
            colliderTriggerScript.onPickUp -= Collider_onPickup;
            
        if (collisionScript != null) 
            collisionScript.onLanded -= Collision_onLanded;
            
        if (LanderController.Instance != null) 
            LanderController.Instance.onStateChanged -= Lander_onStateChanged;
    }


    public void Lander_onStateChanged(object sender, LanderController.onStateChangedEventArgs e){
        isTimeEnable = e.state == LanderController.PlayerState.Start;
    }

    public void Collider_onPickup(object sender, ColliderTriggerScript.OnPickUpEventArgs e)//recebe os dois parametros do invocador de evento;
    {
        AddScore(e.coinValue);
        Debug.Log(e.coinValue);

    }

    public void Collision_onLanded(object sender, CollisionScript.OnLandedEventArgs e)//recebe os dois parametros do invocador de evento;
    {
        AddScore(e.score);
        Debug.Log(score);
    }


    public void AddScore(int addScoreAmmount)
    {
        score += addScoreAmmount;
        Debug.Log("Score: " + score);
    }

    public int ReturnScore()
    {
        return score;
    } 

    public float ReturnTime()
    {
        return time;
    }

    public void FinalizarPartida(int pontuacaoFinal)
    {
        // 2. Captura o nome exato da cena que está aberta agora
        string idDaFase = SceneManager.GetActiveScene().name;

        Debug.Log($"Partida finalizada na cena: {idDaFase}. Salvando score...");

        // 3. Procura o FirebaseManager e envia o score usando o nome da cena como ID
        FirebaseManager firebase = Object.FindFirstObjectByType<FirebaseManager>();
        if (firebase != null)
        {
            firebase.SaveGameScore(pontuacaoFinal, idDaFase);
        }
    }
}
