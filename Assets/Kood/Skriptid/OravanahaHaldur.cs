using UnityEngine;
using System;

public class OravanahaHaldur : MonoBehaviour
{
    public static OravanahaHaldur Instance { get; private set; }

    [Header("Sätted")]
    [SerializeField] private int algusOravanahad = 100;

    public int Oravanahad { get; private set; }
    public event Action<int> OravanahadMuutusid;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Oravanahad = algusOravanahad;
        OravanahadMuutusid?.Invoke(Oravanahad);
    }

    public bool KasSaabKulutada(int kogus)
    {
        return Oravanahad >= kogus;
    }

    public bool KulutaOravanahku(int kogus)
    {
        if (!KasSaabKulutada(kogus)) return false;

        Oravanahad -= kogus;
        OravanahadMuutusid?.Invoke(Oravanahad);
        return true;
    }

    public void LisaOravanahku(int kogus)
    {
        Oravanahad += kogus;
        OravanahadMuutusid?.Invoke(Oravanahad);
    }

    public void SeaOravanahad(int uusSumma)
    {
        Oravanahad = Mathf.Max(0, uusSumma);
        OravanahadMuutusid?.Invoke(Oravanahad);
    }
}