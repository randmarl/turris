using UnityEngine;
using TMPro;

public class EludUI : MonoBehaviour
{
    [Header("UI viited")]
    [SerializeField] private TMP_Text eludTekst;

    [Header("Peitmine jutustuse ajal")]
    [SerializeField] private GameObject jutustusPaneel;

    private CanvasGroup cg;
    private MängijaElud eludMgr;
    private bool onHookitud = false;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        ProoviHookida();
    }

    private void Start()
    {
        ProoviHookida();
        UuendaNähtavust();
    }

    private void OnDisable()
    {
        if (onHookitud && eludMgr != null)
        {
            eludMgr.EludMuutusid.RemoveListener(UuendaTeksti);
        }
        onHookitud = false;
        eludMgr = null;
    }

    private void Update()
    {
        if (!onHookitud) ProoviHookida();

        UuendaNähtavust();
    }

    private void ProoviHookida()
    {
        if (onHookitud) return;

        eludMgr = MängijaElud.Instance;
        if (eludMgr == null)
            eludMgr = Object.FindFirstObjectByType<MängijaElud>();

        if (eludMgr == null) return;

        eludMgr.EludMuutusid.AddListener(UuendaTeksti);
        onHookitud = true;
        UuendaTeksti(eludMgr.Elud);
    }

    private void UuendaNähtavust()
    {
        bool peida = (jutustusPaneel != null && jutustusPaneel.activeSelf);

        cg.alpha = peida ? 0f : 1f;
        cg.blocksRaycasts = !peida;
        cg.interactable = !peida;
    }

    private void UuendaTeksti(int elud)
    {
        if (eludTekst != null)
            eludTekst.text = elud.ToString();
    }
}