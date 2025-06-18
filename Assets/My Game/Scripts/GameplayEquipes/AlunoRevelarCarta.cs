using UnityEngine;
using Photon.Pun;

public class AlunoRevelarCarta : MonoBehaviourPunCallbacks
{
    public static AlunoRevelarCarta Instance { get; private set; }

    public GameObject cartaPrefab;

    public Transform slotInvasaoInicial;
    public Transform slotObtencaoPrivilegios;
    public Transform slotPersistencia;
    public Transform slotC2Exfiltracao;

    private bool invasaoInicialRevelada = false;
    private bool obtencaoPrivilegiosRevelada = false;
    private bool persistenciaRevelada = false;
    private bool c2ExfiltracaoRevelada = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Método público chamado pelo RPCManager via RPC
    public void RevelarCarta(string idCarta, string categoriaStr)
    {
        CategoriaCarta categoria = (CategoriaCarta)System.Enum.Parse(typeof(CategoriaCarta), categoriaStr);
        //esse if nao preciso mais, esta ativo como segurança para os testes
        if ((categoria == CategoriaCarta.InvasaoInicial && invasaoInicialRevelada) ||
            (categoria == CategoriaCarta.ObtencaoPrivilegios && obtencaoPrivilegiosRevelada) ||
            (categoria == CategoriaCarta.Persistencia && persistenciaRevelada) ||
            (categoria == CategoriaCarta.C2Exfiltracao && c2ExfiltracaoRevelada))
        {
            return; // Já revelada
        }

        Carta carta = CartaManager.Instance.todasAsCartas.Find(c => c.id == idCarta);
        if (carta == null)
        {
            Debug.LogWarning("Carta não encontrada pelo ID: " + idCarta);
            return;
        }

        Transform slot = null;
        switch (categoria)
        {
            case CategoriaCarta.InvasaoInicial:
                slot = slotInvasaoInicial;
                invasaoInicialRevelada = true;
                break;
            case CategoriaCarta.ObtencaoPrivilegios:
                slot = slotObtencaoPrivilegios;
                obtencaoPrivilegiosRevelada = true;
                break;
            case CategoriaCarta.Persistencia:
                slot = slotPersistencia;
                persistenciaRevelada = true;
                break;
            case CategoriaCarta.C2Exfiltracao:
                slot = slotC2Exfiltracao;
                c2ExfiltracaoRevelada = true;
                break;
        }

        if (slot != null)
        {
            GameObject cartaGO = Instantiate(cartaPrefab, slot);
            CartaVisual visual = cartaGO.GetComponent<CartaVisual>();
            if (visual != null)
            {
                visual.Configurar(carta);
            }
        }
    }
}

