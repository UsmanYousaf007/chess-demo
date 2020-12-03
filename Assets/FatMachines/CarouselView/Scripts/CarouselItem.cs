using UnityEngine;
using UnityEngine.UI;

namespace FM{
    [RequireComponent(typeof(Button))]
    public class CarouselItem : MonoBehaviour {

        public Transform left;
        public Transform right;

        private Canvas canvas;

        private float maxScale;
        private float minScale;

        private float rightLimit;
        private float scaleDiff;
        private float scaleRatio;

        CarouselView carouselView;
        RectTransform carouselViewT;
        RectTransform rectT;

        private float selectionMargin = 50f;

        void Start(){
            carouselViewT = transform.parent.parent.parent.GetComponent<RectTransform>();
            carouselView = carouselViewT.GetComponent<CarouselView>();
            rectT = GetComponent<RectTransform>();
            canvas = carouselView.parentCanvas;
            maxScale = carouselView.maxScale;
            minScale = carouselView.minScale;

            System.Action InitValues = () => {
                selectionMargin = (rectT.sizeDelta.x / 2f) * canvas.GetComponent<RectTransform>().localScale.x; ;

                rightLimit = (carouselViewT.rect.width * canvas.GetComponent<RectTransform>().localScale.x) / 2f;
                scaleDiff = maxScale - minScale;
                scaleRatio = scaleDiff / rightLimit;
            };

            InitValues();

            carouselView.GetComponent<DeviceOrientation>().AddOnOrienationChange((DeviceOrientation.Orientation or) => {
                InitValues();
            });

            GetComponent<Button>().onClick.AddListener(() => {
                if(carouselView.OnItemClick != null){
                    carouselView.OnItemClick(transform.GetSiblingIndex());
                }
            });
        }

        void OnGUI(){
            Vector2 myPos = rectT.position - carouselViewT.position;
            float scaleFactor = (rightLimit - Mathf.Abs(myPos.x)) * scaleRatio;
            rectT.localScale = new Vector3(minScale + scaleFactor, minScale + scaleFactor, 1f);

            if(myPos.x >= -selectionMargin && myPos.x <= selectionMargin){
                carouselView.selectedIndex = transform.GetSiblingIndex();
                carouselView.selectedItem = transform;

                rectT.SetSiblingIndex(1);
                left.SetSiblingIndex(0);
                right.SetSiblingIndex(2);

                carouselView.scrollT.horizontalNormalizedPosition = carouselView.startingNormalisedPosition;
            }

            
        }

    }
}

