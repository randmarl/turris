using UnityEngine;
using UnityEngine.InputSystem;

public class Maatükk : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private SpriteRenderer renderdaja;
    [SerializeField] private Color hiirPealVärv;

    private Color algneVärv;
    private GameObject torn;

    private void Start()
    {
        algneVärv = renderdaja.color;
    }

    private void Update()
    {
        Vector2 hiirePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D tabatud = Physics2D.OverlapPoint(hiirePos);

        if (tabatud != null && tabatud.gameObject == gameObject)
            renderdaja.color = hiirPealVärv;
        else
            renderdaja.color = algneVärv;
    }

    private void EhitaTorn()
    {
        if (torn != null) return;

        int hind = Ehitaja.peamine.VõtaEhitusHind();

        if (!OravanahaHaldur.Instance.KulutaOravanahku(hind))
            return;

        GameObject prefab = Ehitaja.peamine.VõtaSuvalineTornPrefab();
        torn = Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
