using UnityEngine;
using System;

[Serializable]
public class VaenlasteTõenäosused
{
    public GameObject prefab;
    [Min(0f)] public float kaal = 1f;
}
