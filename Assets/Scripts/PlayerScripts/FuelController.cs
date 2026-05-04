using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class FuelController : MonoBehaviour
{

    public static FuelController Instance {get; private set;}


    private float maxFuel = 10f;
    private float startingFuel = 4f;
    private float currentFuel;
    private float fuelNormalized;

    public float CurrentFuel
    {
        get => currentFuel;
        set => currentFuel = Mathf.Clamp(value, 0, maxFuel);
    }

    private void Awake()
    {   
        Instance = this;
        currentFuel = startingFuel;
    }

    void FixedUpdate()
    {
        
    }

    public void FuelDepletion()
    {
        CurrentFuel -= 1f * Time.deltaTime;
        
    }
    public float RefuelKopter(float newFuel)
    {
        CurrentFuel += newFuel;
        return CurrentFuel;
    }

    public float ReturnFuel()
    {
        return CurrentFuel;
    }

    public float ReturnFuelNormalized()
    {
        fuelNormalized = CurrentFuel / startingFuel;
        return fuelNormalized;
    }
}
