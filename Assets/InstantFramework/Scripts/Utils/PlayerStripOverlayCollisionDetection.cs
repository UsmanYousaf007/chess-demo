using UnityEngine;

public class PlayerStripOverlayCollisionDetection : MonoBehaviour
{
    public string enableColliderName;
    public string disableColliderName;
    public Transform transformToEnable;
    public Transform container;
    public Vector3 enablePosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!string.IsNullOrEmpty(enableColliderName) && col.gameObject.name.Equals(enableColliderName))
        {
            EnableTransform();
        }
        else if (!string.IsNullOrEmpty(disableColliderName) && col.gameObject.name.Equals(disableColliderName))
        {
            transformToEnable.gameObject.SetActive(false);
        }
    }

    public void EnableTransform()
    {
        transformToEnable.SetParent(container, false);
        transformToEnable.localPosition = enablePosition;
        transformToEnable.gameObject.SetActive(true);
    }
}
