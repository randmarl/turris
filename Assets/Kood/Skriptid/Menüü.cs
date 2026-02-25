using UnityEngine;
using TMPro;

public class Menüü : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private TextMeshProUGUI rahaTekst;
    [SerializeField] private Animator animaator;

    private bool kasMenüüOnAvatud = true;

    public void LülitaMenüü()
    {
        kasMenüüOnAvatud = !kasMenüüOnAvatud;
        animaator.SetBool("MenüüAvatud", kasMenüüOnAvatud);
    }

    private void OnEnable()
    {
        OravanahaHaldur.Instance.OravanahadMuutusid += UuendaRaha;
        UuendaRaha(OravanahaHaldur.Instance.Oravanahad);
    }

    private void OnDisable()
    {
        OravanahaHaldur.Instance.OravanahadMuutusid -= UuendaRaha;
    }

    private void UuendaRaha(int uusSumma)
    {
        rahaTekst.text = uusSumma.ToString();
    }
}
