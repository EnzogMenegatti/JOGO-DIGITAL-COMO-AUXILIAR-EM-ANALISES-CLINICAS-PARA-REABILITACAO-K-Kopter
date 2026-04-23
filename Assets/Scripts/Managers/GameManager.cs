using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    int score;
    [SerializeField] private ColliderTriggerScript colliderTriggerScript;

    private void Start()
    {
        colliderTriggerScript.onCoinPickUp += Collider_onCoinPickup;//Toda vez que onCoinPickUp acontecer, triggara o método de mesmo nome
    }


    private void Collider_onCoinPickup(object sender, int coinValue)//recebe os dois parametros do invocador de evento;
    {
        AddScore(coinValue);
    }


    public void AddScore(int addScoreAmmount)
    {
        score += addScoreAmmount;
        Debug.Log(score);
    }
}
