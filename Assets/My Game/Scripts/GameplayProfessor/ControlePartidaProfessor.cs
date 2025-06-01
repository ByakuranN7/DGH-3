using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ControlePartidaProfessor : MonoBehaviour
{
    // Estados e controle de equipes
    public EstadoPartida estadoAtual;
    public int equipeAtual;
    public int totalEquipes;

    // UI do professor - arraste na Unity
    public TextMeshProUGUI textoMensagemProfessor;
    public Button botaoComecarPartida;
    public Button botaoComecarTurno;
    public Button botaoResponder;
    public Button botaoPular;
    public Button botaoAceitarExplicacao;
    public Button botaoRejeitarExplicacao;
    public Button botaoDadoRolado;
    public Button botaoFimTurno;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Esse script só deve rodar no professor (MasterClient). Desabilitando...");
            this.enabled = false;
            return;
        }
    }

    void Start()
    {
        totalEquipes = PhotonNetwork.PlayerList.Length - 1; // exclui professor
        estadoAtual = EstadoPartida.NarrativaInicial;
        equipeAtual = 1;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    #region Métodos dos botões

    public void BotaoComecarPartida()
    {
        if (estadoAtual != EstadoPartida.NarrativaInicial) return;

        estadoAtual = EstadoPartida.TurnoEquipe_EsperaComecar;
        equipeAtual = 1;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    public void BotaoComecarTurno()
    {
        if (estadoAtual != EstadoPartida.TurnoEquipe_EsperaComecar) return;

        estadoAtual = EstadoPartida.TurnoEquipe_Pergunta;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    public void BotaoResponder()
    {
        if (estadoAtual != EstadoPartida.TurnoEquipe_Pergunta) return;

        estadoAtual = EstadoPartida.TurnoEquipe_SelecionarProcedimento;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    public void BotaoPular()
    {
        if (estadoAtual != EstadoPartida.TurnoEquipe_Pergunta) return;

        estadoAtual = EstadoPartida.TurnoEquipe_SelecionarProcedimento;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    public void BotaoAceitarExplicacao()
    {
        if (estadoAtual != EstadoPartida.TurnoEquipe_Explicacao) return;

        estadoAtual = EstadoPartida.TurnoEquipe_Dado;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    public void BotaoRejeitarExplicacao()
    {
        if (estadoAtual != EstadoPartida.TurnoEquipe_Explicacao) return;

        estadoAtual = EstadoPartida.FimTurno;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    public void BotaoDadoRolado()
    {
        if (estadoAtual != EstadoPartida.TurnoEquipe_Dado) return;

        estadoAtual = EstadoPartida.FimTurno;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    public void BotaoFimTurno()
    {
        if (estadoAtual != EstadoPartida.FimTurno) return;

        AvancarEquipe();

        estadoAtual = EstadoPartida.TurnoEquipe_EsperaComecar;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
    }

    #endregion

    void AvancarEquipe()
    {
        equipeAtual++;
        if (equipeAtual > totalEquipes)
            equipeAtual = 1;
    }

    void AtualizarUIProfessor()
    {
        // Desliga todos os botões inicialmente
        botaoComecarPartida.gameObject.SetActive(false);
        botaoComecarTurno.gameObject.SetActive(false);
        botaoResponder.gameObject.SetActive(false);
        botaoPular.gameObject.SetActive(false);
        botaoAceitarExplicacao.gameObject.SetActive(false);
        botaoRejeitarExplicacao.gameObject.SetActive(false);
        botaoDadoRolado.gameObject.SetActive(false);
        botaoFimTurno.gameObject.SetActive(false);

        switch (estadoAtual)
        {
            case EstadoPartida.NarrativaInicial:
                textoMensagemProfessor.text = "Clique em Começar Partida para iniciar.";
                botaoComecarPartida.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_EsperaComecar:
                textoMensagemProfessor.text = $"Turno da Equipe {equipeAtual}\nClique em Começar Turno.";
                botaoComecarTurno.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_Pergunta:
                textoMensagemProfessor.text = $"Equipe {equipeAtual} - Escolha: Responder ou Pular.";
                botaoResponder.gameObject.SetActive(true);
                botaoPular.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_SelecionarProcedimento:
                textoMensagemProfessor.text = $"Equipe {equipeAtual} está selecionando procedimento.";
                break;

            case EstadoPartida.TurnoEquipe_Explicacao:
                textoMensagemProfessor.text = $"Equipe {equipeAtual} está explicando.\nAceitar ou Rejeitar?";
                botaoAceitarExplicacao.gameObject.SetActive(true);
                botaoRejeitarExplicacao.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_Dado:
                textoMensagemProfessor.text = $"Equipe {equipeAtual}, role o dado!";
                botaoDadoRolado.gameObject.SetActive(true);
                break;

            case EstadoPartida.FimTurno:
                textoMensagemProfessor.text = $"Turno da Equipe {equipeAtual} finalizado.\nClique para próximo turno.";
                botaoFimTurno.gameObject.SetActive(true);
                break;

            case EstadoPartida.FimPartida:
                textoMensagemProfessor.text = "Partida finalizada.";
                break;
        }
    }

    void EnviarEstadoParaEquipes()
    {
        // Usa o RPCManager para enviar o estado
        RPCManager.Instance.photonView.RPC("RPC_AtualizarEstadoTurno", RpcTarget.Others, (int)estadoAtual, equipeAtual);
    }
}
 
