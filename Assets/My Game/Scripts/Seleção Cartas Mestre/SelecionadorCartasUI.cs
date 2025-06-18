using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelecionadorCartasUI : MonoBehaviour
{
    public GameObject cartaPrefab;     // Prefab do botão com Image para a carta
    public Transform areaCartas;       // Onde as cartas serão instanciadas (GridLayoutGroup)
    public GameObject tela;            // Painel da tela de seleção (para ativar/desativar)
    public Button botaoVoltar;         // Botão para voltar à tela anterior

    // Referência para as 4 imagens placeholders na interface (arrastei no Inspector)
    public Image[] imagensPlaceholders;

    private CategoriaCarta categoriaAtual;

    private void Start()
    {
        // Configura o botão voltar para esconder a tela quando clicado
        botaoVoltar.onClick.AddListener(FecharTela);
    }

    // Mostra as cartas da categoria selecionada
    public void MostrarCartasDaCategoria(string categoriaStr)
    {
        Debug.Log("MostrarCartasDaCategoria: " + categoriaStr);
        // Converte a string para o enum CategoriaCarta
        categoriaAtual = (CategoriaCarta)System.Enum.Parse(typeof(CategoriaCarta), categoriaStr);

        // Remove cartas antigas da área de cartas
        foreach (Transform child in areaCartas)
        {
            Destroy(child.gameObject);
        }

        // Pega as cartas que pertencem à categoria atual
        List<Carta> cartas = CartaManager.Instance.ObterCartasPorCategoria(categoriaAtual);
        Debug.Log($"Cartas encontradas para categoria {categoriaAtual}: {cartas.Count}");

        // Instancia o prefab para cada carta encontrada
        foreach (Carta carta in cartas)
        {
            GameObject novaCarta = Instantiate(cartaPrefab, areaCartas);
            // Define o sprite da imagem do prefab como o sprite da carta
            novaCarta.GetComponent<Image>().sprite = carta.imagem;

            // Adiciona a função do clique: salvar a carta selecionada, atualizar placeholder e fechar a tela
            novaCarta.GetComponent<Button>().onClick.AddListener(() =>
            {
                CartaManager.Instance.SalvarCartaSelecionada(carta);
                AtualizarPlaceholder(carta);
                tela.SetActive(false);
            });
        }

        // Mostra a tela de seleção
        tela.SetActive(true);
    }

    // Esconde a tela de seleção
    public void FecharTela()
    {
        tela.SetActive(false);
    }













    // Atualiza o placeholder da carta selecionada com o sprite dela
    private void AtualizarPlaceholder(Carta cartaSelecionada)
    {
        // Considera que o enum CategoriaCarta tem valores 0 a 3 para as 4 categorias
        int indice = (int)cartaSelecionada.categoria;

        if (indice >= 0 && indice < imagensPlaceholders.Length)
        {
            imagensPlaceholders[indice].sprite = cartaSelecionada.imagem;
            imagensPlaceholders[indice].color = Color.white; // garante que fique visível
        }
    }
}

