using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;

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






//recebe a carta de procedimento selecionada e atualiza os estados/UI de todos
[PunRPC]
public void RPC_ReceberProcedimentoSelecionado(string idCarta)
{
    Debug.Log($"[RPCManager] Recebi ID do procedimento: {idCarta}");

    // Aqui o professor faz algo com o ID recebido
    ProfessorVisualizarCarta.Instance.MostrarCartaSelecionada(idCarta);
}

[PunRPC]
public void RPC_AvancarParaExplicacao()
{
    ControlePartidaProfessor controle = FindObjectOfType<ControlePartidaProfessor>();
    if (controle != null)
    {
        controle.estadoAtual = EstadoPartida.TurnoEquipe_Explicacao;
        controle.AtualizarUIProfessor();
        controle.EnviarEstadoParaEquipes();
    }
}

//método de enviar o resultado do dado para o professor
[PunRPC]
public void RPC_ResultadoDadoParaProfessor(int resultado, bool sucesso)
{
    ControlePartidaProfessor controle = FindObjectOfType<ControlePartidaProfessor>();
    if (controle != null)
    {
        controle.ReceberResultadoDado(resultado, sucesso);
    }
}


//Sincronizar cronometro
[PunRPC]
public void RPC_AtualizarCronometro(float tempoRestante)
{
    EquipeController equipe = FindObjectOfType<EquipeController>();
    if (equipe != null)
    {
        equipe.AtualizarCronometro(tempoRestante);
    }
}


//Termina a partida para todos, ao professor clicar em "Finalizar partida" quando chega no estado FimPartida
[PunRPC]
public void RPC_FinalizarPartidaParaTodos()
{
    StartCoroutine(SairEDestruir());
}

private IEnumerator SairEDestruir()
{
    PhotonNetwork.LeaveRoom();

    // Espera até sair da sala completamente
    while (PhotonNetwork.InRoom)
        yield return null;

    // Agora é seguro carregar a cena e destruir o objeto
    SceneManager.LoadScene("MenuInicial");
    Destroy(RPCManager.Instance.gameObject);
}


}
