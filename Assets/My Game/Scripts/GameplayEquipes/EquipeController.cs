using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class EquipeController : MonoBehaviour
{
    public TextMeshProUGUI textoMensagemEquipe;  // Referência ao texto da UI para mensagens

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
                    textoMensagemEquipe.text = "Selecione o procedimento que deseja tentar executar.";
                    break;

                case EstadoPartida.TurnoEquipe_Explicacao:
                    textoMensagemEquipe.text = "Líder da rodada: Explique para o professor o motivo pelo qual a sua equipe selecionou este procedimento. Além disso, explique exatamente o que queriam fazer com o procedimento.";
                    break;

                case EstadoPartida.TurnoEquipe_Dado:
                    textoMensagemEquipe.text = "Role o dado. Resultados de 1-5 são falhas, enquanto que resultados de 6-20 são sucesso na execução";
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
        }
    }
}

