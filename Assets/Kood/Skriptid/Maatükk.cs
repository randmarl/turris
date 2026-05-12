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
        if (renderdaja != null)
            algneVärv = renderdaja.color;
    }

    private void Update()
    {
        if (renderdaja == null || Camera.main == null)
            return;

        // hiire asukoht mängumaailmas
        Vector2 hiirePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // kontrollib, kas hiir on selle maatüki peal
        Collider2D tabatud = Physics2D.OverlapPoint(hiirePos);

        bool hiirPeal = tabatud != null && tabatud.gameObject == gameObject;
        renderdaja.color = hiirPeal ? hiirPealVärv : algneVärv;
    }

    public bool KasOnHõivatud()
    {
        return torn != null;
    }

    public bool ProoviPaigaldadaTorn(GameObject prefab)
    {
        if (prefab == null || torn != null)
            return false;

        // paneb torni sellele maatükile
        torn = Instantiate(prefab, transform.position, Quaternion.identity);
        return true;
    }
}