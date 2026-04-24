using UnityEngine;

public class GasCan : MonoBehaviour
{
    [SerializeField] int fuelHold = 100;
    
public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public int FuelHold
    {
        get{return fuelHold;}
        set{fuelHold = value; }
    }
    
}
