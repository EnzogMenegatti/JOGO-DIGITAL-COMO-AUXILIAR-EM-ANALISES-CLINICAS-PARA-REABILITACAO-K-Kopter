using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{   

    public static ScoreManager Instance {get; private set;}
   
    int score;
    [SerializeField] private ColliderTriggerScript colliderTriggerScript;
    [SerializeField] private CollisionScript collisionScript;


    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        colliderTriggerScript.onCoinPickUp += Collider_onCoinPickup;//Toda vez que onCoinPickUp acontecer, triggara o método de mesmo nome
        collisionScript.onLanded += Collision_onLanded;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent memory leaks/errors when objects are destroyed
        colliderTriggerScript.onCoinPickUp -= Collider_onCoinPickup;
        collisionScript.onLanded -= Collision_onLanded;
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
}
