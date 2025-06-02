using UnityEngine;
using UnityEngine.UI;

public class EquipeCartaSelecionada : MonoBehaviour
{
    public static EquipeCartaSelecionada Instance;

    [HideInInspector]
    public string idCartaSelecionada;

    public Button botaoExecutarProcedimento; // Arraste no Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (botaoExecutarProcedimento != null)
            botaoExecutarProcedimento.gameObject.SetActive(false); // Começa invisível
    }

    public void DefinirCartaSelecionada(string id)
    {
        idCartaSelecionada = id;
        Debug.Log($"Carta selecionada: {id}");

        // Ativa o botão de Executar se for o turno da equipe
        //if (botaoExecutarProcedimento != null)
            //botaoExecutarProcedimento.gameObject.SetActive(true);
    }

    public void ExecutarProcedimento()
    {
        if (string.IsNullOrEmpty(idCartaSelecionada))
        {
            Debug.LogWarning("Nenhuma carta selecionada!");
            return;
        }

        // Envia o ID da carta para o professor via RPC
        RPCManager.Instance.photonView.RPC("RPC_ReceberProcedimentoSelecionado", Photon.Pun.RpcTarget.MasterClient, idCartaSelecionada);

        // Atualiza o estado no professor para Explicação
        RPCManager.Instance.photonView.RPC("RPC_AvancarParaExplicacao", Photon.Pun.RpcTarget.MasterClient);

        // Desativa o botão novamente
        botaoExecutarProcedimento.gameObject.SetActive(false);
    }
}
