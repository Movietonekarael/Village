using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;


namespace GameCore
{
    namespace GUI
    {
        public sealed class MuiltiplayerMenuUIState : BaseUIState<MultiplayerMenuViewParameters,
                                                                  MultiplayerMenuController,
                                                                  IMultiplayerMenuController>
        {
            protected override void EndState()
            {

            }

            protected override void StartState(params bool[] args)
            {

            }
        }

        public interface IMultiplayerMenuController : ISpecificController
        {

        }

        public struct RelayHostData
        {
            public string JoinCode;
            public string IPv4Address;
            public ushort Port;
            public System.Guid AllocationID;
            public byte[] AllocationIDBytes;
            public byte[] ConnectionData;
            public byte[] Key;
        }

        public sealed class MultiplayerMenuController : UIController<MultiplayerMenuViewParameters,
                                                                     IMultiplayerMenuController,
                                                                     MultiplayerMenuView,
                                                                     IMultiplayerMenuView>,
                                                        IMultiplayerMenuController
        {
            protected override void InitializeParameters(MultiplayerMenuViewParameters parameters)
            {

            }

            protected override void OnActivate()
            {
                var data = StartRelayServer();

            }

            private async Task<RelayHostData> StartRelayServer()
            {
                Debug.Log("Starting authentication.");

                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    //If not already logged, log the user in
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                Debug.Log("Starting relay server.");

                Allocation allocation = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(5);

                RelayHostData data = new RelayHostData
                {
                    // WARNING allocation.RelayServer is deprecated
                    IPv4Address = allocation.RelayServer.IpV4,
                    Port = (ushort)allocation.RelayServer.Port,

                    AllocationID = allocation.AllocationId,
                    AllocationIDBytes = allocation.AllocationIdBytes,
                    ConnectionData = allocation.ConnectionData,
                    Key = allocation.Key,
                };

                data.JoinCode = await Unity.Services.Relay.RelayService.Instance.GetJoinCodeAsync(data.AllocationID);
                Debug.Log($"Data: {data.JoinCode}.");

                return data;
            }

            protected override void OnDeactivate()
            {

            }

            protected override void SubscribeForPermanentEvents()
            {

            }

            protected override void SubscribeForTemporaryEvents()
            {

            }

            protected override void UnsubscribeForPermanentEvents()
            {

            }

            protected override void UnsubscribeForTemporaryEvents()
            {

            }
        }

        public interface IMultiplayerMenuView : ISpecificView
        {

        }

        public sealed class MultiplayerMenuView : UIView<MultiplayerMenuViewParameters,
                                                         IMultiplayerMenuController,
                                                         IMultiplayerMenuView>,
                                                  IMultiplayerMenuView
        {
            public override void Activate()
            {

            }

            public override void Deactivate()
            {

            }

            public override void Deinitialize()
            {

            }

            protected override void InstantiateViewElementsOnAwake()
            {

            }
        }
    }
}