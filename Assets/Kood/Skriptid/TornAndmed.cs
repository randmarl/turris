using UnityEngine;

public class TornAndmed : MonoBehaviour
{
    [SerializeField] private Sprite poeIkoon;

    // võimaldab ikooni teistest skriptidest kasutada
    public Sprite PoeIkoon => poeIkoon;
}