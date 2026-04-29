using UnityEngine;

public class LeveliAlguseHaldur : MonoBehaviour
{
    [Header("Viited")]
    [SerializeField] private JutustuseHaldur jutustuseHaldur;
    [SerializeField] private ÕpetuseHaldur õpetuseHaldur;

    private bool õpetusJubaKäivitatud;

    private void OnEnable()
    {
        if (jutustuseHaldur != null)
            jutustuseHaldur.JutustusLõppes.AddListener(KäivitaÕpetus);
    }

    private void OnDisable()
    {
        if (jutustuseHaldur != null)
            jutustuseHaldur.JutustusLõppes.RemoveListener(KäivitaÕpetus);
    }

    private void KäivitaÕpetus()
    {
        if (õpetusJubaKäivitatud)
            return;

        õpetusJubaKäivitatud = true;

        if (õpetuseHaldur != null)
            õpetuseHaldur.KäivitaÕpetus();
    }
}