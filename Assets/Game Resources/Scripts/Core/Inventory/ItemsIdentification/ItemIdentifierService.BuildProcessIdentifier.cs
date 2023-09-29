#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GameCore
{
    namespace Inventory
    {
        public static partial class ItemIdentifierService
        {
            public class BuildProcessIdentifier : IPreprocessBuildWithReport
            {
                public int callbackOrder => 0;

                public void OnPreprocessBuild(BuildReport report)
                {
                    IdentifyAllItems();
                }
            }
        }
    }
}
#endif