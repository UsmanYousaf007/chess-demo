using UnityEngine;

namespace FM.Legacy{
	public class Scaler : MonoBehaviour {

		private float screenWidth;
		private float scalerRatio;

		private RectTransform rectTransform;
		[SerializeField] private float minScale;
		[SerializeField] private float maxScale;
		[SerializeField] private float screenRatio;
		[SerializeField] private bool isDefault;
		private int index;

		private Scroller parent;

		private bool horizontal = false;
		private bool updateParent = false;

		public Transform left;
		public Transform right;

		private Vector3 originalPosition;

		void Start(){
			index = transform.GetSiblingIndex();
			parent = transform.parent.GetComponent<Scroller>();
			GetRectTransform();
			UpdateScreenConfig();
		}

		void UpdateScreenConfig(){
			screenWidth = Screen.width;
			scalerRatio = (maxScale - minScale) / (screenWidth / screenRatio);
		}

		void Update () {
			#region Orientation
			if(Screen.width > Screen.height){
				if(!horizontal){
					horizontal = true;
					UpdateScreenConfig();
				}
			}else{
				if(horizontal){
					horizontal = false;
					UpdateScreenConfig();
				}
			}
			#endregion

			float xPos = rectTransform.position.x - (screenWidth/2);
			// if(transform.GetSiblingIndex() == 0){
			// 	Debug.Log(rectTransform.position.x);
			// }
			// Debug.Log(xPos);

			RectTransform newTransform = rectTransform;
			if(Mathf.Abs(xPos) > screenWidth/screenRatio){
				newTransform.localScale = new Vector3(minScale, minScale, 1f);
				updateParent = true;
			}else{
				newTransform.localScale = new Vector3(maxScale - ((Mathf.Abs(xPos) * scalerRatio)), maxScale - ((Mathf.Abs(xPos) * scalerRatio)), 1f);
				parent.SetCurrentItem(rectTransform, index, this, updateParent);
				updateParent = false;
			}
		}

		public void SelectItem(){
			// Debug.Log("myname: "+name);
			// parent.SelectItem(rectTransform, index);
		}

		public void SetIndex(int _index){
			index = _index;
		}

		public void Setup()
		{
			GetRectTransform();
			originalPosition = rectTransform.localPosition;
		}

		public void Reset()
        {
			rectTransform.localPosition = originalPosition;
			rectTransform.localScale = isDefault ? new Vector3(maxScale, maxScale, 1) : new Vector3(minScale, minScale, 1);
        }

		void GetRectTransform()
		{
			if (rectTransform == null)
			{
				rectTransform = GetComponent<RectTransform>();
			}
		}
    }
}