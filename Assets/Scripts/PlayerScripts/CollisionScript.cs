using System;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    [SerializeField] private GameObject vfxExplosion;
    private int finalScore;

    public static CollisionScript Instance { get; private set; }
    public event EventHandler<OnLandedEventArgs> onLanded;

    public class OnLandedEventArgs : EventArgs
    {
        public LandingTypes landingTypes;
        public float landingSpeed;
        public float dotVector;
        public float scoreMultipler;
        public int score;
    }

    public enum LandingTypes
    {
        Sucess,
        WrongLanding,
        SteepAngle,
        TooFastLanding,
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        onLanded += Collision_OnLanded;
    }

    private void OnDisable()
    {
        onLanded -= Collision_OnLanded;
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (!collision2D.gameObject.TryGetComponent(out Landingpad landingPad))
        {
            Debug.Log("Crashed");
            TriggerEvent(LandingTypes.WrongLanding, 0f, 0f, 0, 0f);
            return;
        }

        float softLandingVelocityMagnitude = 3f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            Debug.Log("Hard Landing!");
            TriggerEvent(LandingTypes.TooFastLanding, relativeVelocityMagnitude, 0f, 0, 0f);
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .97f;
        
        if (dotVector < minDotVector)
        {
            Debug.Log("Steep angle!");
            TriggerEvent(LandingTypes.SteepAngle, relativeVelocityMagnitude, dotVector, 0, 0f);
            return;
        }

        Debug.Log("Safe Landing");
        float ScoreMultiplier = 100f;
        float maxScorelanding = 10f;
        
        float scoreLanding = (maxScorelanding - Mathf.Abs(dotVector - 1f) * ScoreMultiplier);
        float scoreSpeed = ((softLandingVelocityMagnitude - relativeVelocityMagnitude) * ScoreMultiplier);

        finalScore = Mathf.RoundToInt((scoreLanding + scoreSpeed) * landingPad.ReturnMultiplier());
        
        TriggerEvent(LandingTypes.Sucess, relativeVelocityMagnitude, dotVector, ScoreManager.Instance.ReturnScore(), landingPad.ReturnMultiplier());
    }

    private void TriggerEvent(LandingTypes type, float speed, float dot, int currentScore, float multiplier)
    {
        onLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingTypes = type,
            landingSpeed = speed,
            dotVector = dot,
            score = currentScore,
            scoreMultipler = multiplier
        });
        
        LanderController.Instance.enabled = false;
    }

    public void Collision_OnLanded(object sender, OnLandedEventArgs e)
    {
        switch (e.landingTypes)
        {
            case LandingTypes.SteepAngle:
            case LandingTypes.TooFastLanding:
            case LandingTypes.WrongLanding:
                Instantiate(vfxExplosion, LanderController.Instance.transform.position, Quaternion.identity);
                break;
        }
    }

    public int FinalScore()
    {
        return finalScore;
    }
}