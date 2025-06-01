using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RPCManager : MonoBehaviourPun
{
    public static RPCManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }










    // RPC para revelar carta para os alunos
    [PunRPC]
    public void RPC_RevelarCartaParaAlunos(string idCarta, string categoria)
    {
        if (AlunoRevelarCarta.Instance != null)
        {
            AlunoRevelarCarta.Instance.RevelarCarta(idCarta, categoria);
        }
        else
        {
            Debug.LogWarning("AlunoRevelarCarta.Instance não está setado.");
        }
    }

    // Método público para iniciar a partida, chamado pelo LobbyController
    public void IniciarPartida()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("RPC_IniciarPartida", RpcTarget.All);
    }

    //Método público para enviar os jogadores para a cena correta pós lobby (atrelado ao botão de começar partida do lobby, acessivel apenas pelo professor)
    [PunRPC]
    void RPC_IniciarPartida()
    {
        if (PhotonNetwork.IsMasterClient)
            SceneManager.LoadScene("SelecaoCartasMestre"); // Tela do professor
        else
            SceneManager.LoadScene("GameplayEquipes"); // Tela dos alunos
    }









//envia a info do estado atual do turno e de quem é o turno
[PunRPC]
public void RPC_AtualizarEstadoTurno(int estadoInt, int equipeAtual)
{
    Debug.Log($"[RPCManager] Recebi RPC_AtualizarEstadoTurno: Estado {estadoInt}, EquipeAtual {equipeAtual}");
    EquipeController equipeCtrl = FindObjectOfType<EquipeController>();
    if (equipeCtrl != null)
    {
        equipeCtrl.AtualizarEstadoTurno(estadoInt, equipeAtual);
    }
    else
    {
        Debug.LogWarning("[RPCManager] Nenhum EquipeController encontrado!");
    }
}



}
