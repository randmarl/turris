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

    public void SeaSisu(GameObject uusPrefab, Sprite uusIkoon)
    {
        torniPrefab = uusPrefab;
        torniIkoon = uusIkoon;
        if (torniIkoon == null && torniPrefab != null)
        {
            SpriteRenderer sr = torniPrefab.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) torniIkoon = sr.sprite;
        }

        Image img = GetComponent<Image>();
        if (img != null)
        {
            if (torniIkoon != null)
                img.sprite = torniIkoon;

            img.color = Color.white;
            img.raycastTarget = true;
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

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

        Vector3 torniPos = tabamus.collider.transform.position;
        Instantiate(torniPrefab, torniPos, Quaternion.identity);
        return true;
    }
}