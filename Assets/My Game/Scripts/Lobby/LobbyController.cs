using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LobbyController : MonoBehaviourPunCallbacks
{
    public GameObject btnIniciarPartida; // Referência ao botão (objeto GameObject)

    void Start()
    {
        // Só ativa o botão para o professor (MasterClient)
        btnIniciarPartida.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void OnClickIniciarPartida()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("RPC_IniciarPartida", RpcTarget.All);
    }

    [PunRPC]
    void RPC_IniciarPartida()
    {
        if (PhotonNetwork.IsMasterClient)
            SceneManager.LoadScene("SelecaoCartasMestre"); // Tela professor
        else
            SceneManager.LoadScene("GameplayEquipes"); // Tela equipes
    }
}


