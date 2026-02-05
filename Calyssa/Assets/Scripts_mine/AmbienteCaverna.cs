using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class AmbienteCaverna : MonoBehaviour
{
    [Header("LUZ GLOBAL")]
    public Light2D luzGlobal; 

    [Header("CONFIGURAÇÃO DE INTENSIDADE")]
    public float tempoTransicao = 2.0f;
    
    [Tooltip("Valor da luz quando estiver DENTRO da caverna (Breu)")]
    public float intensidadeNaCaverna = 0f; 

    [Tooltip("Valor da luz quando SAIR da caverna (Fixo em 1)")]
    public float intensidadeFora = 1f;

    private Coroutine corrotinaLuz;

    // Quando ENTRA na área da caverna -> Escurece
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            TrocarLuz(intensidadeNaCaverna);
        }
    }

    // Quando SAI da área da caverna -> Clareia para 1
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            TrocarLuz(intensidadeFora);
        }
    }

    // Função pública para chamar manualmente se precisar (ex: Boss morreu)
    public void RestaurarLuzImediato()
    {
        TrocarLuz(intensidadeFora);
    }

    void TrocarLuz(float alvo)
    {
        if (luzGlobal == null) return;
        
        // Para a transição anterior se houver uma rodando
        if (corrotinaLuz != null) StopCoroutine(corrotinaLuz);
        
        // Começa a nova transição
        corrotinaLuz = StartCoroutine(MudancaSuave(alvo));
    }

    IEnumerator MudancaSuave(float alvo)
    {
        float inicial = luzGlobal.intensity;
        float tempo = 0;

        while (tempo < tempoTransicao)
        {
            luzGlobal.intensity = Mathf.Lerp(inicial, alvo, tempo / tempoTransicao);
            tempo += Time.deltaTime;
            yield return null;
        }

        luzGlobal.intensity = alvo;
    }
}