using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Landingpad : MonoBehaviour
{
    [SerializeField] int scoreMultiplier;
    public int ReturnScore()
    {
        return scoreMultiplier;   
    }
}
