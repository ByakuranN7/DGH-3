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

    MarcarCartaComoRevelada(carta.categoria);

    // Agora FINALIZA o turno após a carta ser revelada
    if (professor != null)
    {
        professor.estadoAtual = EstadoPartida.FimTurno;
        professor.AtualizarUIProfessor();
        professor.EnviarEstadoParaEquipes();
    }
}







//Marcar carta visualmente com uma cor levemente verde quando ela é revelada, além de desativar seu botão para evitar revelação duplicada.
public void MarcarCartaComoRevelada(CategoriaCarta categoria)
{
    // Acha o script que tem os slots
    CarregarCartasSelecionadas carregador = FindObjectOfType<CarregarCartasSelecionadas>();
    if (carregador == null)
    {
        Debug.LogWarning("Não foi possível encontrar o script CarregarCartasSelecionadas na cena.");
        return;
    }

    Transform slot = null;

    switch (categoria)
    {
        case CategoriaCarta.InvasaoInicial:
            slot = carregador.slotInvasaoInicial;
            break;
        case CategoriaCarta.ObtencaoPrivilegios:
            slot = carregador.slotObtencaoPrivilegios;
            break;
        case CategoriaCarta.Persistencia:
            slot = carregador.slotPersistencia;
            break;
        case CategoriaCarta.C2Exfiltracao:
            slot = carregador.slotC2Exfiltracao;
            break;
    }

    if (slot != null && slot.childCount > 0)
    {
        GameObject cartaGO = slot.GetChild(0).gameObject;

        var imagem = cartaGO.GetComponent<UnityEngine.UI.Image>();
        if (imagem != null)
        {
            imagem.color = new Color(0.65f, 0.96f, 0.63f); // Verde claro
        }

        var botao = cartaGO.GetComponent<UnityEngine.UI.Button>();
        if (botao != null)
        {
            botao.interactable = false; // Desativa o botão para impedir múltiplas revelações
        }
    }
}
}

 