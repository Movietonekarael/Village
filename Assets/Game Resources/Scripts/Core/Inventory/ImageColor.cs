using UnityEngine;
using UnityEngine.UI;


namespace GameCore
{
    namespace Inventory
    {
        [RequireComponent(typeof(Image))]
        public class ImageColor : MonoBehaviour
        {
            [SerializeField] private Color _normalColor;
            [SerializeField] private Color _activeColor;
            private Image _image;


            private void Awake()
            {
                _image = GetComponent<Image>();
            }

            public void SetNormalColor()
            {
                _image.color = _normalColor;
            }

            public void SetActiveColor()
            {
                _image.color = _activeColor;
            }
        }
    }
}