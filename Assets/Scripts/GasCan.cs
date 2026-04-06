using UnityEngine;

public class GasCan : MonoBehaviour
{
    int fuelHold = 400;

    public int FuelHold
    {
        get{return fuelHold;}
        set{fuelHold = value; }
    }
    
}
