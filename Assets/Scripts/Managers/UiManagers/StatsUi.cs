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

        fuelbar.fillAmount = FuelController.Instance.ReturnFuelNormalized();
        statsTextMesh.text = 
        Mathf.RoundToInt (ScoreManager.Instance.ReturnScore()) + "\n" +
        Mathf.RoundToInt (ScoreManager.Instance.ReturnTime()) + "\n" +
        Mathf.RoundToInt (LanderController.Instance.ReturnSpeed()*10f);
        
    }

}
