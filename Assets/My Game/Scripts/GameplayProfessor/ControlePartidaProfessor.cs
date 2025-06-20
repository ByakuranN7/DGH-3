using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;


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
    public Button botaoPausarCronometro;
    public Button botaoPular;
    public Button botaoAceitarExplicacao;
    public Button botaoRejeitarExplicacao;
    public Button botaoDadoRolado;
    public Button botaoRevelarCarta;
    public Button botaoNaoRevelarCarta;
    public Button botaoFimTurno;
    public Button botaoFinalizarPartida;

    // Elementos relacionados ao resultado do dado
    public int ultimoResultadoDado;
    public bool ultimoSucesso;

    //Contador de cartas reveladas, para saber que deve-se ir para o turno "FimPartida" caso for 4
    public int cartasReveladas = 0;

    //Cronometro (professor que controla)
    public TextMeshProUGUI textoCronometroProfessor;
    private float tempoCronometro = 90f; // 1 minuto e 30 segundos
    private bool cronometroAtivo = false;
    private Coroutine coroutineCronometro;




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
        ResetarCronometro();
    }

    public void BotaoComecarTurno()
    {
        if (estadoAtual != EstadoPartida.TurnoEquipe_EsperaComecar) return;

        estadoAtual = EstadoPartida.TurnoEquipe_Pergunta;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();

        // Começar o cronômetro
        if (coroutineCronometro != null)
        StopCoroutine(coroutineCronometro);
        tempoCronometro = 90f;
        coroutineCronometro = StartCoroutine(Cronometro());
    }

    public void BotaoResponder()
    {
        if (estadoAtual != EstadoPartida.TurnoEquipe_Pergunta) return;

        estadoAtual = EstadoPartida.TurnoEquipe_SelecionarProcedimento;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
        ContinuarCronometro();
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
        PausarCronometro();

    }

    public void BotaoRejeitarExplicacao()
    {

        estadoAtual = EstadoPartida.FimTurno;

        AtualizarUIProfessor();
        EnviarEstadoParaEquipes();
        ResetarCronometro();
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

    public void BotaoRevelarCarta()
{
    textoMensagemProfessor.text = "Clique em uma carta para revelá-la.";

    botaoRevelarCarta.gameObject.SetActive(false);
    botaoNaoRevelarCarta.gameObject.SetActive(false);

    // Atualiza a pontuação da equipe atual (+1 ponto) ao clicar no botão "Revelar Carta"
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

    // Muda para o novo estado!
    estadoAtual = EstadoPartida.TurnoEquipe_RevelarCarta;

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
        ResetarCronometro();
}

    public void BotaoFinalizarPartida()
{
    if (estadoAtual != EstadoPartida.FimPartida) return;

    RPCManager.Instance.photonView.RPC("RPC_FinalizarPartidaParaTodos", RpcTarget.All);
}


    #endregion





//*****************************************************************************CRONOMETRO*******************************************************************
    //formata o tempo do cronometro para minutos e segundos
    private string FormatTempo(float tempo)
{
    int minutos = Mathf.FloorToInt(tempo / 60);
    int segundos = Mathf.FloorToInt(tempo % 60);
    return string.Format("{0:00}:{1:00}", minutos, segundos);
}

    //Coroutine que faz o cronometro contar e enviar o valor para todos
    private IEnumerator Cronometro()
{
    cronometroAtivo = true;

    while (tempoCronometro > 0 && cronometroAtivo)
    {
        textoCronometroProfessor.text = "Cronometro: " + FormatTempo(tempoCronometro);
        RPCManager.Instance.photonView.RPC("RPC_AtualizarCronometro", RpcTarget.Others, tempoCronometro);

        yield return new WaitForSeconds(1f);

        tempoCronometro -= 1f;
    }

    if (tempoCronometro <= 0 && cronometroAtivo)
    {
        textoCronometroProfessor.text = "Cronometro: 00:00";
        // Tempo acabou, chama rejeitar explicação pq essa funcao manda pro fim do turno
        BotaoRejeitarExplicacao();
    }
}

    private void ResetarCronometro()
{
    // Para o cronômetro se estiver rodando
    if (coroutineCronometro != null)
    {
        StopCoroutine(coroutineCronometro);
        coroutineCronometro = null;
    }
    tempoCronometro = 90f;
    textoCronometroProfessor.text = "Cronometro: " + FormatTempo(tempoCronometro);
    RPCManager.Instance.photonView.RPC("RPC_AtualizarCronometro", RpcTarget.Others, tempoCronometro);
}

    public void PausarCronometro()
{
    cronometroAtivo = false;

    if (coroutineCronometro != null)
    {
        StopCoroutine(coroutineCronometro);
        coroutineCronometro = null;
    }

    textoCronometroProfessor.text = "Cronometro: " + FormatTempo(tempoCronometro);
    RPCManager.Instance.photonView.RPC("RPC_AtualizarCronometro", RpcTarget.Others, tempoCronometro);
    botaoPausarCronometro.gameObject.SetActive(false); //esconde o botao apos clicar para pausar
}

public void ContinuarCronometro()
{
    if (coroutineCronometro != null)
        StopCoroutine(coroutineCronometro);

    cronometroAtivo = true;
    coroutineCronometro = StartCoroutine(Cronometro());
    textoCronometroProfessor.text = "Cronometro: " + FormatTempo(tempoCronometro);;
    RPCManager.Instance.photonView.RPC("RPC_AtualizarCronometro", RpcTarget.Others, tempoCronometro);
}





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
        botaoPausarCronometro.gameObject.SetActive(false);
        botaoPular.gameObject.SetActive(false);
        botaoAceitarExplicacao.gameObject.SetActive(false);
        botaoRejeitarExplicacao.gameObject.SetActive(false);
        botaoDadoRolado.gameObject.SetActive(false);
        botaoRevelarCarta.gameObject.SetActive(false);
        botaoNaoRevelarCarta.gameObject.SetActive(false);
        botaoFimTurno.gameObject.SetActive(false);
        botaoFinalizarPartida.gameObject.SetActive(false);



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
                textoMensagemProfessor.text = $"Pergunte se a equipe {equipeAtual} deseja realizar uma pergunta. \n\nClique em 'Pausar Cronometro' quando for responder.\n\nClique em 'respondido' quando tiver terminado de responder para liberar a seleção de procedimento";
                botaoResponder.gameObject.SetActive(true);
                botaoPausarCronometro.gameObject.SetActive(true);
                botaoPular.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_SelecionarProcedimento:
                textoMensagemProfessor.text = $"Equipe {equipeAtual} está selecionando um procedimento.\n\n O procedimento selecionado aparecerá a direita da tela na etapa seguinte.";
                break;

            case EstadoPartida.TurnoEquipe_Explicacao:
                string integrantes = ObterIntegrantesDaEquipe(equipeAtual);
                textoMensagemProfessor.text = $"Equipe {equipeAtual}:\n{integrantes}\n\nInforme em voz alta o líder da rodada escolhido por você. O lider é responsável por explicar o motivo da escolha do procedimento e descrever a ação que a equipe deseja realizar.\n\nClique em 'Aceitar' se a explicação do líder for satisfatória.";
                botaoAceitarExplicacao.gameObject.SetActive(true);
                botaoRejeitarExplicacao.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_Dado:
                textoMensagemProfessor.text = $"Fale para a Equipe {equipeAtual} rolar o dado. Confirme quando o resultado aparecer em sua tela.";
                //botaoDadoRolado.gameObject.SetActive(true); //o botão é ativado após receber o resultado por RPC da equipe, em ReceberResultadoDado
                break;

            case EstadoPartida.TurnoEquipe_Pontuar:
                textoMensagemProfessor.text = $"Baseando-se na explicação do líder da rodada, levando em consideração a narrativa e o contexto da situação apresentada, deseja revelar uma carta?\n\nLembrete: Caso o procedimento revelaria uma carta, mas a explicação do líder não descreveu uma ação que, na prática, levaria uma pessoa a obter o resultado esperado, não revele a carta. O 'Detectada por:' é apenas uma sugestão";
                botaoRevelarCarta.gameObject.SetActive(true);
                botaoNaoRevelarCarta.gameObject.SetActive(true);
                break;

            case EstadoPartida.TurnoEquipe_RevelarCarta:
                textoMensagemProfessor.text = $"Clique em uma das 4 cartas da narrativa para revelá-la. \n\nJustifique o motivo pelo qual revelou a carta, interligando isso com a narrativa e com as ações tomadas pela equipe.";
                // Aqui não mostra botões, porque o clique é nas cartas.
                break;

            case EstadoPartida.FimTurno:
                textoMensagemProfessor.text = $"Turno da Equipe {equipeAtual} finalizado.\nClique para começar o próximo turno.";
                botaoFimTurno.gameObject.SetActive(true);
                break;

            case EstadoPartida.FimPartida:
                textoMensagemProfessor.text = "A partida chegou ao fim! Reveja com os alunos como foi o jogo, discuta as decisões tomadas e o que aprenderam.";
                botaoFinalizarPartida.gameObject.SetActive(true);
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
 
