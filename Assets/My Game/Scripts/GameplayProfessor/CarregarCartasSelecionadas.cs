using UnityEngine;
using UnityEngine.UI;

public class CarregarCartasSelecionadas : MonoBehaviour
{
    public GameObject cartaPrefab;

    [Header("Locais onde as cartas aparecerão")]
    public Transform slotInvasaoInicial;
    public Transform slotObtencaoPrivilegios;
    public Transform slotPersistencia;
    public Transform slotC2Exfiltracao;

    void Start()
    {
        CarregarCartas();
    }

    void CarregarCartas()
    {
        var manager = CartaManager.Instance;

        if (manager.cartaInvasaoInicial != null)
            InstanciarCarta(manager.cartaInvasaoInicial, slotInvasaoInicial);

        if (manager.cartaObtencaoPrivilegios != null)
            InstanciarCarta(manager.cartaObtencaoPrivilegios, slotObtencaoPrivilegios);

        if (manager.cartaPersistencia != null)
            InstanciarCarta(manager.cartaPersistencia, slotPersistencia);

        if (manager.cartaC2Exfiltracao != null)
            InstanciarCarta(manager.cartaC2Exfiltracao, slotC2Exfiltracao);
    }

    void InstanciarCarta(Carta carta, Transform destino)
{
    GameObject novaCarta = Instantiate(cartaPrefab, destino);
    CartaVisual cartaVisual = novaCarta.GetComponent<CartaVisual>();

    if (cartaVisual != null)
    {
        cartaVisual.Configurar(carta);
    }

    // Pega o botão para adicionar o clique
    UnityEngine.UI.Button btn = novaCarta.GetComponent<UnityEngine.UI.Button>();
    if (btn != null)
    {
        ProfessorRevelarCarta professorScript = FindObjectOfType<ProfessorRevelarCarta>();
        if (professorScript != null)
        {
            btn.onClick.AddListener(() =>
            {
                professorScript.RevelarCartaParaAlunos(carta);
            });
        }
    }
}


        
    
}
