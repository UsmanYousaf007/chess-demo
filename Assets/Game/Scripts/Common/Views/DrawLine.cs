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
        //lineRenderer.startWidth = 33.0f;
        //lineRenderer.endWidth = 33.0f;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 0.8f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );

        lineRenderer.colorGradient = gradient;
    }

    public void Draw(Vector3 pointA, Vector3 pointB, float width = 20.0f)
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);
    }
    
    public void Hide()
    {
        lineRenderer.positionCount = 0;
    }

    public void Fade(int from, int to, float time)
    {
        iTween.ValueTo(this.gameObject, iTween.Hash("from", from, "to", to, "time", time, "onupdate", "SetAlpha", "onupdatetarget", this.gameObject));
    }

    public void SetAlpha(int value)
    {
        var color = lineRenderer.material.color;
        lineRenderer.material.color = new Color(color.r, color.g, color.b, (float)value/255f);
    }
}
