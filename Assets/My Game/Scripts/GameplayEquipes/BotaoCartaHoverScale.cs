using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BotaoCartaHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float escalaAoEntrar = 1.4f; // escala alvo (40% maior)
    public float duracaoTransicao = 0.2f; // tempo da animação em segundos

    private Vector3 escalaOriginal;
    private Coroutine escalaCoroutine;
    private int ordemOriginal;

    private void Awake()
    {
        escalaOriginal = transform.localScale;
        ordemOriginal = transform.GetSiblingIndex(); // salva a ordem original
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IniciarTransicaoEscala(escalaAoEntrar);
        transform.SetAsLastSibling(); // manda pro topo
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IniciarTransicaoEscala(1f); // volta à escala original
        transform.SetSiblingIndex(ordemOriginal); // volta à ordem original (opcional)
    }

    private void IniciarTransicaoEscala(float escalaAlvo)
    {
        if (escalaCoroutine != null)
            StopCoroutine(escalaCoroutine);

        escalaCoroutine = StartCoroutine(TransicaoEscala(escalaAlvo));
    }

    private IEnumerator TransicaoEscala(float escalaAlvo)
    {
        Vector3 escalaInicial = transform.localScale;
        Vector3 escalaFinal = escalaOriginal * escalaAlvo;
        float tempo = 0f;

        while (tempo < duracaoTransicao)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / duracaoTransicao);
            transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            yield return null;
        }

        transform.localScale = escalaFinal;
        escalaCoroutine = null;
    }
}
