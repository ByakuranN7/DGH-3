using UnityEngine;
using UnityEngine.UI;

//esse script serve para salvar o ID da carta no prefab, ja que o prefab por si so nao eh capaz de fazer isso. Esse script eh interligado com o CartaPrefab na interface da unity

public class CartaVisual : MonoBehaviour
{
    public string idCarta;  // ID da carta para referência futura

    public void Configurar(Carta carta)
    {
        idCarta = carta.id;  // Guarda o ID da carta (string)
        GetComponent<Image>().sprite = carta.imagem;  // Define a imagem visível na UI
    }
}
