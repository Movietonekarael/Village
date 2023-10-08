using GameCore.Inventory;
using Unity.Netcode;

namespace GameCore
{
    namespace Network
    {
        public struct NetworkGameItem : INetworkSerializable
        {
            public uint Id;
            public int Number;
            public bool NotNull;

            public NetworkGameItem(uint id, int number, bool notNull = true)
            {
                Id = id;
                Number = number;
                NotNull = notNull;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Id);
                serializer.SerializeValue(ref Number);
                serializer.SerializeValue(ref NotNull);
            }

            public GameItem DeserializeGameItem(ItemIdentificationService identificationService)
            {
                GameItem gameItem;
                if (NotNull)
                {
                    var gameItemData = identificationService.GetItemData(Id);
                    gameItem = new GameItem(gameItemData, Number);
                }
                else
                {
                    gameItem = null;
                }

                return gameItem;
            }
        }
    }
}