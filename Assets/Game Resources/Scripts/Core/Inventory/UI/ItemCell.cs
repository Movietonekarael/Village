using GameCore.GUI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace GameCore
{
    namespace Inventory
    {
        public class ItemCell : MonoBehaviour, ISelectable
        {
            public Image ItemImage;
            public TextMeshProUGUI ItemCountText;
            [Header("Put if necessary")]
            [SerializeField] private ImageColor _imageColor;

            private readonly uint _number = 0;

            public uint GetNumber()
            {
                return _number;
            }

            public void SetItem(GameItem item)
            {
                if (item is not null)
                    SetProperItemInformation(item);
                else
                    SetEmptyItemInformation();
            }

            private void SetProperItemInformation(GameItem item)
            {
                ItemImage.gameObject.SetActive(true);
                ItemImage.sprite = item.Image;
                ItemCountText.text = item.Number.ToString();
            }

            private void SetEmptyItemInformation()
            {
                ItemImage.gameObject.SetActive(false);
                ItemImage.sprite = null;
                ItemCountText.text = string.Empty;
            }

            protected virtual void Awake()
            {
                SetItemNumberTextEmpty();
                SetInactive();
            }

            private void SetItemNumberTextEmpty()
            {
                ItemCountText.text = string.Empty;
            }

            public void Select()
            {
                SetActive();
            }

            public void Deselect()
            {
                SetInactive();
            }

            private void SetActive(bool isActive)
            {
                if (isActive)
                    SetActive();
                else
                    SetInactive();
            }

            private void SetActive()
            {
                if (_imageColor != null)
                    _imageColor.SetActiveColor();
            }

            private void SetInactive()
            {
                if (_imageColor != null)
                    _imageColor.SetNormalColor();
            }
        }
    }
}