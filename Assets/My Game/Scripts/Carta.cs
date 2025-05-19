using UnityEngine;

public enum CategoriaCarta
{
    InvasaoInicial,
    ObtencaoPrivilegios,
    Persistencia,
    C2Exfiltracao,
    Processos
}

[CreateAssetMenu(fileName = "NovaCarta", menuName = "Cartas/Carta")]
public class Carta : ScriptableObject
{
    public string id;
    public Sprite imagem;
    public CategoriaCarta categoria;
}
