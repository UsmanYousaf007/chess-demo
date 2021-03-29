using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PickerDemoController : MonoBehaviour
{
    private List<Button> m_DemoButtons = null;

	void Awake()
	{
        //collect demo buttons
        m_DemoButtons = GetComponentsInChildren<Button>().ToList();

		GameObject.DontDestroyOnLoad(gameObject);
        OnClickButton( m_DemoButtons.Where( button => button.name == "CustomSkinDemo" ).First() );
    }

    static void LoadScene( string name )
    {
	#if UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4_7 || UNITY_4_6
		Application.LoadLevel( name );
	#else
		UnityEngine.SceneManagement.SceneManager.LoadScene( name );
	#endif
	}

    public void OnClickButton( Button button )
    {
        foreach (Button demoButton in m_DemoButtons)
        {
            Text text = demoButton.GetComponentInChildren<Text>();

            if (demoButton == button)
            {
                text.fontStyle = FontStyle.Bold;
                demoButton.interactable = false;
            }
            else
            {
                text.fontStyle = FontStyle.Normal;
                demoButton.interactable = true;
            }
        }

        LoadScene(button.name);
    }
}

