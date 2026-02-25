using UnityEngine;

public class Kuul : MonoBehaviour
{
    [SerializeField] private Rigidbody2D keha2D;
    [SerializeField] private float kiirus = 10f;
    [SerializeField] private int kahju = 1;   // <-- lisa see

    private Transform sihtmärk;

    private void Awake()
    {
        if (keha2D == null) keha2D = GetComponent<Rigidbody2D>();
    }

    public void MääraSihtmärk(Transform uusSiht)
    {
        sihtmärk = uusSiht;
    }

    private void FixedUpdate()
    {
        if (sihtmärk == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 suund = ((Vector2)sihtmärk.position - keha2D.position).normalized;
        keha2D.linearVelocity = suund * kiirus;
    }

    private void OnCollisionEnter2D(Collision2D muu)
    {
        Tervis tervis = muu.gameObject.GetComponentInParent<Tervis>();
        if (tervis != null)
        {
            tervis.Kahjusta(kahju);  // <-- kasuta kahju
        }

        Destroy(gameObject);
    }
}