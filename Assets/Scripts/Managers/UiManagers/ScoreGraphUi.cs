using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions; // Referência para a biblioteca UI Extensions

public class ScoreGraphUI : MonoBehaviour
{
    [Header("Componentes Visuais")]
    public UILineRenderer linhaGrafico;
    public RectTransform contentorGrafico; // O retângulo invisível que define a área do gráfico
    public GraphDataManager gestorDeDados;

    [Header("Definições do Jogo")]
    [Tooltip("A pontuação máxima possível numa fase perfeita.")]
    public float pontuacaoMaxima = 1000f; // Altera este valor para o máximo do teu jogo

    public async void AtualizarGrafico()
    {
        // 1. Vai buscar as pontuações mais recentes do Firebase (10 posições)
        int[] scores = await gestorDeDados.GetLatestScoresForGraph();
        
        List<Vector2> pontosMatematicos = new List<Vector2>();

        // Captura a largura e a altura do espaço alocado para o gráfico no ecrã
        float larguraDaArea = contentorGrafico.rect.width;
        float alturaDaArea = contentorGrafico.rect.height;

        // O espaço horizontal que separa cada uma das 10 fases (9 intervalos no total)
        float espacamentoX = larguraDaArea / 9f;

        // 2. Transforma as pontuações em posições físicas no Canvas
        for (int i = 0; i < scores.Length; i++)
        {
            // O eixo X avança consoante o número da fase
            float posicaoX = i * espacamentoX;
            
            // O eixo Y sobe consoante a percentagem da pontuação
            float posicaoY = ((float)scores[i] / pontuacaoMaxima) * alturaDaArea;

            pontosMatematicos.Add(new Vector2(posicaoX, posicaoY));
        }

        // 3. Entrega os pontos à linha e força-a a redesenhar-se
        linhaGrafico.Points = pontosMatematicos.ToArray();
        linhaGrafico.SetAllDirty(); 
    }
}