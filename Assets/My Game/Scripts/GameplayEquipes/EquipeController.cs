using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;


public class EquipeController : MonoBehaviour
{
    public TextMeshProUGUI textoMensagemEquipe;  // Texto da UI para mensagens
    public GameObject painelRolarDado; // Painel com imagem e botão
    public TextMeshProUGUI textoDado; // Texto central que mostra o número do dado
    public Button botaoRolarDado; // Botão para rolar o dado
    public TextMeshProUGUI textoCronometroEquipe; // cronometro (controlado pelo professor)


    private int equipeId;  // ID da equipe deste jogador

    void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("equipeId", out object id))
        {
            equipeId = (int)id;
            Debug.Log($"[EquipeController] Meu equipeId é {equipeId}");
        }
        else
        {
            Debug.LogWarning("[EquipeController] equipeId não encontrado nas propriedades do jogador!");
        }

        if (textoMensagemEquipe == null) Debug.LogError("O textoMensagemEquipe não está vinculado no Inspector!");
        if (painelRolarDado != null) painelRolarDado.SetActive(false); // Esconde o painel no início
    }

    public void AtualizarCronometro(float tempoRestante)
    {
        if (textoCronometroEquipe != null)
        {
            int minutos = Mathf.FloorToInt(tempoRestante / 60);
            int segundos = Mathf.FloorToInt(tempoRestante % 60);
            textoCronometroEquipe.text = "Cronometro: " + string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }


    public void BotaoRolarDado()
    {
        int resultado = Random.Range(1, 21); // D20
        bool sucesso = resultado >= 6;

        // Atualiza o texto central do dado
        if (textoDado != null) textoDado.text = resultado.ToString();

        // Envia o resultado para o professor
        RPCManager.Instance.photonView.RPC("RPC_ResultadoDadoParaProfessor", RpcTarget.MasterClient, resultado, sucesso);

        textoMensagemEquipe.text = $"Você rolou {resultado} ({(sucesso ? "Sucesso" : "Falha")})!";
        if (botaoRolarDado != null) botaoRolarDado.gameObject.SetActive(false); //esconde o botao para o jogador n poder ficar rolando o dado varias vezes no mesmo turno
    }

    //Este metodo é chamado por RPC quando o professor envia o comando de mudança de estado
    public void AtualizarEstadoTurno(int estadoInt, int equipeAtual)
    {
        EstadoPartida estado = (EstadoPartida)estadoInt;

        if (estado == EstadoPartida.FimPartida)
        {
            textoMensagemEquipe.text = "A partida chegou ao fim. Obrigado por jogar!";
            return; // Não precisa verificar equipe já que eh o fim
        }

        if (equipeAtual == equipeId)
        {
            switch (estado)
            {
                case EstadoPartida.TurnoEquipe_Dado:
                    textoMensagemEquipe.text = "Role o dado. Resultados de 1-5 são falhas, enquanto que de 6-20 são sucesso.";
                    if (painelRolarDado != null) painelRolarDado.SetActive(true);
                    if (botaoRolarDado != null) botaoRolarDado.gameObject.SetActive(true);
                    if (textoDado != null) textoDado.text = ""; // Reseta o número do dado
                    break;

                case EstadoPartida.TurnoEquipe_EsperaComecar:
                    textoMensagemEquipe.text = "Seu turno vai começar. Aguarde o professor.";
                    break;

                case EstadoPartida.TurnoEquipe_Pergunta:
                    textoMensagemEquipe.text = "Faça uma pergunta ao professor, a fim de tirar dúvidas. Caso não tenha perguntas, peça para pular esta etapa e ir direto para a seleção de procedimento.";
                    break;

                case EstadoPartida.TurnoEquipe_SelecionarProcedimento:
                    textoMensagemEquipe.text = "Selecione o procedimento que deseja tentar executar. Clique em uma das cartas e depois em 'executar procedimento'";
                    if (EquipeCartaSelecionada.Instance != null && EquipeCartaSelecionada.Instance.botaoExecutarProcedimento != null)
                    {
                        EquipeCartaSelecionada.Instance.botaoExecutarProcedimento.gameObject.SetActive(true);
                    }
                    break;

                case EstadoPartida.TurnoEquipe_Explicacao:
                    textoMensagemEquipe.text = "Líder da rodada: Explique para o professor o motivo pelo qual a sua equipe selecionou este procedimento. Além disso, descreva que ação desejam tomar.";
                    break;

                case EstadoPartida.FimTurno:
                    textoMensagemEquipe.text = "Fim do seu turno.";
                    if (painelRolarDado != null) painelRolarDado.SetActive(false);
                    break;

                default:
                    textoMensagemEquipe.text = "Aguarde...";
                    if (painelRolarDado != null) painelRolarDado.SetActive(false);
                    break;
            }
        }
        else
        {
            textoMensagemEquipe.text = $"É o turno da Equipe {equipeAtual}. Aguarde sua vez.";
            if (painelRolarDado != null) painelRolarDado.SetActive(false);
        }
    }
}
