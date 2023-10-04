#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameCore
{
    namespace Inventory
    {
        public partial class ItemIdentificationService
        {
            [InitializeOnLoad]
            public static class PlayModeIdentifier
            {
                static PlayModeIdentifier()
                {
                    EditorApplication.playModeStateChanged += ModeStateChanged;
                }

                private static void ModeStateChanged(PlayModeStateChange state)
                {
                    if (state == PlayModeStateChange.ExitingEditMode)
                    {
                        IdentifyAllItems();
                    }
                }
            }
        }
    }
}
#endif