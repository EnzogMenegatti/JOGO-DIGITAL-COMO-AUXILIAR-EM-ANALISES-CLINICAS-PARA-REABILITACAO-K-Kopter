using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class FuelController : MonoBehaviour
{
    private float startingfuel = 10f;
    private float fuel;

    public float Fuel
    {
        get{return fuel;}
        set{fuel = (value<0)? 0:value;}
    }

    void Start()
    {
        fuel = startingfuel;
    }

    public void FuelDepletion()
    {
        Fuel -= 1f * Time.deltaTime;

        if (Fuel <= 0)
        {
            GetComponent<PlayerController>().enabled = false;
        }
        else
        {
            GetComponent<PlayerController>().enabled = true;
            Debug.Log($"{Fuel}");
        }
    }
    public float RefuelKopter(float newFuel)
    {
        Fuel += newFuel;
        return Fuel;
    }
}
