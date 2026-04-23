using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    [SerializeField] int coinScoreValue = 100;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public int CoinScoreValue
    {
        get{return coinScoreValue;}
        set{coinScoreValue = value;}
    }

}
