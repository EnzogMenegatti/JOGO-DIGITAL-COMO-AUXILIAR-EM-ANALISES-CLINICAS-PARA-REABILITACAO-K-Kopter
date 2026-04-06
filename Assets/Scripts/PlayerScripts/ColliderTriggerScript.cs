using UnityEngine;

public class ColliderTriggerScript : MonoBehaviour
{

[SerializeField] private FuelController fuelController;

void Awake()
    {
        fuelController = GetComponent<FuelController>();
    }

private void OnTriggerEnter2D(Collider2D collider2D)
    { 
        if(collider2D.gameObject.TryGetComponent(out GasCan gasCan)){
            fuelController.RefuelKopter(gasCan.FuelHold);
            Debug.Log("Refulled");
            Destroy(gasCan.gameObject);
            return;
        }
    }
    
    
}
