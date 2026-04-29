using TMPro;
using UnityEngine;

public class EludUI : MonoBehaviour
{
    [Header("UI viited")]
    [SerializeField] private TMP_Text eludTekst;

    [Header("Peitmine jutustuse ajal")]
    [SerializeField] private GameObject jutustusPaneel;

    private CanvasGroup canvasGroup;
    private MängijaElud mängijaElud;
    private bool onÜhendatud;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        ProoviÜhendadaEludega();
    }

    private void Start()
    {
        ProoviÜhendadaEludega();
        UuendaNähtavust();
    }

    private void OnDisable()
    {
        if (onÜhendatud && mängijaElud != null)
            mängijaElud.EludMuutusid.RemoveListener(UuendaTeksti);

        onÜhendatud = false;
        mängijaElud = null;
    }

    private void Update()
    {
        if (!onÜhendatud)
            ProoviÜhendadaEludega();

        UuendaNähtavust();
    }

    private void ProoviÜhendadaEludega()
    {
        if (onÜhendatud)
            return;

        mängijaElud = MängijaElud.Instance;

        if (mängijaElud == null)
            mängijaElud = FindFirstObjectByType<MängijaElud>();

        if (mängijaElud == null)
            return;

        mängijaElud.EludMuutusid.AddListener(UuendaTeksti);
        onÜhendatud = true;

        UuendaTeksti(mängijaElud.Elud);
    }

    private void UuendaNähtavust()
    {
        bool peida = jutustusPaneel != null && jutustusPaneel.activeSelf;

        canvasGroup.alpha = peida ? 0f : 1f;
        canvasGroup.blocksRaycasts = !peida;
        canvasGroup.interactable = !peida;
    }

    private void UuendaTeksti(int elud)
    {
        if (eludTekst != null)
            eludTekst.text = elud.ToString();
    }
}