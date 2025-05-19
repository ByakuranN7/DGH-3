using UnityEngine;
using System.Collections.Generic;

public class CartaManager : MonoBehaviour
{
    public static CartaManager Instance;

    [Header("Cartas disponíveis")]
    public List<Carta> todasAsCartas;

    [Header("Cartas selecionadas")]
    public Carta cartaInvasaoInicial;
    public Carta cartaObtencaoPrivilegios;
    public Carta cartaPersistencia;
    public Carta cartaC2Exfiltracao;

    void Awake()
{

    // Singleton (acesso global ao manager)
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // <- mantem salvo eu acho
    }
    else
    {
        Destroy(gameObject);
    }
}

    public List<Carta> ObterCartasPorCategoria(CategoriaCarta categoria)
    {
        return todasAsCartas.FindAll(carta => carta.categoria == categoria);
    }

    public void SalvarCartaSelecionada(Carta carta)
    {
        switch (carta.categoria)
        {
            case CategoriaCarta.InvasaoInicial:
                cartaInvasaoInicial = carta;
                break;
            case CategoriaCarta.ObtencaoPrivilegios:
                cartaObtencaoPrivilegios = carta;
                break;
            case CategoriaCarta.Persistencia:
                cartaPersistencia = carta;
                break;
            case CategoriaCarta.C2Exfiltracao:
                cartaC2Exfiltracao = carta;
                break;
        }
    }
}
