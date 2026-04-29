using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tornikaart : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Sisu")]
    [SerializeField] private GameObject torniPrefab;
    [SerializeField] private Sprite torniIkoon;

    [Header("Lohistamine")]
    [SerializeField] private Canvas juurCanvas;
    [SerializeField] private LayerMask maatükiKiht;
    [SerializeField] private float maksimaalneRayKaugus = 100f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform algneVanem;
    private Vector2 algneAsukoht;
    private GameObject ulatuseEelvaade;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (juurCanvas == null)
            juurCanvas = GetComponentInParent<Canvas>()?.rootCanvas ?? FindAnyObjectByType<Canvas>()?.rootCanvas;
    }

    public void SeaTorn(GameObject prefab)
    {
        torniPrefab = prefab;

        Image pilt = GetComponent<Image>();

        if (pilt == null || torniPrefab == null)
            return;

        Sprite ikoon = VõtaTorniIkoon(torniPrefab);

        if (ikoon == null)
            return;

        torniIkoon = ikoon;
        pilt.sprite = torniIkoon;
        pilt.color = Color.white;

        VenitaKaartTäisSuurusesse();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (juurCanvas == null)
            juurCanvas = GetComponentInParent<Canvas>()?.rootCanvas ?? FindAnyObjectByType<Canvas>()?.rootCanvas;

        algneVanem = transform.parent;
        algneAsukoht = rectTransform.anchoredPosition;

        transform.SetParent(juurCanvas.transform, true);
        canvasGroup.blocksRaycasts = false;

        LooUlatuseEelvaade();
        UuendaUlatuseEelvaateAsukoht();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
        UuendaUlatuseEelvaateAsukoht();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        EemaldaUlatuseEelvaade();

        if (ProoviPaigaldadaMängulauale())
        {
            Destroy(gameObject);
            return;
        }

        transform.SetParent(algneVanem, true);
        rectTransform.anchoredPosition = algneAsukoht;
    }

    private bool ProoviPaigaldadaMängulauale()
    {
        if (torniPrefab == null || Camera.main == null)
            return false;

        Vector2 punkt = VõtaHiireMaailmaPunkt();
        RaycastHit2D tabamus = Physics2D.Raycast(punkt, Vector2.zero, maksimaalneRayKaugus, maatükiKiht);

        if (tabamus.collider == null)
            return false;

        Maatükk maatükk = tabamus.collider.GetComponent<Maatükk>();

        if (maatükk == null)
            return false;

        return maatükk.ProoviPaigaldadaTorn(torniPrefab);
    }

    private void LooUlatuseEelvaade()
    {
        if (torniPrefab == null || ulatuseEelvaade != null)
            return;

        float raadius = VõtaTorniRaadius();

        if (raadius <= 0f)
            return;

        ulatuseEelvaade = new GameObject("TorniUlatusEelvaade");
        ulatuseEelvaade.transform.position = VõtaEelvaateMaailmaPositsioon();

        LineRenderer lineRenderer = ulatuseEelvaade.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 9999;

        TorniUlatusEelvaade eelvaade = ulatuseEelvaade.AddComponent<TorniUlatusEelvaade>();
        eelvaade.SeaRaadius(raadius);
    }

    private void UuendaUlatuseEelvaateAsukoht()
    {
        if (ulatuseEelvaade != null)
            ulatuseEelvaade.transform.position = VõtaEelvaateMaailmaPositsioon();
    }

    private void EemaldaUlatuseEelvaade()
    {
        if (ulatuseEelvaade != null)
            Destroy(ulatuseEelvaade);
    }

    private Vector3 VõtaEelvaateMaailmaPositsioon()
    {
        Vector2 punkt = VõtaHiireMaailmaPunkt();
        RaycastHit2D tabamus = Physics2D.Raycast(punkt, Vector2.zero, maksimaalneRayKaugus, maatükiKiht);

        if (tabamus.collider != null)
        {
            Vector3 positsioon = tabamus.collider.transform.position;
            positsioon.z = 0f;
            return positsioon;
        }

        return punkt;
    }

    private Vector2 VõtaHiireMaailmaPunkt()
    {
        if (Camera.main == null)
            return Vector2.zero;

        Vector3 hiireMaailmaPositsioon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(hiireMaailmaPositsioon.x, hiireMaailmaPositsioon.y);
    }

    private Sprite VõtaTorniIkoon(GameObject prefab)
    {
        TornAndmed andmed = prefab.GetComponent<TornAndmed>();

        if (andmed != null && andmed.PoeIkoon != null)
            return andmed.PoeIkoon;

        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        return spriteRenderer != null ? spriteRenderer.sprite : null;
    }

    private void VenitaKaartTäisSuurusesse()
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

    private float VõtaTorniRaadius()
    {
        if (torniPrefab == null)
            return 0f;

        Kahur kahur = torniPrefab.GetComponent<Kahur>();

        if (kahur != null)
            return kahur.VõtaSihtimisRaadius();

        AeglustavTorn aeglustavTorn = torniPrefab.GetComponent<AeglustavTorn>();

        if (aeglustavTorn != null)
            return aeglustavTorn.VõtaSihtimisRaadius();

        return 0f;
    }
}