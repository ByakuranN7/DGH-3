using System.Collections.Generic;
using UnityEngine;

public class SugestoesUI : MonoBehaviour
{
    public GameObject sugestaoItemPrefab; // seu prefab com 4 cartas + texto
    public Transform sugestoesContainer;  // onde vão os itens na UI

    private Rest_Controller restController;

    void Start()
    {
        restController = FindObjectOfType<Rest_Controller>();

        if (restController == null)
        {
            Debug.LogError("Rest_Controller não encontrado na cena!");
            return;
        }

        // Pega sugestões do servidor e cria os itens na UI
        restController.GetSugestoes(OnSugestoesRecebidas);
    }

    private void OnSugestoesRecebidas(List<SugestaoData> sugestoes)
    {
        if (sugestoes == null || sugestoes.Count == 0)
        {
            Debug.Log("Nenhuma sugestão recebida.");
            return;
        }

        // Limpa itens antigos se houver
        foreach (Transform child in sugestoesContainer)
        {
            Destroy(child.gameObject);
        }

        // Instancia um item para cada sugestão
        foreach (SugestaoData sugestao in sugestoes)
        {
            GameObject itemGO = Instantiate(sugestaoItemPrefab, sugestoesContainer);
            SugestaoItem itemScript = itemGO.GetComponent<SugestaoItem>();

            if (itemScript != null)
            {
                itemScript.Configurar(sugestao);
            }
            else
            {
                Debug.LogError("SugestaoItem script não encontrado no prefab!");
            }
        }
    }
    //metodo utilizado ao clicar em "registrar
    public void AtualizarSugestoes()
{
    if (restController == null)
    {
        restController = FindObjectOfType<Rest_Controller>();
        if (restController == null)
        {
            Debug.LogError("Rest_Controller não encontrado!");
            return;
        }
    }

    restController.GetSugestoes(OnSugestoesRecebidas);
}

}
