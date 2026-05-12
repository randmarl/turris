using System;
using UnityEngine;

[Serializable]
public class VaenlasteTõenäosused
{
    [SerializeField] private GameObject prefab;
    [SerializeField, Min(0f)] private float kaal = 1f;

    // vaenlase objekt ja selle valiku kaal
    public GameObject Prefab => prefab;
    public float Kaal => kaal;
}