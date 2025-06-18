using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AbrirCadastroSugestao : MonoBehaviour
{
    public Button botaoNovaSugestao;           // botão que abre a tela de criar sugestao
    public Button botaoFecharCadastro;         // botão que fecha a tela
    public Button botaoRegistrarSugestao;      // botão que registra a sugestão no banco e fecha a tela
    public Button botaoVoltarMenuPrincipal;    // botão para voltar para o menu principal
    public GameObject painelCadastroSugestao; // painel que abre/fecha

    void Start()
    {
        if (botaoNovaSugestao == null || painelCadastroSugestao == null)
        {
            Debug.LogError("Botão ou painel não configurados no Inspector!");
            return;
        }

        botaoNovaSugestao.onClick.AddListener(() =>
        {
            painelCadastroSugestao.SetActive(true);
        });

        if (botaoFecharCadastro != null)
        {
            botaoFecharCadastro.onClick.AddListener(FecharTela);
        }

        if (botaoRegistrarSugestao != null)
        {
            botaoRegistrarSugestao.onClick.AddListener(FecharTela);
        }

        if (botaoVoltarMenuPrincipal != null)
        {
            botaoVoltarMenuPrincipal.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MenuInicial");
            });
        }
    }

    void FecharTela()
    {
        painelCadastroSugestao.SetActive(false);
    }
}


