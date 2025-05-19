using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BotaoAvancar : MonoBehaviour
{
    public Button botaoAvancar;
    public TextMeshProUGUI mensagemErroTMP;
    public float tempoErroVisivel = 3f;

    private void Start()
    {
        botaoAvancar.onClick.AddListener(VerificarOuAvancar);
        if (mensagemErroTMP != null)
            mensagemErroTMP.gameObject.SetActive(false); // Garante que comece invisível
    }

    void VerificarOuAvancar()
    {
        var cm = CartaManager.Instance;

        bool todasSelecionadas = 
            cm.cartaInvasaoInicial != null &&
            cm.cartaObtencaoPrivilegios != null &&
            cm.cartaPersistencia != null &&
            cm.cartaC2Exfiltracao != null;

        if (todasSelecionadas)
        {
            SceneManager.LoadScene("GameplayProfessor");
        }
        else
        {
            MostrarMensagemErro("Você precisa selecionar uma carta de cada categoria!");
        }
    }

    void MostrarMensagemErro(string texto)
    {
        if (mensagemErroTMP != null)
        {
            mensagemErroTMP.text = texto;
            mensagemErroTMP.gameObject.SetActive(true);
            CancelInvoke(nameof(EsconderMensagem));
            Invoke(nameof(EsconderMensagem), tempoErroVisivel);
        }
    }

    void EsconderMensagem()
    {
        if (mensagemErroTMP != null)
        {
            mensagemErroTMP.gameObject.SetActive(false);
        }
    }
}
