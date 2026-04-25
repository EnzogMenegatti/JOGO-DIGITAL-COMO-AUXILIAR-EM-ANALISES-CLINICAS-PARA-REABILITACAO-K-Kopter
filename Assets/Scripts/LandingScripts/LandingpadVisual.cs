using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class LandingpadVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreMultiplier;
    private void Awake()
    {
        Landingpad landingPad = GetComponent<Landingpad>();
        scoreMultiplier.text = "x" + landingPad.ReturnMultiplier();
    }
}
