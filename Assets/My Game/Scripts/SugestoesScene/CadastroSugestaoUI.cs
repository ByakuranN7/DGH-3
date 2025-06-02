using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CadastroSugestaoUI : MonoBehaviour
{
    [Header("Placeholders das Cartas")]
    public Image imagemInvasao;
    public Image imagemPrivilegios;
    public Image imagemPersistencia;
    public Image imagemC2;

    [Header("Botões para selecionar cartas")]
    public Button botaoInvasao;
    public Button botaoPrivilegios;
    public Button botaoPersistencia;
    public Button botaoC2;

    [Header("Descrição da sugestão e titulo")]
    public TMP_InputField inputDescricao;
    public TMP_InputField inputTitulo;

    [Header("Botão para registrar")]
    public Button botaoRegistrar;

    [Header("Tela de seleção de cartas")]
    public GameObject telaSelecaoCartas;  // Painel da seleção
    public Transform areaCartas;          // Grid para instanciar cartas
    public GameObject prefabCartaBotao;   // Prefab do botão de carta

    // Armazenar os IDs selecionados das cartas
    private string idCartaInvasao = null;
    private string idCartaPrivilegios = null;
    private string idCartaPersistencia = null;
    private string idCartaC2 = null;

    // Guarda qual categoria está sendo selecionada
    private CategoriaCarta categoriaAtual;

    // Referência para o Rest_Controller
    private Rest_Controller restController;

    void Start()
    {
        restController = FindObjectOfType<Rest_Controller>();
        if (restController == null)
        {
            Debug.LogError("Rest_Controller não encontrado na cena!");
        }

        // Configura os botões para abrir a seleção
        botaoInvasao.onClick.AddListener(() => MostrarCartasDaCategoria(CategoriaCarta.InvasaoInicial));
        botaoPrivilegios.onClick.AddListener(() => MostrarCartasDaCategoria(CategoriaCarta.ObtencaoPrivilegios));
        botaoPersistencia.onClick.AddListener(() => MostrarCartasDaCategoria(CategoriaCarta.Persistencia));
        botaoC2.onClick.AddListener(() => MostrarCartasDaCategoria(CategoriaCarta.C2Exfiltracao));

        botaoRegistrar.onClick.AddListener(RegistrarSugestao);

        telaSelecaoCartas.SetActive(false);
    }

    void MostrarCartasDaCategoria(CategoriaCarta categoria)
    {
        categoriaAtual = categoria;

        // Limpa cartas antigas
        foreach (Transform child in areaCartas)
        {
            Destroy(child.gameObject);
        }

        // Busca cartas da categoria
        List<Carta> cartas = CartaManager.Instance.ObterCartasPorCategoria(categoria);
        Debug.Log($"Cartas encontradas para {categoria}: {cartas.Count}");

        // Instancia as cartas
        foreach (Carta carta in cartas)
        {
            GameObject novaCarta = Instantiate(prefabCartaBotao, areaCartas);
            novaCarta.GetComponent<Image>().sprite = carta.imagem;

            // Configura clique da carta
            novaCarta.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelecionarCarta(carta);
                telaSelecaoCartas.SetActive(false);
            });
        }

        // Mostra a tela de seleção
        telaSelecaoCartas.SetActive(true);
    }

    void SelecionarCarta(Carta cartaSelecionada)
    {
        switch (categoriaAtual)
        {
            case CategoriaCarta.InvasaoInicial:
                idCartaInvasao = cartaSelecionada.id;
                imagemInvasao.sprite = cartaSelecionada.imagem;
                SetPlaceholderAlpha(imagemInvasao, 1);
                break;
            case CategoriaCarta.ObtencaoPrivilegios:
                idCartaPrivilegios = cartaSelecionada.id;
                imagemPrivilegios.sprite = cartaSelecionada.imagem;
                SetPlaceholderAlpha(imagemPrivilegios, 1);
                break;
            case CategoriaCarta.Persistencia:
                idCartaPersistencia = cartaSelecionada.id;
                imagemPersistencia.sprite = cartaSelecionada.imagem;
                SetPlaceholderAlpha(imagemPersistencia, 1);
                break;
            case CategoriaCarta.C2Exfiltracao:
                idCartaC2 = cartaSelecionada.id;
                imagemC2.sprite = cartaSelecionada.imagem;
                SetPlaceholderAlpha(imagemC2, 1);
                break;
        }
    }

    void SetPlaceholderAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void RegistrarSugestao()
{
    if (restController == null)
    {
        Debug.LogError("Não foi possível enviar sugestão: Rest_Controller não está atribuído.");
        return;
    }

    if (string.IsNullOrEmpty(idCartaInvasao) || string.IsNullOrEmpty(idCartaPrivilegios) ||
        string.IsNullOrEmpty(idCartaPersistencia) || string.IsNullOrEmpty(idCartaC2))
    {
        Debug.LogWarning("Por favor, selecione uma carta para todas as categorias.");
        return;
    }

    if (string.IsNullOrWhiteSpace(inputDescricao.text))
    {
        Debug.LogWarning("Por favor, digite uma descrição para a sugestão.");
        return;
    }

    if (string.IsNullOrWhiteSpace(inputTitulo.text))
    {
        Debug.LogWarning("Por favor, digite um título para a sugestão.");
        return;
    }

    SugestaoData novaSugestao = new SugestaoData()
    {
    titulo = inputTitulo.text.Trim(),
    cartaInvasaoInicial = idCartaInvasao,
    cartaObtencaoPrivilegios = idCartaPrivilegios,
    cartaPersistencia = idCartaPersistencia,
    cartaC2Exfiltracao = idCartaC2,
    descricao = inputDescricao.text.Trim()
    };


    restController.PostSugestao(novaSugestao, (resultado) =>
    {
        if (resultado == "Sugestão enviada com sucesso!")
        {
            Debug.Log("Sugestão registrada com sucesso!");
            LimparCampos();
            gameObject.SetActive(false);

            // Chama a atualização das sugestões
            SugestoesUI sugestoesUI = FindObjectOfType<SugestoesUI>();
            if (sugestoesUI != null)
            {
                sugestoesUI.AtualizarSugestoes();
            }
            else
            {
                Debug.LogWarning("SugestoesUI não encontrado na cena!");
            }
        }
        else
        {
            Debug.LogWarning("Falha ao registrar sugestão: " + resultado);
        }
    });
}


    void LimparCampos()
    {
        idCartaInvasao = null;
        idCartaPrivilegios = null;
        idCartaPersistencia = null;
        idCartaC2 = null;

        SetPlaceholderAlpha(imagemInvasao, 0);
        SetPlaceholderAlpha(imagemPrivilegios, 0);
        SetPlaceholderAlpha(imagemPersistencia, 0);
        SetPlaceholderAlpha(imagemC2, 0);

        inputDescricao.text = "";
        inputTitulo.text = "";
    }
}
