using UnityEngine;
using System;

public class ColliderTriggerScript : MonoBehaviour
{



[SerializeField] private FuelController fuelController;


public event EventHandler<OnPickUpEventArgs> onPickUp;//Cria um evento com um parametro
public class OnPickUpEventArgs : EventArgs//cria uma classe que herda/extend o generico de EventArgs, podendo criar um "array" de novos argumentos em um Invoke
    {
        public int coinValue;//Vai carregar o valor de escore da aterriçagem
    }

int coinValueHolder;
    void Awake()
    {
        fuelController = GetComponent<FuelController>();
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    { 
        if(collider2D.gameObject.TryGetComponent(out GasCan gasCan)){
            fuelController.RefuelKopter(gasCan.FuelHold);
            Debug.Log("Refulled");
            gasCan.DestroySelf();
            return;
        }

        if(collider2D.gameObject.TryGetComponent(out CoinPickUp coinPickUp)){
            onPickUp?.Invoke(this, new OnPickUpEventArgs
            {
                coinValue = coinPickUp.CoinScoreValue,
            });//(Sender, Argumento) sendo This quer dizer que o script está mandando, e coinPickUp.CoinScoreValue é a propriedade de coinPickUp
            coinPickUp.DestroySelf();
            return;
        }

    }
    
    
}
