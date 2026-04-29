using TMPro;
using UnityEngine;

public class OravanahaTekstiUuendaja : MonoBehaviour
{
    [SerializeField] private TMP_Text tekst;

    private void Reset()
    {
        tekst = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        if (tekst == null)
            tekst = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (OravanahaHaldur.Instance != null)
        {
            OravanahaHaldur.Instance.OravanahadMuutusid += UuendaTeksti;
            UuendaTeksti(OravanahaHaldur.Instance.Oravanahad);
        }
    }

    private void OnDisable()
    {
        if (OravanahaHaldur.Instance != null)
            OravanahaHaldur.Instance.OravanahadMuutusid -= UuendaTeksti;
    }

    private void UuendaTeksti(int summa)
    {
        if (tekst != null)
            tekst.text = summa.ToString();
    }
}