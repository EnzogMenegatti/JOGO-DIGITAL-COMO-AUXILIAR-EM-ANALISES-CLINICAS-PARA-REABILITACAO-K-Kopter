using UnityEngine;

public class GasCan : MonoBehaviour
{
    [SerializeField] int fuelHold = 400;
    
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
