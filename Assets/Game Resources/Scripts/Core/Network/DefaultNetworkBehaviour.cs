using Unity.Netcode;

namespace GameCore
{
    namespace Network
    {
        public abstract class DefaultNetworkBehaviour : NetworkBehaviour
        {
            private void Awake()
            {
                if (IsServer)
                    ServerAwake();
                if (IsHost)
                    HostAwake();
                if (IsClient)
                    ClientAwake();
                AllAwake();
            }

            protected virtual void AllAwake() { }
            protected virtual void ServerAwake() { }
            protected virtual void HostAwake() { }
            protected virtual void ClientAwake() { }

            private void Start()
            {
                if (IsServer)
                    ServerStart();
                if (IsHost)
                    HostStart();
                if (IsClient)
                    ClientStart();
                AllStart();
            }

            protected virtual void AllStart() { }
            protected virtual void ServerStart() { }
            protected virtual void HostStart() { }
            protected virtual void ClientStart() { }

            private void Update()
            {
                if (IsServer)
                    ServerUpdate();
                if (IsHost)
                    HostUpdate();
                if (IsClient)
                    ClientUpdate();
                AllUpdate();
            }

            protected virtual void AllUpdate() { }
            protected virtual void ServerUpdate() { }
            protected virtual void HostUpdate() { }
            protected virtual void ClientUpdate() { }

            private void FixedUpdate()
            {
                if (IsServer)
                    ServerFixedUpdate();
                if (IsHost)
                    HostFixedUpdate();
                if (IsClient)
                    ClientFixedUpdate();
                AllFixedUpdate();
            }

            protected virtual void AllFixedUpdate() { }
            protected virtual void ServerFixedUpdate() { }
            protected virtual void HostFixedUpdate() { }
            protected virtual void ClientFixedUpdate() { }

            private void LateUpdate()
            {
                if (IsServer)
                    ServerLateUpdate();
                if (IsHost)
                    HostLateUpdate();
                if (IsClient)
                    ClientLateUpdate();
                AddLateUpdate();
            }

            protected virtual void AddLateUpdate() { }
            protected virtual void ServerLateUpdate() { }
            protected virtual void HostLateUpdate() { }
            protected virtual void ClientLateUpdate() { }

            public sealed override void OnDestroy()
            {
                if (IsServer)
                    OnServerDestroy();
                if (IsHost)
                    OnHostDestroy();
                if (IsClient)
                    OnClientDestroy();
                AllOnDestroy();
            }

            protected virtual void AllOnDestroy() { }
            protected virtual void OnServerDestroy() { }
            protected virtual void OnHostDestroy() { }
            protected virtual void OnClientDestroy() { }

            public sealed override void OnNetworkSpawn()
            {
                if (IsServer)
                    OnServerNetworkSpawn();
                if (IsHost)
                    OnHostNetworkSpawn();
                if (IsClient)
                    OnClientNetworkSpawn();
                AllOnNetworkSpawn();
            }

            protected virtual void AllOnNetworkSpawn() { }
            protected virtual void OnServerNetworkSpawn() { }
            protected virtual void OnHostNetworkSpawn() { }
            protected virtual void OnClientNetworkSpawn() { }

            public sealed override void OnNetworkDespawn()
            {
                if (IsServer)
                    OnServerNetworkDespawn();
                if (IsHost)
                    OnHostNetworkDespawn();
                if (IsClient)
                    OnClientNetworkDespawn();
                AllOnNetworkDespawn();
            }

            protected virtual void AllOnNetworkDespawn() { }
            protected virtual void OnServerNetworkDespawn() { }
            protected virtual void OnHostNetworkDespawn() { }
            protected virtual void OnClientNetworkDespawn() { }

            public sealed override void OnGainedOwnership()
            {
                if (IsServer)
                    OnServerGainedOwnership();
                if (IsHost)
                    OnHostGainedOwnership();
                if (IsClient)
                    OnClientGainedOwnership();
                AllOnGainedOwnership();
            }

            protected virtual void AllOnGainedOwnership() { }
            protected virtual void OnServerGainedOwnership() { }
            protected virtual void OnHostGainedOwnership() { }
            protected virtual void OnClientGainedOwnership() { }

            public sealed override void OnLostOwnership()
            {
                if (IsServer)
                    OnServerLostOwnership();
                if (IsHost)
                    OnHostLostOwnership();
                if (IsClient)
                    OnClientLostOwnership();
                AllOnLostOwnership();
            }

            protected virtual void AllOnLostOwnership() { }
            protected virtual void OnServerLostOwnership() { }
            protected virtual void OnHostLostOwnership() { }
            protected virtual void OnClientLostOwnership() { }
        }
    }
}