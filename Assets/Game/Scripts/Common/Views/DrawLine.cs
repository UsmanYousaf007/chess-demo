using UnityEngine;
using System.Collections;

public class DrawLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
   
    // Use this for initialization
    void Start()
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 10.0f;
        lineRenderer.endWidth = 10.0f;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );

        lineRenderer.colorGradient = gradient;
    }

    public void Draw(Vector3 pointA, Vector3 pointB)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);
    }
    
    public void Hide()
    {
        lineRenderer.positionCount = 0;
    }
}
