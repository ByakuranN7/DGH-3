using UnityEngine;
using Photon.Pun;

public class LobbyController : MonoBehaviour
{
    public GameObject btnIniciarPartida; // Referência ao botão

    void Start()
    {
        // Só ativa o botão para o professor (MasterClient)
        btnIniciarPartida.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void OnClickIniciarPartida()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Chama o método do RPCManager para iniciar a partida
        RPCManager.Instance.IniciarPartida();
    }
}



