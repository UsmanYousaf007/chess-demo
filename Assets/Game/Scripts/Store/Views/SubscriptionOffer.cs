using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

public class SubscriptionOffer : MonoBehaviour
{
    public Text text;
    public Image icon;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(Sprite icon, string text)
    {
        this.text.text = text;
        //TO-DO Remove code - SubDlg with icons
        this.icon.sprite = icon;
        this.icon.SetNativeSize();

        //Patch for no ads icon
        if (text.ToLower().Contains("ads"))
        {
            this.icon.rectTransform.localPosition = new Vector3(-40, 36, 0);
        }
    }
}
