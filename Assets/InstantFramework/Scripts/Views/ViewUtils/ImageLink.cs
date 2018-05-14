using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageLink : MonoBehaviour {

    public Image imageLink;

    public void Awake()
    {
        this.GetComponent<Image>().sprite = imageLink.sprite;
    }
}
