using Mono.Cecil;
using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class LanderController : MonoBehaviour
{
    private const float GRAVITY_SCALE = 0.1f;
    public static LanderController Instance {get; private set;}//Singleton, estou definindo que essa classe é estatica (Não existe instancias dela, apenas ela) e estanciando ela dentro da propria classe uma unica vez
    [SerializeField] float force = 100f;
    [SerializeField] float forceXY = 15f;
    [SerializeField] private Rigidbody2D landerRigidbody2D;
    [SerializeField] private FuelController fuelController;
    [SerializeField] private ManagerAPI managerAPI;
    [SerializeField] private float facing;
    
    public event EventHandler onUpForce;//Cria uma variavel de evento. Eventos são usados para comunicar com partes desacopladas, mantendo um encapsulamento segruo
    public event EventHandler onLeftForce;//Cria uma variavel de evento. Eventos são usados para comunicar com partes desacopladas, mantendo um encapsulamento segruo
    public event EventHandler onRightForce;//Cria uma variavel de evento. Eventos são usados para comunicar com partes desacopladas, mantendo um encapsulamento seguro
    public event EventHandler<onStateChangedEventArgs> onStateChanged;
    public class onStateChangedEventArgs : EventArgs
    {
        public PlayerState state;
    }
    private PlayerState playerState;
    public enum PlayerState
    {
        WaitingForStart,
        Start,
    }
    private void Awake()
    {   
        Instance = this;
        landerRigidbody2D = GetComponent<Rigidbody2D>();
        fuelController = GetComponent<FuelController>();
        managerAPI = GetComponent<ManagerAPI>();
        playerState = PlayerState.WaitingForStart;
    }

    private void FixedUpdate(){
        switch(playerState){
            default:
            case PlayerState.WaitingForStart:
                if(Keyboard.current.upArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                {
                    this.landerRigidbody2D.gravityScale = GRAVITY_SCALE;
                    playerState = PlayerState.Start;
                    onStateChanged?.Invoke(this, new onStateChangedEventArgs{
                    state = playerState,
                    });
                }
            break;
            case PlayerState.Start:
                bool hasFuel = FuelController.Instance.CurrentFuel > 0;
                if (hasFuel){
                if(Keyboard.current.upArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                {
                    fuelController.FuelDepletion();
                }
                if (Keyboard.current.upArrowKey.isPressed)//quando tecla pra cima pressionada
                {
                    landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime);//adiciona força pro cima local do foguete
                    onUpForce?.Invoke(this, EventArgs.Empty);//invocação de evento
                }
                if (Keyboard.current.leftArrowKey.isPressed)//quando tecla pra esquerda pressioanda
                {
                    landerRigidbody2D.AddForce(forceXY * -1 * transform.right * Time.deltaTime);//adiciona força de rotação pra esquerda
                    onLeftForce?.Invoke(this, EventArgs.Empty);//invoca evento
                }
                if (Keyboard.current.rightArrowKey.isPressed)//quando tecla pra direita pressionada
                {
                    landerRigidbody2D.AddForce(forceXY * transform.right * Time.deltaTime);//adiciona força de rotação pra direita
                    onRightForce?.Invoke(this, EventArgs.Empty);//invoca evento
                }
                }
            break;
        }
    }

    public float ReturnSpeed()
    {
        return LanderController.Instance.landerRigidbody2D.linearVelocityX;
    }
}
