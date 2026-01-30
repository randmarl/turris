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

//        if (tabatud != null && tabatud.gameObject == gameObject && Mouse.current.leftButton.wasPressedThisFrame)
//        {
//            EhitaTorn();
//        }
    }

    private void EhitaTorn()
    {
        if (torn != null) return;

        Torn tornEhitatav = Ehitaja.peamine.VõtaValitudTorn();

        if (tornEhitatav.hind > haldur.peamine.raha)
        {
            Debug.Log("Sa oled liiga vaene!");
            return;
        }

        haldur.peamine.KulutaRaha(tornEhitatav.hind);

        torn = Instantiate(tornEhitatav.prefab, transform.position, Quaternion.identity);
    }
}
