using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms.Impl;

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

    private void OnEnable()
    {
        colliderTriggerScript.onCoinPickUp += Collider_onCoinPickup;//Toda vez que onCoinPickUp acontecer, triggara o método de mesmo nome
        collisionScript.onLanded += Collision_onLanded;
        LanderController.Instance.onStateChanged += Lander_onStateChanged;
    }

    private void OnDisable()
    {
        colliderTriggerScript.onCoinPickUp -= Collider_onCoinPickup;
        collisionScript.onLanded -= Collision_onLanded;
        LanderController.Instance.onStateChanged -= Lander_onStateChanged;
    }


    public void Lander_onStateChanged(object sender, LanderController.onStateChangedEventArgs e){
        isTimeEnable = e.state == LanderController.PlayerState.Start;
    }

    public void Collider_onCoinPickup(object sender, ColliderTriggerScript.OnCoinPickUpEventArgs e)//recebe os dois parametros do invocador de evento;
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
    }

    public int ReturnScore()
    {
        return score;
    } 

    public float ReturnTime()
    {
        return time;
    } 
}
