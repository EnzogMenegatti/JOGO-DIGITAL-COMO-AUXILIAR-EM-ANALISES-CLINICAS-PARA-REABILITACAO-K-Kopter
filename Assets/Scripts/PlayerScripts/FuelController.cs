using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class FuelController : MonoBehaviour
{

    public static FuelController Instance {get; private set;}

    private float startingFuel = 10f;
    private float fuelMax;
    private float fuelNormalized;

    public float FuelMax
    {
        get{return fuelMax;}
        set{fuelMax = (value<0)? 0:value;}
    }

    private void Awake()
    {
        fuelMax = startingFuel;
        Instance = this;
    }

    public void FuelDepletion()
    {
        FuelMax -= 1f * Time.deltaTime;

        if (FuelMax <= 0)
        {
            LanderController.Instance.enabled = false;
        }
        else
        {
            LanderController.Instance.enabled = true;
            /*Debug.Log($"{Fuel}");*/
        }
    }
    public float RefuelKopter(float newFuel)
    {
        if (newFuel > 10)
        {
            FuelMax = 10;
        }
        else{
        FuelMax += newFuel;
        }
        return FuelMax;
    }

    public float ReturnFuel()
    {
        return fuelMax;
    }

    public float ReturnFuelNormalized()
    {
        fuelNormalized = startingFuel / fuelMax;
        return fuelNormalized;
    }
}
