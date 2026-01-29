using UnityEngine;

public class Peamenüühaldur : MonoBehaviour
{
    public void AvaTasemed()
    {
        Debug.Log("Tasemed avatud");
    }

    public void AvaOpetus()
    {
        Debug.Log("Õpetus avatud");
    }

    public void Välju()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
