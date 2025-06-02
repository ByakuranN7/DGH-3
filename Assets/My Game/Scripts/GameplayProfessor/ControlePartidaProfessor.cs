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
    public Button botaoRevelarCarta;
    public Button botaoNaoRevelarCarta;
    public Button botaoFimTurno;

    // Elementos relacionados ao resultado do dado
    public int ultimoResultadoDado;
    public bool ultimoSucesso;

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

        if (ultimoSucesso)
        {
        estadoAtual = EstadoPartida.TurnoEquipe_Pontuar; // Vai para Pontuar (ou outra lógica no futuro)
        }
        else
        {
        estadoAtual = EstadoPartida.FimTurno; // Falhou, fim de turno
        }

    AtualizarUIProfessor();
    EnviarEstadoParaEquipes();
}



    //public void BotaoDadoRolado()
   // {
   //     if (estadoAtual != EstadoPartida.TurnoEquipe_Dado) return;

   //     estadoAtual = EstadoPartida.FimTurno;

   //     AtualizarUIProfessor();
   //     EnviarEstadoParaEquipes();
   // }





    public void BotaoRevelarCarta()
{
    textoMensagemProfessor.text = "Clique em uma carta para revelá-la.";

    botaoRevelarCarta.gameObject.SetActive(false);
    botaoNaoRevelarCarta.gameObject.SetActive(false);

    // Atualiza a pontuação da equipe atual (+1 ponto) ao clicar no botão "revelar carta", mesmo antes de revelar a carta em si
    foreach (var player in PhotonNetwork.PlayerList)
    {
        if (player.CustomProperties.TryGetValue("equipeId", out object id) && (int)id == equipeAtual)
        {
            int pontosAtuais = 0;
            if (player.CustomProperties.TryGetValue("pontosEquipe", out object pontosObj))
            {
                pontosAtuais = (int)pontosObj;
            }

            pontosAtuais += 1; // Adiciona 1 ponto

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "pontosEquipe", pontosAtuais }
            };

            player.SetCustomProperties(props); // Atualiza o Photon
            break; // Encontrou a equipe, sai do loop
        }
    }

    // Altera o estado para FimTurno
    estadoAtual = EstadoPartida.FimTurno;

    botaoFimTurno.gameObject.SetActive(true); // Mostra o botão FimTurno

    AtualizarUIProfessor();
    EnviarEstadoParaEquipes();
}



    public void BotaoNaoRevelarCarta()
{
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

    public void AtualizarUIProfessor()
    {
        // Desliga todos os botões inicialmente
        botaoComecarPartida.gameObject.SetActive(false);
        botaoComecarTurno.gameObject.SetActive(false);
        botaoResponder.gameObject.SetActive(false);
        botaoPular.gameObject.SetActive(false);
        botaoAceitarExplicacao.gameObject.SetActive(false);
        botaoRejeitarExplicacao.gameObject.SetActive(false);
        botaoDadoRolado.gameObject.SetActive(false);
        botaoRevelarCarta.gameObject.SetActive(false);
        botaoNaoRevelarCarta.gameObject.SetActive(false);
        botaoFimTurno.gameObject.SetActive(false);



        switch (estadoAtual)
        {
            case EstadoPartida.NarrativaInicial:
                textoMensagemProfessor.text = "Conte a narrativa inicial para seus alunos. Além disso, dê um tempo para eles conversarem sobre a narrativa inicial e se prepararem para a partida.\n\n\nClique em 'Começar Partida' quando tiver dado tempo o suficiente.";
                botaoComecarPartida.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_EsperaComecar:
                textoMensagemProfessor.text = $"Turno da Equipe {equipeAtual}\n\nClique em Começar Turno.";
                botaoComecarTurno.gameObject.SetActive(true);
                ProfessorVisualizarCarta.Instance.LimparParaNovaEquipe(); //remover da tela do professor a carta de procedimento da equipe anterior
                break;

            case EstadoPartida.TurnoEquipe_Pergunta:
                textoMensagemProfessor.text = $"Pergunte se a equipe {equipeAtual} deseja realizar uma pergunta. \n\nClique em 'respondido' quando tiver terminado de responder para liberar a seleção de procedimento";
                botaoResponder.gameObject.SetActive(true);
                botaoPular.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_SelecionarProcedimento:
                textoMensagemProfessor.text = $"Equipe {equipeAtual} está selecionando um procedimento.\n\n O procedimento selecionado aparecerá na esquerda da tela na etapa seguinte.";
                break;

            case EstadoPartida.TurnoEquipe_Explicacao:
                string integrantes = ObterIntegrantesDaEquipe(equipeAtual);
                textoMensagemProfessor.text = $"Equipe {equipeAtual}:\n{integrantes}\n\nInforme em voz alta o líder da rodada, responsável por explicar o motivo da escolha do procedimento e descrever a ação que a equipe deseja realizar.\n\nClique em 'Aceitar' se a explicação for satisfatória.";
                botaoAceitarExplicacao.gameObject.SetActive(true);
                botaoRejeitarExplicacao.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_Dado:
                textoMensagemProfessor.text = $"Fale para a Equipe {equipeAtual} rolar o dado. Confirme quando o resultado aparecer em sua tela.";
                botaoDadoRolado.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_Pontuar:
                textoMensagemProfessor.text = $"Equipe {equipeAtual} teve sucesso! Deseja revelar uma carta?";
                botaoRevelarCarta.gameObject.SetActive(true);
                botaoNaoRevelarCarta.gameObject.SetActive(true);
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

    public void EnviarEstadoParaEquipes()
    {
        // Usa o RPCManager para enviar o estado
        RPCManager.Instance.photonView.RPC("RPC_AtualizarEstadoTurno", RpcTarget.Others, (int)estadoAtual, equipeAtual);
    }




    public void ReceberResultadoDado(int resultado, bool sucesso)
{
    ultimoResultadoDado = resultado;
    ultimoSucesso = sucesso;

    textoMensagemProfessor.text = $"A Equipe {equipeAtual} rolou {resultado}.\n" + (sucesso ? "SUCESSO!" : "FALHA!");

    // O botão já existe e está configurado no estado TurnoEquipe_Dado
    // Ativa ele só por garantia:
    botaoDadoRolado.gameObject.SetActive(true);
}



private string ObterIntegrantesDaEquipe(int equipeId)
{
    foreach (var player in PhotonNetwork.PlayerList)
    {
        if (player.CustomProperties.TryGetValue("equipeId", out object id) && (int)id == equipeId)
        {
            if (player.CustomProperties.TryGetValue("teamNames", out object nomesObj))
            {
                string nomesStrRaw = nomesObj as string;
                if (!string.IsNullOrEmpty(nomesStrRaw))
                {
                    string[] nomes = nomesStrRaw.Split('|');
                    return string.Join("\n", nomes); // cada nome em uma linha
                }
            }
        }
    }

    return "Nomes não encontrados";
}

}
 
