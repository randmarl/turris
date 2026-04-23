using UnityEngine;
using TMPro;
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

    private bool kasMenüüOnAvatud = false;

    private void Awake()
    {
        kasMenüüOnAvatud = false;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.SetBool(menüüAvatudParameeter, false);
    }

    public void LülitaMenüü()
    {
        kasMenüüOnAvatud = !kasMenüüOnAvatud;

        if (animator != null)
            animator.SetBool(menüüAvatudParameeter, kasMenüüOnAvatud);
    }

    public void SulgeMenüü()
    {
        kasMenüüOnAvatud = false;

        if (animator != null)
            animator.SetBool(menüüAvatudParameeter, false);
    }

    public void MineAvalehele()
    {
        Time.timeScale = 1f;

        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.LähtestaAlgrahale();

        SceneManager.LoadScene(avaekraaniStseen);
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

    private void UuendaRaha(int uusSumma)
    {
        if (rahaTekst != null)
            rahaTekst.text = uusSumma.ToString();
    }
}