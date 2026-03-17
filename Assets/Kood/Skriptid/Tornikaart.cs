using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tornikaart : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Sisu")]
    public GameObject torniPrefab;
    public Sprite torniIkoon;

    [Header("Lohistamine")]
    [SerializeField] private Canvas juurCanvas;
    [SerializeField] private LayerMask maatiukiKiht;
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
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (juurCanvas == null)
        {
            var c = GetComponentInParent<Canvas>();
            if (c != null) juurCanvas = c.rootCanvas;
            else juurCanvas = FindAnyObjectByType<Canvas>()?.rootCanvas;
        }
    }

    public void SeaTorn(GameObject prefab)
    {
        torniPrefab = prefab;

        Image img = GetComponent<Image>();
        if (img == null || prefab == null) return;

        var andmed = prefab.GetComponent<TornAndmed>();
        if (andmed != null && andmed.poeIkoon != null)
        {
            img.sprite = andmed.poeIkoon;
            img.color = Color.white;
        }
        else
        {
            SpriteRenderer sr = prefab.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                img.sprite = sr.sprite;
                img.color = Color.white;
            }
        }

        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
        }
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
        if (torniPrefab == null) return false;

        Camera kaamera = Camera.main;
        if (kaamera == null) return false;

        Vector3 hiireMaailmaPos = kaamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 punkt2D = new Vector2(hiireMaailmaPos.x, hiireMaailmaPos.y);

        RaycastHit2D tabamus = Physics2D.Raycast(punkt2D, Vector2.zero, maksimaalneRayKaugus, maatiukiKiht);
        if (!tabamus.collider) return false;

        Maatükk maatükk = tabamus.collider.GetComponent<Maatükk>();
        if (maatükk == null) return false;

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

        LineRenderer lr = ulatuseEelvaade.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0f, 1f, 1f, 1f);
        lr.endColor = new Color(0f, 1f, 1f, 1f);
        lr.sortingLayerName = "Default";
        lr.sortingOrder = 9999;

        TorniUlatusEelvaade eelvaade = ulatuseEelvaade.AddComponent<TorniUlatusEelvaade>();
        eelvaade.SeaRaadius(raadius);
    }

    private void UuendaUlatuseEelvaateAsukoht()
    {
        if (ulatuseEelvaade == null)
            return;

        ulatuseEelvaade.transform.position = VõtaEelvaateMaailmaPositsioon();
    }

    private void EemaldaUlatuseEelvaade()
    {
        if (ulatuseEelvaade != null)
            Destroy(ulatuseEelvaade);
    }

    private Vector3 VõtaEelvaateMaailmaPositsioon()
    {
        Camera kaamera = Camera.main;
        if (kaamera == null)
            return Vector3.zero;

        Vector3 hiireMaailmaPos = kaamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 punkt2D = new Vector2(hiireMaailmaPos.x, hiireMaailmaPos.y);

        RaycastHit2D tabamus = Physics2D.Raycast(punkt2D, Vector2.zero, maksimaalneRayKaugus, maatiukiKiht);
        if (tabamus.collider != null)
        {
            Vector3 pos = tabamus.collider.transform.position;
            pos.z = 0f;
            return pos;
        }

        hiireMaailmaPos.z = 0f;
        return hiireMaailmaPos;
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