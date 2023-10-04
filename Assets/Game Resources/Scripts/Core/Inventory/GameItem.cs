using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore
{
    namespace Inventory
    {
        public sealed class GameItem
        {
            public GameItemData Data;
            public string Name => Data.Name;
            public AssetReferenceGameObject PrefabReference => Data.PrefabReference;
            public Sprite Image => Data.Image;
            public int MaxStackNumber => Data.MaxStackNumber;

            public int Number = 1;


            public GameItem(GameItemData data, int number)
            {
                Data = data;
                Number = number;
            }

            public static (bool, GameItem, GameItem) Add(GameItem a, GameItem b)
            {
                if (a == null && b != null)
                {
                    a = b;
                    b = null;
                    return (true, a, b);
                }
                else if (a != null && b == null)
                {
                    return (true, a, b);
                }
                else if (a == null && b == null)
                {
                    return (true, a, b);
                }
                else if (a.Data == b.Data)
                {
                    a.Number += b.Number;
                    var difference = a.Number - a.MaxStackNumber;
                    a.Number = a.Number > a.MaxStackNumber ? a.MaxStackNumber : a.Number;
                    if (difference > 0)
                    {
                        if (difference != b.Number)
                        {
                            b.Number = difference;
                            return (false, a, b);
                        }
                        else
                        {
                            b.Number = difference;
                            return (false, b, a);
                        }
                    }
                    else
                    {
                        b = null;
                        return (true, a, b);
                    }
                }
                else
                {
                    throw new System.InvalidOperationException("Trying to add two Game Items " +
                                                               "with different data sources.");
                }
            }
        }
    }
}