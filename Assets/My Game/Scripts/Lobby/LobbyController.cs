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

        // Inicializa a pontuação para todas as equipes (menos o professor)
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient) continue;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "pontosEquipe", 0 }
            };
        player.SetCustomProperties(props);
        }

    // Chama o método do RPCManager para iniciar a partida
    RPCManager.Instance.IniciarPartida();
    }  
     
}



