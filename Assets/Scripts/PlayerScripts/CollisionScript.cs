using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CollisionScript : MonoBehaviour
{
    [SerializeField]int finalScore;
    [SerializeField]private GameObject vfxExplosion;
    public static CollisionScript Instance {get; private set;}
    public event EventHandler<OnLandedEventArgs> onLanded;//Cria evento quando uma aterriçagem acontecer
    public class OnLandedEventArgs : EventArgs{//cria uma classe que herda/extend o generico de EventArgs, podendo criar um "array" de novos argumentos em um Invoke
        public LandingTypes landingTypes;
        public float landingSpeed;//Vai carregar o valor a velocidade de aterrissagem
        public float dotVector;//Vai carregar a diferença entre os dois raios dos angulos (Helicoptero e terreno)
        public float scoreMultipler;
        public int score;//Vai carregar o valor de escore da aterriçagem
    }
    public enum LandingTypes{//Enum é um tipo de field que recebe valores que podem definir estados
        Sucess,
        WrongLanding,
        SteepAngle,
        TooFastLanding,
    }

    private void Awake(){
        Instance = this;
    }

    private void OnEnable()
    {
        CollisionScript.Instance.onLanded += Collision_OnLanded;
    }

    private void OnCollisionEnter2D(Collision2D collision2D){//parametro nativo unity pra colisão 
        if (!collision2D.gameObject.TryGetComponent(out Landingpad landingPad)){//o parametro checa a classe do objeto colidido, se não tiver o script de Landingpad, é uma batida
            Debug.Log("Crashed");
            onLanded?.Invoke(this, new OnLandedEventArgs{//(sender = objeto que envia, Arguments = nova classe de argumentos, que aqui será o argumento score recebendo o valor finalScore da classe)
            landingTypes = LandingTypes.WrongLanding,
            landingSpeed = 0f,
            dotVector = 0f,
            score = 0,
        });
            LanderController.Instance.enabled = false;
            return;
            
        }

        float softLandingVelocityMagnitude = 3f;//define a força necessaria pra um pouso suave pra variavel
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude){//se a magnitude da colisão for maior que a variavel
            Debug.Log("Hard Landing!");
            onLanded?.Invoke(this, new OnLandedEventArgs{//(sender = objeto que envia, Arguments = nova classe de argumentos, que aqui será o argumento score recebendo o valor finalScore da classe)
            landingTypes = LandingTypes.TooFastLanding,
            landingSpeed = relativeVelocityMagnitude,
            dotVector = 0f,
            score = 0,
        });
            LanderController.Instance.enabled = false;
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);//DotVector recebe o produto escalar entre os angulos do objeto e do terreno
        float minDotVector = .97f;//define o valor minimo do angulo necessario (1 = 0f, 0 = 90f, -1 = 180f)
        if (dotVector < minDotVector){//Tira a diferença entre os angulos, se for menos que .90f(Diferença de 25f entre objeto e terreno) é uma falha
            Debug.Log("Steep angle!");
            onLanded?.Invoke(this, new OnLandedEventArgs{//(sender = objeto que envia, Arguments = nova classe de argumentos, que aqui será o argumento score recebendo o valor finalScore da classe)
            landingTypes = LandingTypes.SteepAngle,
            landingSpeed = relativeVelocityMagnitude,
            dotVector = dotVector,
            score = 0,
        });
            LanderController.Instance.enabled = false;
            return;
        }

        Debug.Log("Safe Landing");
        float ScoreMultiplier = 100f; //Multiplicador do placar geral
        float maxScorelanding = 10; //Valor maximo do placar de pouso
        float scoreLanding = (maxScorelanding - Mathf.Abs(dotVector - 1f) * ScoreMultiplier);//calculo de placar de pouso. Você reduz o valor maximo pelo valor escalar dos dois angulos (nave e ch�o), e multiplica pelo multiplicador base.
        float scoreSpeed = ((softLandingVelocityMagnitude - relativeVelocityMagnitude) * ScoreMultiplier);//calculo de placar de velocidade. Voce reduz o valor definido para um pouso suave pelo valor real da for�a de velocidade da nave, e multiplica pelo valor base.

        finalScore = Mathf.RoundToInt((scoreLanding + scoreSpeed) * landingPad.ReturnMultiplier());//calculo do placar final. Você soma o placar de pouso e velocidade, e multiplica pela fun��o criada no objeto landingPad(returnMultiplier retorna o valor da variaviel privada que dita o valor de multiplicador).
        onLanded?.Invoke(this, new OnLandedEventArgs{//(sender = objeto que envia, Arguments = nova classe de argumentos, que aqui será o argumento score recebendo o valor finalScore da classe)
            landingTypes = LandingTypes.Sucess,
            landingSpeed = relativeVelocityMagnitude,
            dotVector = dotVector,
            scoreMultipler = landingPad.ReturnMultiplier(),
            score = ScoreManager.Instance.ReturnScore(),
        });
        LanderController.Instance.enabled = false;
    }
    
    
    public void Collision_OnLanded(object sender, CollisionScript.OnLandedEventArgs e)
    {
        switch(e.landingTypes){
            case(CollisionScript.LandingTypes.SteepAngle):
            case(CollisionScript.LandingTypes.TooFastLanding):
            case(CollisionScript.LandingTypes.WrongLanding):
            Instantiate(vfxExplosion, LanderController.Instance.transform.position, Quaternion.identity);
            break;
        }
    }

    public int FinalScore()
    {
        return finalScore;
    }
}
