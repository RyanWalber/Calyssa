using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidaPlayer : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 3;
    public int vidaAtual;

    [Header("Arraste os Corações do Canvas aqui")]
    public GameObject[] coracoesUI;

    public static Vector3 pontoDeRespawn;
    public static bool temCheckpointSalvo = false;

    void Start()
    {
        vidaAtual = vidaMaxima;

        if (temCheckpointSalvo)
        {
            transform.position = pontoDeRespawn;
        }

        AtualizarInterface();
    }

    public void GanharVida(int quantidade)
    {
        vidaAtual += quantidade;
        if (vidaAtual > vidaMaxima)
        {
            vidaAtual = vidaMaxima;
        }
        AtualizarInterface();
    }

    public void Machucar(int dano)
    {
        vidaAtual -= dano;
        AtualizarInterface();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void AtualizarInterface()
    {
        for (int i = 0; i < coracoesUI.Length; i++)
        {
            if (coracoesUI[i] != null)
            {
                if (i < vidaAtual)
                {
                    coracoesUI[i].SetActive(true);
                }
                else
                {
                    coracoesUI[i].SetActive(false);
                }
            }
        }
    }

    void Morrer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}