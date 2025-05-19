using UnityEngine;

public class ControleCategoriasUI : MonoBehaviour
{
    public SelecionadorCartasUI selecionadorCartasUI;

    public void OnCliqueBotaoCategoria(string categoria)
    {
        Debug.Log("Categoria clicada: " + categoria);
        selecionadorCartasUI.MostrarCartasDaCategoria(categoria);
    }
}
