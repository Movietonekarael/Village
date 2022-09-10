using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using GameCore.GameControls;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameCore.Inventory
{
    public struct UIItem
    {
        public event UIntHandler OnNumberChanged;
        public event IntHandler OnImageChanged;
        public event IntHandler OnItemNumberChanged;
        public InventoryItem inventoryItem;
        public Image image { private get; set; }
        public int xCoordinate;

        public Sprite imageSprite
        {
            get
            {
                return image.sprite;
            }
            set
            {
                image.sprite = value;
                OnImageChanged?.Invoke(xCoordinate);
            }
        }

        public GameObject imageGameObject
        {
            get
            {
                return image.gameObject;
            }
        }

        private uint _number;
        public uint number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                OnNumberChanged?.Invoke(_number);
            }
        }

        public bool SetNumber(uint newNumber)
        {

            if (inventoryItem is null)
            {
                number = 0;
                OnItemNumberChanged?.Invoke(xCoordinate);
                return true;
            }
            else if (newNumber > inventoryItem.maxStackNumber)
            {
                OnItemNumberChanged?.Invoke(xCoordinate);
                return false;
            }
            else
            {
                number = newNumber;
                OnItemNumberChanged?.Invoke(xCoordinate);
                return true;
            }
        }
    }

    public struct PlayerUIItemMatrix
    {
        public Vector2Int inventorySize;
        public UIItem[,] inventory;
        public int holdingItem;

        public PlayerUIItemMatrix(int x, int y)
        {
            inventorySize = new Vector2Int(x, y);
            inventory = new UIItem[x, y];

            for (var i = 0; i < x; i++)
                for (var j = 0; j < y; j++)
                    inventory[i, j].number = 0;

            holdingItem = 0;
        }
    }

    public partial class PlayerInventory : MonoBehaviour, IInventory
    {
        [Header("Inventory: ")]

        public Vector2Int _inventorySize = new (8, 4);

        [HideInInspector] public PlayerUIItemMatrix _fullInventory;

        [Header("Holder: ")]
        [SerializeField] private PlayerHoldItem _holdItem;


        private void Awake()
        {
            SetUpInventory();
        }

        private void SetUpInventory()
        {
            _fullInventory = new(_inventorySize.x, _inventorySize.y);
        }

        public bool Push(InventoryItem item)
        {
            bool isPulled = false;
            for (var j = 0; j < _inventorySize.y; j++)
                for (var i = 0; i < _inventorySize.x; i++)
                    if (_fullInventory.inventory[i, j].inventoryItem == item)
                        if (isPulled = _fullInventory.inventory[i, j].SetNumber(_fullInventory.inventory[i, j].number + 1))
                        {
                            UpdateInventoryImage(i, j);
                            ChangeHoldedItem();
                            return true;
                        }
                            

            if (!isPulled)
                for (var j = 0; j < _inventorySize.y; j++)
                    for (var i = 0; i < _inventorySize.x; i++)
                        if (_fullInventory.inventory[i, j].inventoryItem is null)
                        {
                            _fullInventory.inventory[i, j].inventoryItem = item;
                            _fullInventory.inventory[i, j].SetNumber(1);
                            UpdateInventoryImage(i, j);
                            ChangeHoldedItem();
                            return true;
                        }


            return false;
        }

        public bool Push(InventoryItem item, int i, int j)
        {
            if (item is null)
            {
                _fullInventory.inventory[i, j].inventoryItem = null;
                _fullInventory.inventory[i, j].SetNumber(0);
                UpdateInventoryImage(i, j);
                ChangeHoldedItem();
                return true;
            }
            else if (_fullInventory.inventory[i, j].inventoryItem is null)
            {
                _fullInventory.inventory[i, j].inventoryItem = item;
                _fullInventory.inventory[i, j].SetNumber(1);
                UpdateInventoryImage(i, j);
                ChangeHoldedItem();
                return true;
            }
            else if (_fullInventory.inventory[i, j].inventoryItem == item && _fullInventory.inventory[i, j].SetNumber(_fullInventory.inventory[i, j].number + 1))
            {
                UpdateInventoryImage(i, j);
                ChangeHoldedItem();
                return true;
            }

            return false;
        }

        public InventoryItem Pull(int i, int j)
        {
            var item = _fullInventory.inventory[i, j].inventoryItem;
            if (item is not null)
            {
                _fullInventory.inventory[i, j].SetNumber(_fullInventory.inventory[i, j].number - 1);
                if (_fullInventory.inventory[i, j].number == 0)
                    _fullInventory.inventory[i, j].inventoryItem = null;

                UpdateInventoryImage(i, j);
                ChangeHoldedItem();
                return item;
            }
            else
            {
                return null;
            }
        }

        public void MoveItems(int x1, int y1, int x2, int y2)
        {
            var item1 = _fullInventory.inventory[x1, y1].inventoryItem;
            var item2 = _fullInventory.inventory[x2, y2].inventoryItem;
            var number1 = _fullInventory.inventory[x1, y1].number;
            var number2 = _fullInventory.inventory[x2, y2].number;

            _fullInventory.inventory[x1, y1].inventoryItem = item2;
            _fullInventory.inventory[x2, y2].inventoryItem = item1;
            _fullInventory.inventory[x1, y1].SetNumber(number2);
            _fullInventory.inventory[x2, y2].SetNumber(number1);

            UpdateInventoryImage(x1, y1);
            UpdateInventoryImage(x2, y2);
            ChangeHoldedItem();
        }

        private void UpdateInventoryImage(int x, int y)
        {
            if (_fullInventory.inventory[x, y].inventoryItem is null)
            {
                _fullInventory.inventory[x, y].imageSprite = null;
                _fullInventory.inventory[x, y].imageGameObject.SetActive(false);
            }
            else
            {
                _fullInventory.inventory[x, y].imageSprite = _fullInventory.inventory[x, y].inventoryItem.image;
                _fullInventory.inventory[x, y].imageGameObject.SetActive(true);
            }

        }

        public void ChangeHoldedItem()
        {
            var index = _fullInventory.holdingItem;
            _holdItem.item = _fullInventory.inventory[index, 0].inventoryItem;
        }

    }
}
