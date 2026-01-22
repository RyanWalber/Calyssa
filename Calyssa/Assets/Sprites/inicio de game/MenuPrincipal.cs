using UnityEngine;
using UnityEngine.SceneManagement; // <--- Importante: Permite mexer nas cenas

public class MenuPrincipal : MonoBehaviour
{
    // Função para o botão JOGAR
    public void CarregarJogo()
    {
        // Certifique-se que o nome aqui é EXATAMENTE igual ao da sua cena
        SceneManager.LoadScene("Fase 1"); 
    }

    // Função para o botão SAIR (Opcional)
    public void SairDoJogo()
    {
        Debug.Log("Saiu do Jogo"); // Só aparece no editor para testar
        Application.Quit();
    }
}