using UnityEngine;

public class SalaDoBoss : MonoBehaviour
{
    [Header("Configurações")]
    public BossUmbra scriptDoBoss; // Arraste o objeto do Boss (Umbra) aqui
    public float tempoParaAcordar = 3f;

    [Header("Portas")]
    public GameObject portaEntrada; // A porta que fecha nas costas (PORTA_0)
    public GameObject portaSaida;   // A porta do final (PORTA_0 (1))

    private bool ativou = false;

    void Start()
    {
        // Garante que a entrada está aberta e a saída fechada
        if(portaEntrada != null) portaEntrada.SetActive(false);
        if(portaSaida != null) portaSaida.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ativou) return; // Se já ativou, não faz nada

        if (other.CompareTag("Player"))
        {
            ativou = true;
            Debug.Log("O Player entrou na arena!");

            // 1. FECHA A PORTA (TRANCA O JOGADOR)
            if (portaEntrada != null) 
            {
                portaEntrada.SetActive(true); // Agora ela aparece e tem colisão sólida!
            }

            // 2. ACORDA O BOSS
            if (scriptDoBoss != null)
            {
                // Passamos as referências para o Boss saber qual porta abrir quando morrer
                scriptDoBoss.portaSaida = portaSaida; 
                scriptDoBoss.AcordarBoss(tempoParaAcordar);
            }

            // Opcional: Destrói este gatilho para não ativar de novo
            // (Mas mantém as portas funcionando)
            GetComponent<Collider2D>().enabled = false;
        }
    }
}