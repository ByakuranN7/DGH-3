using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SugestaoItem : MonoBehaviour
{
    [Header("Referências do Prefab")]
    public TextMeshProUGUI descricaoText;
    public Image[] cartaImages; // Arraste os 4 Image slots do prefab aqui

    public void Configurar(SugestaoData data)
{
    descricaoText.text = data.descricao;

    string[] ids = {
        data.cartaInvasaoInicial,
        data.cartaObtencaoPrivilegios,
        data.cartaPersistencia,
        data.cartaC2Exfiltracao
    };

    for (int i = 0; i < ids.Length; i++)
    {
        var carta = CartaManager.Instance.todasAsCartas.Find(c => c.id == ids[i]);
        if (carta != null)
        {
            cartaImages[i].sprite = carta.imagem;
        }
        else
        {
            Debug.LogWarning($"Carta com ID {ids[i]} não encontrada!");
        }
    }
}

}
