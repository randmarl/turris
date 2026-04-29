using UnityEngine;
using UnityEngine.UI;

public class JärgmiseLaineNupp : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private VaenlaseTekitaja vaenlaseTekitaja;
    [SerializeField] private Button nupp;
    [SerializeField] private GameObject nupuObjekt;

    [Header("Seaded")]
    [SerializeField] private float ilmumiseViivitusSekundites = 7f;
    [SerializeField] private float fadeKiirus = 5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (nupp == null)
            nupp = GetComponentInChildren<Button>();

        if (nupuObjekt == null)
            nupuObjekt = gameObject;

        canvasGroup = nupuObjekt.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = nupuObjekt.AddComponent<CanvasGroup>();

        PeidaKohe();
    }

    private void OnEnable()
    {
        if (nupp != null)
            nupp.onClick.AddListener(VajutatiNuppu);
    }

    private void Start()
    {
        if (vaenlaseTekitaja == null)
            vaenlaseTekitaja = FindFirstObjectByType<VaenlaseTekitaja>();
    }

    private void OnDisable()
    {
        if (nupp != null)
            nupp.onClick.RemoveListener(VajutatiNuppu);
    }

    private void Update()
    {
        if (vaenlaseTekitaja == null)
        {
            FadeOut();
            return;
        }

        bool peabNähaOlema =
            vaenlaseTekitaja.KasLaineKäib &&
            !vaenlaseTekitaja.KasOnViimaneLaine &&
            vaenlaseTekitaja.LaineKestus >= ilmumiseViivitusSekundites;

        if (peabNähaOlema)
            FadeIn();
        else
            FadeOut();
    }

    private void VajutatiNuppu()
    {
        if (vaenlaseTekitaja == null)
            return;

        vaenlaseTekitaja.KäivitaJärgmineLaineKohe();
        FadeOut();
    }

    private void FadeIn()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeKiirus * Time.deltaTime);

        bool nähtav = canvasGroup.alpha > 0.9f;
        canvasGroup.interactable = nähtav;
        canvasGroup.blocksRaycasts = nähtav;
    }

    private void FadeOut()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeKiirus * Time.deltaTime);

        bool nähtav = canvasGroup.alpha >= 0.1f;
        canvasGroup.interactable = nähtav;
        canvasGroup.blocksRaycasts = nähtav;
    }

    private void PeidaKohe()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}