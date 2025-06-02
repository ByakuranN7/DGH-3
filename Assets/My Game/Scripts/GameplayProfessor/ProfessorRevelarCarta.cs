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

    // Envia RPC para os alunos
    RPCManager.Instance.photonView.RPC("RPC_RevelarCartaParaAlunos", RpcTarget.All, carta.id, carta.categoria.ToString());

    Debug.Log($"Carta {carta.id} da categoria {carta.categoria} revelada para alunos.");

    // Agora FINALIZA o turno após a carta ser revelada
    ControlePartidaProfessor professor = FindObjectOfType<ControlePartidaProfessor>();
    if (professor != null)
    {
        professor.estadoAtual = EstadoPartida.FimTurno;
        professor.AtualizarUIProfessor();
        professor.EnviarEstadoParaEquipes();
    }
}
}

 