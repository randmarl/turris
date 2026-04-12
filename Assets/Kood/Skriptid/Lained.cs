using UnityEngine;
using TMPro;

public class Lained : MonoBehaviour
{
    [Header("UI viited")]
    [SerializeField] private TMP_Text lainedTekst;
    [SerializeField] private VaenlaseTekitaja vaenlaseTekitaja;

    [Header("Peitmine jutustuse ajal")]
    [SerializeField] private GameObject jutustusPaneel;

    private CanvasGroup cg;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (vaenlaseTekitaja == null)
            vaenlaseTekitaja = FindFirstObjectByType<VaenlaseTekitaja>();

        if (vaenlaseTekitaja != null)
        {
            vaenlaseTekitaja.LaineMuutus += UuendaLaineteTeksti;
            UuendaLaineteTeksti(
                vaenlaseTekitaja.PraeguneLaine,
                vaenlaseTekitaja.MaksimaalneLaineteArv
            );
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
            vaenlaseTekitaja.LaineMuutus -= UuendaLaineteTeksti;
    }

    private void UuendaLaineteTeksti(int praeguneLaine, int kokkuLaineid)
    {
        if (lainedTekst != null)
            lainedTekst.text = $"LAINED: {praeguneLaine}/{kokkuLaineid}";
    }

    private void UuendaNähtavust()
    {
        bool peida = (jutustusPaneel != null && jutustusPaneel.activeSelf);

        cg.alpha = peida ? 0f : 1f;
        cg.blocksRaycasts = !peida;
        cg.interactable = !peida;
    }
}