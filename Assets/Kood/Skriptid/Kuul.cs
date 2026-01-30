using UnityEngine;

public class Kuul : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private Rigidbody2D keha2D;

    [Header("Atribuudid")]
    [SerializeField] private float kuuliKiirus = 5f;
    [SerializeField] private int kuuliTugevus = 1;

    private Transform sihtmärk;

    public void MääraSihtmärk(Transform _sihtmärk)
    {
        sihtmärk = _sihtmärk;
    }

    private void FixedUpdate()
    {
        if (!sihtmärk) return;
        Vector2 suund = (sihtmärk.position - transform.position).normalized;
        keha2D.linearVelocity = suund * kuuliKiirus;
    }
    private void OnCollisionEnter2D(Collision2D muu)
    {
        Tervis tervis = muu.gameObject.GetComponent<Tervis>();
        if (tervis != null)
        {
            tervis.Kahjusta(kuuliTugevus);
        }

        Destroy(gameObject);
    }
}
