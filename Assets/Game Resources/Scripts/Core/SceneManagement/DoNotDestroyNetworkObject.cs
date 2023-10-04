using GameCore.Network;


namespace GameCore
{
    namespace SceneManagement
    {
        public sealed class DoNotDestroyNetworkObject : DefaultNetworkBehaviour
        {
            protected override void AllOnNetworkSpawn()
            {
                transform.parent = null;
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}