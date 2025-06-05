using UnityEngine;
using Photon.Pun;

public class ProfessorRevelarCarta : MonoBehaviourPun
{
    public void RevelarCartaParaAlunos(Carta carta)
{
    if (!PhotonNetwork.IsMasterClient)
    {
        Debug.LogWarning("Apenas o professor (master client) pode revelar cartas.");
        return;
    }


    // Referência ao controlador de partida
        ControlePartidaProfessor professor = FindObjectOfType<ControlePartidaProfessor>();

    // Verifica se o estado atual permite revelar carta
    if (professor.estadoAtual != EstadoPartida.TurnoEquipe_RevelarCarta)
    {
        Debug.LogWarning("Não é possível revelar carta neste momento. Estado atual: " + professor.estadoAtual);
        return;
    }

    // Envia RPC para os alunos
    RPCManager.Instance.photonView.RPC("RPC_RevelarCartaParaAlunos", RpcTarget.All, carta.id, carta.categoria.ToString());

    Debug.Log($"Carta {carta.id} da categoria {carta.categoria} revelada para alunos.");

    // Agora FINALIZA o turno após a carta ser revelada
    if (professor != null)
    {
        professor.estadoAtual = EstadoPartida.FimTurno;
        professor.AtualizarUIProfessor();
        professor.EnviarEstadoParaEquipes();
    }
}
}

 