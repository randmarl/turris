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

    public bool KasOnHõivatud()
    {
        return torn != null;
    }

    public bool ProoviPaigaldadaTorn(GameObject prefab)
    {
        if (prefab == null) return false;
        if (torn != null) return false;

        torn = Instantiate(prefab, transform.position, Quaternion.identity);
        return true;
    }
}