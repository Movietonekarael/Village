using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameCore.Inventory;
using UnityEngine.UI;
using Sirenix.OdinInspector;


namespace GameCore.Inventory
{
    public class ItemCell : MonoBehaviour
    {
        public Image itemImage;
        public Text itemCountText;
        [InfoBox("Put if necessary", InfoMessageType.Info)]
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
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = item.Image;
            itemCountText.text = item.Number.ToString();
        }

        private void SetEmptyItemInformation()
        {
            itemImage.gameObject.SetActive(false);
            itemImage.sprite = null;
            itemCountText.text = string.Empty;
        }

        protected virtual void Awake()
        {
            SetItemNumberTextEmpty();
            SetInactive();
        }

        private void SetItemNumberTextEmpty()
        {
            itemCountText.text = string.Empty;
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

