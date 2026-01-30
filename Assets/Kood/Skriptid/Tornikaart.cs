using UnityEngine;
using UnityEngine.UI;

public class Tornikaart : MonoBehaviour
{
    [SerializeField] private Image ikoon;

    public GameObject TorniPrefab { get; private set; }

    private void Reset()
    {
        ikoon = GetComponent<Image>();
    }

    public void SeaSisu(GameObject torniPrefab, Sprite torniIkoon)
    {
        TorniPrefab = torniPrefab;

        if (ikoon == null) ikoon = GetComponent<Image>();
        if (ikoon != null && torniIkoon != null)
            ikoon.sprite = torniIkoon;
    }
}
