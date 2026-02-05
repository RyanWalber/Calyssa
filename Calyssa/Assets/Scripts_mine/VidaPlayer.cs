using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VidaPlayer : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 3;
    public int vidaAtual;
    public float tempoDeInvencibilidade = 1.5f; // Tempo que fica piscando
    private bool estaInvencivel = false;

    [Header("Arraste os Corações do Canvas aqui")]
    public GameObject[] coracoesUI;

    public static Vector3 pontoDeRespawn;
    public static bool temCheckpointSalvo = false;

    private SpriteRenderer spritePlayer;

    void Start()
    {
        vidaAtual = vidaMaxima;
        spritePlayer = GetComponentInChildren<SpriteRenderer>(); // Pega o visual para piscar

        if (temCheckpointSalvo)
        {
            transform.position = pontoDeRespawn;
        }

        AtualizarInterface();
    }

    // Essa função agora é chamada pelo Boss
    public void Machucar(int dano)
    {
        // Se já tomou dano recentemente, ignora
        if (estaInvencivel) return;

        vidaAtual -= dano;
        AtualizarInterface();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            StartCoroutine(InvulneravelRoutine());
        }
    }

    IEnumerator InvulneravelRoutine()
    {
        estaInvencivel = true;
        
        // Efeito de piscar (opcional, mas ajuda visualmente)
        if (spritePlayer != null) 
        {
            for (int i = 0; i < 5; i++)
            {
                spritePlayer.color = new Color(1, 1, 1, 0.5f); // Transparente
                yield return new WaitForSeconds(tempoDeInvencibilidade / 10);
                spritePlayer.color = Color.white; // Normal
                yield return new WaitForSeconds(tempoDeInvencibilidade / 10);
            }
        }
        else 
        {
            yield return new WaitForSeconds(tempoDeInvencibilidade);
        }

        estaInvencivel = false;
    }

    public void GanharVida(int quantidade)
    {
        vidaAtual += quantidade;
        if (vidaAtual > vidaMaxima) vidaAtual = vidaMaxima;
        AtualizarInterface();
    }

    void AtualizarInterface()
    {
        for (int i = 0; i < coracoesUI.Length; i++)
        {
            if (coracoesUI[i] != null)
            {
                if (i < vidaAtual) coracoesUI[i].SetActive(true);
                else coracoesUI[i].SetActive(false);
            }
        }
    }

    void Morrer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}