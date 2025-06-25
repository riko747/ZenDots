using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dotLabel;
    [SerializeField] private Image dotImage;
    
    public RectTransform GetDotTransform => transform as RectTransform;
    public Vector2 GetDotPosition => transform.position;
    public TextMeshProUGUI DotLabel => dotLabel;
    public Image DotImage => dotImage;
    public float GetDotSizeInWorldSpace => GetSizeInWorldSpace();
    public float GetDotSizeInPixelsX => GetDotTransform.sizeDelta.x;
    public float GetDotSizeInPixelsY => GetDotTransform.sizeDelta.y;
    public int DotNumber { get; private set; }

    private float GetSizeInWorldSpace()
    {
        Vector3[] corners = new Vector3[4];
        GetDotTransform.GetWorldCorners(corners);
        return Vector3.Distance(corners[0], corners[1]);
    }

    public void SetDotSize(float size) => GetDotTransform.sizeDelta = new Vector2(size, size);

    public void SetDotNumber(int number) => DotNumber = number;

    public float GetMinDotSize() => 100;
    public float GetMaxDotSize() => 250;
}
