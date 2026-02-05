using UnityEngine;

public class SalaDoBoss : MonoBehaviour
{
    [Header("O Chefão")]
    public GameObject boss; // Arraste o objeto do Boss aqui

    [Header("Portas")]
    public GameObject portaEntrada; // A porta que fecha atrás de você
    public GameObject portaSaida;   // A porta que abre quando vence

    private bool batalhaComecou = false;

    void Start()
    {
        // Garante o estado inicial das portas
        if (portaEntrada != null) portaEntrada.SetActive(false); // Começa aberta
        if (portaSaida != null) portaSaida.SetActive(true);   // Começa fechada
    }

    void Update()
    {
        // Se a batalha já começou...
        if (batalhaComecou)
        {
            // Verifica se o Boss foi destruído (é null)
            if (boss == null)
            {
                VencerBatalha();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Se o Player passou pelo gatilho invisível
        if (other.CompareTag("Player") && !batalhaComecou)
        {
            ComecarBatalha();
        }
    }

    void ComecarBatalha()
    {
        batalhaComecou = true;

        // Tranca o jogador na sala
        if (portaEntrada != null) portaEntrada.SetActive(true);

        // Dica: Se quiser ativar o boss só quando entrar, faça aqui:
        if (boss != null) boss.SetActive(true);

        Debug.Log("AS PORTAS SE FECHARAM! LUTA!");
    }

    void VencerBatalha()
    {
        // Abre a saída
        if (portaSaida != null) portaSaida.SetActive(false);

        // Abre a entrada também (opcional, pra voltar)
        if (portaEntrada != null) portaEntrada.SetActive(false);

        Debug.Log("BOSS DERROTADO! CAMINHO LIVRE.");

        // Desliga este script para não rodar mais
        this.enabled = false;
    }
}