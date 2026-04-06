using Mono.Cecil;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] float forceUP = 100f;
    [SerializeField] float forceXY = 15f;
    [SerializeField] float torque = 10f;
    float fuel;
    private Rigidbody2D landerRigidbody2D;
    private FuelController fuelController;
    [SerializeField] private ManagerAPI managerAPI;
    
    private void Awake()
    {
        landerRigidbody2D = GetComponent<Rigidbody2D>();
        fuelController = GetComponent<FuelController>();
        managerAPI = GetComponent<ManagerAPI>();

    }

    public event EventHandler onUpForce;//Cria uma variavel de evento. Eventos são usados para comunicar com partes desacopladas, mantendo um encapsulamento segruo
    public event EventHandler onLeftForce;//Cria uma variavel de evento. Eventos são usados para comunicar com partes desacopladas, mantendo um encapsulamento segruo
    public event EventHandler onRightForce;//Cria uma variavel de evento. Eventos são usados para comunicar com partes desacopladas, mantendo um encapsulamento segruo

    private void FixedUpdate()
    {
        if(Keyboard.current.upArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            fuelController.FuelDepletion();
        }
        if (Keyboard.current.upArrowKey.isPressed)//quando tecla pra cima pressionada
        {
            landerRigidbody2D.AddForce(forceUP * transform.up * Time.deltaTime);//adiciona força pro cima local do foguete
            onUpForce?.Invoke(this, EventArgs.Empty);//invocação de evento
        }
        if (Keyboard.current.leftArrowKey.isPressed)//quando tecla pra esquerda pressioanda
        {
            landerRigidbody2D.AddForce((forceXY * -1) * transform.right * Time.deltaTime);//adiciona força de rotação pra esquerda
            landerRigidbody2D.AddTorque(torque * Time.deltaTime);
            onLeftForce?.Invoke(this, EventArgs.Empty);//invoca evento

        }
        if (Keyboard.current.rightArrowKey.isPressed)//quando tecla pra direita pressionada
        {
            landerRigidbody2D.AddForce(forceXY * transform.right * Time.deltaTime);//adiciona força de rotação pra direita
            landerRigidbody2D.AddTorque(torque * -1 * Time.deltaTime);
            onRightForce?.Invoke(this, EventArgs.Empty);//invoca evento
        }
        
    }

    

}
