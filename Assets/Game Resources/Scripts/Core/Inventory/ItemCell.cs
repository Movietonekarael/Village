using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameCore.Inventory;
using UnityEngine.UI;


namespace GameCore.Inventory
{
    public class ItemCell : MonoBehaviour
    {
        public Image ItemImage;
        public Text ItemCountText;
        [Header("Put if necessary")]
        [SerializeField] private ImageColor _imageColor;

        private uint _number = 0;

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

        public void SetActive()
        {
            if (_imageColor != null)
                _imageColor.SetActiveColor();
        }

        public void SetInactive()
        {
            if (_imageColor != null)
                _imageColor.SetNormalColor();
        }
    }
}

