using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TorniUlatusEelvaade : MonoBehaviour
{
    [SerializeField] private int segmentideArv = 64;

    private LineRenderer jooneRenderdaja;
    private float raadius = 1f;

    private void Awake()
    {
        jooneRenderdaja = GetComponent<LineRenderer>();

        SeadistaLineRenderer();
        UuendaRing();
    }

    public void SeaRaadius(float uusRaadius)
    {
        raadius = uusRaadius;
        UuendaRing();
    }

    private void SeadistaLineRenderer()
    {
        jooneRenderdaja.useWorldSpace = false;
        jooneRenderdaja.loop = true;
        jooneRenderdaja.positionCount = segmentideArv;

        jooneRenderdaja.startWidth = 0.08f;
        jooneRenderdaja.endWidth = 0.08f;

        jooneRenderdaja.material = new Material(Shader.Find("Sprites/Default"));
        jooneRenderdaja.startColor = new Color(0f, 0.4f, 0f, 0.7f);
        jooneRenderdaja.endColor = new Color(0f, 0.4f, 0f, 0.7f);

        jooneRenderdaja.sortingLayerName = "Vaenlased";
        jooneRenderdaja.sortingOrder = 9999;

        jooneRenderdaja.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        jooneRenderdaja.receiveShadows = false;
    }

    private void UuendaRing()
    {
        if (jooneRenderdaja == null)
            jooneRenderdaja = GetComponent<LineRenderer>();

        jooneRenderdaja.positionCount = segmentideArv;

        float nurgaSamm = 360f / segmentideArv;

        for (int i = 0; i < segmentideArv; i++)
        {
            float nurk = i * nurgaSamm * Mathf.Deg2Rad;
            float x = Mathf.Cos(nurk) * raadius;
            float y = Mathf.Sin(nurk) * raadius;

            jooneRenderdaja.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}