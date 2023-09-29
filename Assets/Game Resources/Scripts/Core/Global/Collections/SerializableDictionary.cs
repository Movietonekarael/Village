using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameCore
{
    namespace Collections
    {
        [Serializable]
        public sealed class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
        {
            [SerializeField]
            private List<TKey> keys = new();

            [SerializeField]
            private List<TValue> values = new();


            public void OnBeforeSerialize()
            {
                keys.Clear();
                values.Clear();
                foreach (KeyValuePair<TKey, TValue> pair in this)
                {
                    keys.Add(pair.Key);
                    values.Add(pair.Value);
                }
            }

            public void OnAfterDeserialize()
            {
                this.Clear();

                var keysCount = keys.Count;
                var valuesCount = values.Count;
                if (keysCount != valuesCount)
                    throw new System.Exception(string.Format($"There are {keysCount} keys and {valuesCount} values " +
                                                              "after deserialization. Make sure that both key and value " +
                                                              "types are serializable."));

                for (int i = 0; i < keys.Count; i++)
                    this.Add(keys[i], values[i]);
            }
        }
    }
}