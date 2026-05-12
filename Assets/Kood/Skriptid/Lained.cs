using TMPro;
using UnityEngine;

public class Lained : MonoBehaviour
{
    [Header("UI viited")]
    [SerializeField] private TMP_Text lainedTekst;
    [SerializeField] private VaenlaseTekitaja vaenlaseTekitaja;

    [Header("Peitmine jutustuse ajal")]
    [SerializeField] private GameObject jutustuspaneel;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (vaenlaseTekitaja == null)
            vaenlaseTekitaja = FindFirstObjectByType<VaenlaseTekitaja>();

        if (vaenlaseTekitaja != null)
        {
            // kuulab laine numbri muutust
            vaenlaseTekitaja.LaineMuutus += UuendaLaineteTeksti;

            UuendaLaineteTeksti(vaenlaseTekitaja.PraeguneLaine, vaenlaseTekitaja.MaksimaalneLaineteArv);
        }

        UuendaNähtavust();
    }

    private void Update()
    {
        UuendaNähtavust();
    }

    private void OnDestroy()
    {
        if (vaenlaseTekitaja != null)
            // lõpetab laine muutuse kuulamise
            vaenlaseTekitaja.LaineMuutus -= UuendaLaineteTeksti;
    }

    private void UuendaLaineteTeksti(int praeguneLaine, int kokkuLaineid)
    {
        if (lainedTekst != null)
            lainedTekst.text = $"LAINED: {praeguneLaine}/{kokkuLaineid}";
    }

    private void UuendaNähtavust()
    {
        bool peida = jutustuspaneel != null && jutustuspaneel.activeSelf;

        // ui peitu jutustuse ajaks
        canvasGroup.alpha = peida ? 0f : 1f;
        canvasGroup.blocksRaycasts = !peida;
        canvasGroup.interactable = !peida;
    }
}