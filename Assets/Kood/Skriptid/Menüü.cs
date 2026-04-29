using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menüü : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private TextMeshProUGUI rahaTekst;
    [SerializeField] private Animator animator;

    [Header("Animatori seaded")]
    [SerializeField] private string menüüAvatudParameeter = "MenüüAvatud";

    [Header("Stseenid")]
    [SerializeField] private string avaekraaniStseen = "avaekraan";

    private bool kasMenüüOnAvatud;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        UuendaMenüüAnimatsiooni();
    }

    private void OnEnable()
    {
        if (OravanahaHaldur.Instance != null)
        {
            OravanahaHaldur.Instance.OravanahadMuutusid += UuendaRaha;
            UuendaRaha(OravanahaHaldur.Instance.Oravanahad);
        }
    }

    private void OnDisable()
    {
        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.OravanahadMuutusid -= UuendaRaha;
    }

    public void LülitaMenüü()
    {
        kasMenüüOnAvatud = !kasMenüüOnAvatud;
        UuendaMenüüAnimatsiooni();
    }

    public void SulgeMenüü()
    {
        kasMenüüOnAvatud = false;
        UuendaMenüüAnimatsiooni();
    }

    public void MineAvalehele()
    {
        Time.timeScale = 1f;

        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.LähtestaAlgrahale();

        SceneManager.LoadScene(avaekraaniStseen);
    }

    private void UuendaMenüüAnimatsiooni()
    {
        if (animator != null)
            animator.SetBool(menüüAvatudParameeter, kasMenüüOnAvatud);
    }

    private void UuendaRaha(int uusSumma)
    {
        if (rahaTekst != null)
            rahaTekst.text = uusSumma.ToString();
    }
}