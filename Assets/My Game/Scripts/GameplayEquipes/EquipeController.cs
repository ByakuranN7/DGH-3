using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class EquipeController : MonoBehaviour
{
    public TextMeshProUGUI textoMensagemEquipe;  // Referência ao texto da UI para mensagens
    public Button botaoRolarDado; //botão utilizado para rolar o dado na etapa correta


    private int equipeId;  // Armazena o ID único da equipe deste jogador (vindo do Lobby)

    void Start()
    {
        // Tenta ler o equipeId salvo nas CustomProperties
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("equipeId", out object id))
        {
            equipeId = (int)id;
            Debug.Log($"[EquipeController] Meu equipeId é {equipeId}");
        }
        else
        {
            Debug.LogWarning("[EquipeController] equipeId não encontrado nas propriedades do jogador!");
        }
        if (textoMensagemEquipe == null)
    Debug.LogError("O textoMensagemEquipe não está vinculado no Inspector!");
    }



    public void BotaoRolarDado()
{
    int resultado = Random.Range(1, 21); // D20
    bool sucesso = resultado >= 6;

    // Envia o resultado para o professor
    RPCManager.Instance.photonView.RPC("RPC_ResultadoDadoParaProfessor", RpcTarget.MasterClient, resultado, sucesso);

    textoMensagemEquipe.text = $"Você rolou {resultado} ({(sucesso ? "Sucesso" : "Falha")})!";

    // Esconde o botão
    if (botaoRolarDado != null)
        botaoRolarDado.gameObject.SetActive(false);
}




    /// Este método é chamado pelo RPCManager quando o professor envia um comando de turno.
    public void AtualizarEstadoTurno(int estadoInt, int equipeAtual)
    {
        EstadoPartida estado = (EstadoPartida)estadoInt;

        if (equipeAtual == equipeId)
        {
            // É o meu turno! Exibe a mensagem certa.
            switch (estado)
            {
                case EstadoPartida.TurnoEquipe_EsperaComecar:
                    textoMensagemEquipe.text = "Seu turno vai começar. Aguarde o professor.";
                    break;

                case EstadoPartida.TurnoEquipe_Pergunta:
                    textoMensagemEquipe.text = "Faça uma pergunta ao professor, a fim de tirar Dúvidas. Caso não tenha perguntas, peça para pular esta etapa e ir direto para a seleção de procedimento.";
                    break;

                case EstadoPartida.TurnoEquipe_SelecionarProcedimento:
                    textoMensagemEquipe.text = "Selecione o procedimento que deseja tentar executar. Clique em uma das cartas e depois em 'executar procedimento'";
                    // Ativa o botão de Executar Procedimento
                    if (EquipeCartaSelecionada.Instance != null && EquipeCartaSelecionada.Instance.botaoExecutarProcedimento != null)
                    {
                        EquipeCartaSelecionada.Instance.botaoExecutarProcedimento.gameObject.SetActive(true);
                    }
                    break;

                case EstadoPartida.TurnoEquipe_Explicacao:
                    textoMensagemEquipe.text = "Lider da rodada: Explique para o professor o motivo pelo qual a sua equipe selecionou este procedimento. Além disso, descreva que ação desejam tomar.";
                    break;

                case EstadoPartida.TurnoEquipe_Dado:
                    textoMensagemEquipe.text = "Role o dado. Resultados de 1-5 são falhas, enquanto que resultados de 6-20 são sucesso na execução";
                    if (botaoRolarDado != null)
                    botaoRolarDado.gameObject.SetActive(true); //ativa o botao de rolar dado na interface
                    break;

                case EstadoPartida.FimTurno:
                    textoMensagemEquipe.text = "Fim do seu turno.";
                    break;

                default:
                    textoMensagemEquipe.text = "Aguarde...";
                    break;
            }
        }
        else
        {
            // Não é meu turno, apenas aguardo.
            textoMensagemEquipe.text = $"É o turno da Equipe {equipeAtual}. Aguarde sua vez.";
            
            // Esconde o botão para outras equipes
            if (EquipeCartaSelecionada.Instance != null && EquipeCartaSelecionada.Instance.botaoExecutarProcedimento != null)
            {
                EquipeCartaSelecionada.Instance.botaoExecutarProcedimento.gameObject.SetActive(false);
            }
        }
    }
}

