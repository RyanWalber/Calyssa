using UnityEngine;
using UnityEngine.UI; // Necessário para mexer na Interface
using UnityEngine.SceneManagement; // Necessário para reiniciar o jogo

public class VidaPlayer : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 3;
    public int vidaAtual;

    [Header("Arraste os Corações do Canvas aqui")]
    public GameObject[] coracoesUI; // Lista de corações

    void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarInterface();
    }

    // --- NOVO: Função para Ganhar Vida (Chamada pelo Item) ---
    public void GanharVida(int quantidade)
    {
        vidaAtual += quantidade;

        // Trava a vida para não passar do máximo (ex: não ficar com 4 corações se o max é 3)
        if (vidaAtual > vidaMaxima)
        {
            vidaAtual = vidaMaxima;
        }

        Debug.Log("Player recuperou vida! Vida: " + vidaAtual);

        // Importante: Atualiza os corações na tela para mostrar que curou
        AtualizarInterface();
    }
    // ---------------------------------------------------------

    // O Inimigo vai chamar essa função quando tocar em você
    public void Machucar(int dano)
    {
        vidaAtual -= dano;
        Debug.Log("Player tomou dano! Vida: " + vidaAtual);

        // Atualiza os corações na tela
        AtualizarInterface();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void AtualizarInterface()
    {
        // Esse loop verifica cada coração
        for (int i = 0; i < coracoesUI.Length; i++)
        {
            // Se o índice for menor que a vida atual, mostra o coração.
            // Se não, esconde.
            if (i < vidaAtual)
            {
                if (coracoesUI[i] != null) coracoesUI[i].SetActive(true);
            }
            else
            {
                if (coracoesUI[i] != null) coracoesUI[i].SetActive(false);
            }
        }
    }

    void Morrer()
    {
        Debug.Log("Game Over!");
        // Reinicia a fase atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}