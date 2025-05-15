using UnityEngine;

public class Dot : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Dot with name " + name + " clicked!");
    }
}
