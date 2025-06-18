using UnityEngine;
using UnityEngine.UI;

public class BotaoCartaScript : MonoBehaviour
{
    public string idCarta; // Defina no Inspector (ex: "PROC001", "PROC002")

    private void Awake()
    {
        // Posso remover
        if (GetComponent<Button>() == null)
            Debug.LogWarning($"{gameObject.name} não tem componente Button.");
    }

    public void SelecionarCarta()
    {
        EquipeCartaSelecionada.Instance.DefinirCartaSelecionada(idCarta);
    }
}
