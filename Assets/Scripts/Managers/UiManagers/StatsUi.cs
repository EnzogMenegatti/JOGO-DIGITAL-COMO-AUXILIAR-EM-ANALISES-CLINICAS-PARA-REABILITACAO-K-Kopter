using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsTextMesh;
    [SerializeField] private Image fuelbar;

    private void Update()
    {
        UpdateStatsTextMeshPro();
    }

    private void UpdateStatsTextMeshPro()
    {   
        // 1. Check if FuelController and the fuelbar image exist before updating
        if (FuelController.Instance != null && fuelbar != null)
        {
            fuelbar.fillAmount = FuelController.Instance.ReturnFuelNormalized();
        }

        // 2. Check if ScoreManager, LanderController, and the text component exist
        if (ScoreManager.Instance != null && LanderController.Instance != null && statsTextMesh != null)
        {
            statsTextMesh.text = 
                Mathf.RoundToInt(ScoreManager.Instance.ReturnScore()) + "\n" +
                Mathf.RoundToInt(ScoreManager.Instance.ReturnTime()) + "\n" +
                Mathf.RoundToInt(LanderController.Instance.ReturnSpeed() * 10f);
        }
    }
}