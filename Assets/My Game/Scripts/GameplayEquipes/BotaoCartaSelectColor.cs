using UnityEngine;
using UnityEngine.UI;

public class BotaoCartaSelectColor : MonoBehaviour
{
    public Image fundoCarta; // Defina no Inspector

    private static BotaoCartaSelectColor cartaSelecionada;

    private Color corSelecionada = new Color(0.65f, 0.96f, 0.63f);
    private Color corNormal = Color.white;

    private void Awake()
    {
        if (fundoCarta != null)
            fundoCarta.color = corNormal;
    }

    public void MarcarSelecionado()
    {
        if (cartaSelecionada != null && cartaSelecionada != this)
        {
            if (cartaSelecionada.fundoCarta != null)
                cartaSelecionada.fundoCarta.color = corNormal;
        }

        if (fundoCarta != null)
            fundoCarta.color = corSelecionada;

        cartaSelecionada = this;
    }
}
