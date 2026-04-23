using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    int finalScore;

    private void OnCollisionEnter2D(Collision2D collision2D)//parametro nativo unity pra colisão
    {   
        if (!collision2D.gameObject.TryGetComponent(out Landingpad landingPad))//o parametro checa a classe do objeto colidido, se não tiver o script de Landingpad, é uma batida
        {
            Debug.Log("Crashed");
            return;
        }

        float softLandingVelocityMagnitude = 3f;//define a força necessaria pra um pouso suave pra variavel
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)//se a magnitude da colisão for maior que a variavel
        {
            Debug.Log("Hard Landing!");
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);//DotVector recebe o produto escalar entre os angulos do objeto e do terreno
        float minDotVector = .97f;//define o valor minimo do angulo necessario (1 = 0f, 0 = 90f, -1 = 180f)
        if (dotVector < minDotVector)//Tira a diferença entre os angulos, se for menos que .90f(Diferença de 25f entre objeto e terreno) é uma falha
        {
            Debug.Log("Steep angle!");
            return;
        }

        Debug.Log("Safe Landing");


        
        float ScoreMultiplier = 100f; //Multiplicador do placar geral
        float maxScorelanding = 10; //Valor maximo do placar de pouso
        float scoreLanding = (maxScorelanding - Mathf.Abs(dotVector - 1f) * ScoreMultiplier);//calculo de placar de pouso. Você reduz o valor maximo pelo valor escalar dos dois angulos (nave e ch�o), e multiplica pelo multiplicador base.
        float scoreSpeed = ((softLandingVelocityMagnitude - relativeVelocityMagnitude) * ScoreMultiplier);//calculo de placar de velocidade. Voce reduz o valor definido para um pouso suave pelo valor real da for�a de velocidade da nave, e multiplica pelo valor base.

        Debug.Log("Score of landing is: " + scoreLanding);
        Debug.Log("Score of speed is: " + scoreSpeed);

        finalScore = Mathf.RoundToInt((scoreLanding + scoreSpeed) * landingPad.ReturnScore());//calculo do placar final. Você soma o placar de pouso e velocidade, e multiplica pela fun��o criada no objeto landingPad(returnMultiplier retorna o valor da variaviel privada que dita o valor de multiplicador).

        Debug.Log("Final Score is: " + finalScore);
    }
}
