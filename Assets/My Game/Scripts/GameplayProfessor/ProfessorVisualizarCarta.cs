using UnityEngine;
using UnityEngine.UI;

public class ProfessorVisualizarCarta : MonoBehaviour
{
    public static ProfessorVisualizarCarta Instance;

    public Image imagemCarta; // Arraste o Image da UI aqui no Inspector

    void Awake()
    {
        // Implementação simples de Singleton
        if (Instance == null)
        {
            Instance = this;
            // Se quiser manter entre cenas, descomente a linha abaixo
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MostrarCartaSelecionada(string idCarta)
    {
        // Busca a carta correta na lista de todas as cartas pelo ID
        Carta carta = CartaManager.Instance.todasAsCartas.Find(c => c.id == idCarta);

        if (carta != null)
        {
            imagemCarta.sprite = carta.imagem;
            imagemCarta.color = Color.white; // Torna a imagem visível
        }
        else
        {
            Debug.LogWarning($"Nenhuma carta encontrada com o ID: {idCarta}");
        }
    }

    public void DefinirInvisivel()
    {
        imagemCarta.sprite = null;
        imagemCarta.color = new Color(1, 1, 1, 0); // Torna a imagem invisível
    }
}

 
