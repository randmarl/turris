using UnityEngine;
using TMPro;

public class OravanahaTekstiUuendaja : MonoBehaviour
{
    [SerializeField] private TMP_Text tekst;
    private void Reset()
    {
        tekst = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        if (tekst == null) tekst = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (tekst == null) return;
        if (OravanahaHaldur.Instance == null) return;

        tekst.text = OravanahaHaldur.Instance.Oravanahad.ToString();
    }
}