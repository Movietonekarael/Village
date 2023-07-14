using System;
using UnityEngine; 


namespace GameCore
{
    namespace Inventory
    {
        public sealed class PlayerInventory : MonoBehaviour, IInventory
        {
            [Header("Inventory: ")]
            [SerializeField] private int _inventorySize = 32;
            private GameItem[] _gameItems;

            public Transform DropPoint;

            public event Action<int> OnItemChanged;

            public int GetInventorySize()
            {
                return _inventorySize;
            }

            private void Start()
            {
                _gameItems = new GameItem[_inventorySize];
            }

            public bool Push(ref GameItem item)
            {
                var result = PushItemToNextCell(ref item, 0);

                return result;
            }

            private bool PushItemToNextCell(ref GameItem item, int startIndex)
            {
                for (var i = startIndex; i < _inventorySize; i++)
                {
                    if (_gameItems[i] == null || _gameItems[i].Data == item.Data)
                    {
                        var addResult = GameItem.Add(_gameItems[i], item);

                        if (addResult.Item1)
                        {
                            _gameItems[i] = addResult.Item2;
                            item = addResult.Item3;
                            OnItemChanged?.Invoke(i);
                            return true;
                        }
                        else
                        {
                            _gameItems[i] = addResult.Item3;
                            item = addResult.Item2;
                            return PushItemToNextCell(ref item, i + 1);
                        }
                    }
                }

                Debug.Log("Inventory is full.");
                return false;
            }

            public bool Push(ref GameItem item, int position)
            {
                var inventoryItem = _gameItems[position];
                if (inventoryItem != null && inventoryItem.Data == item.Data)
                {
                    var addResult = GameItem.Add(inventoryItem, item);
                    _gameItems[position] = addResult.Item2;
                    item = addResult.Item3;
                    OnItemChanged?.Invoke(position);
                    return addResult.Item1;
                }
                else
                {
                    _gameItems[position] = item;
                    item = inventoryItem;
                    OnItemChanged?.Invoke(position);
                    return true;
                }
            }

            public GameItem Pull(int position)
            {
                var item = _gameItems[position];
                _gameItems[position] = null;
                OnItemChanged?.Invoke(position);
                return item;
            }

            public GameItem GetGameItem(int position)
            {
                return _gameItems[position];
            }

            public void DropItem(int position)
            {
                var item = Pull(position);
                for (var i = 0; i < item.Number; i++)
                {
                    Instantiate(item.Prefab, DropPoint.position, Quaternion.identity);
                }
            }

            public void MoveItem(int fromPosition, int toPosition)
            {
                if (fromPosition == toPosition) return;
                Push(ref _gameItems[fromPosition], toPosition);
                OnItemChanged?.Invoke(fromPosition);
            }
        }
    }
}