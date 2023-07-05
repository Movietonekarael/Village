using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GUI
{
    public sealed class CursorUnlockUIState : BaseUIState<CursorUnlockViewParameters, ICursorUnlockController>
    {
        protected override void StartState()
        {

        }

        protected override void EndState()
        {

        }
    }

    public sealed class CursorUnlockController : UIController<CursorUnlockViewParameters, ICursorUnlockController, ICursorUnlockView>, ICursorUnlockView
    {
        protected override void InitializeParameters(CursorUnlockViewParameters parameters)
        {

        }

        protected override void OnActivate()
        {

        }

        protected override void OnDeactivate()
        {

        }

        protected override void SubscribeForEvents()
        {

        }

        protected override void UnsubscribeForEvents()
        {

        }
    }

    public sealed class CursorUnlockView : UIView<CursorUnlockViewParameters, ICursorUnlockController, ICursorUnlockView>, ICursorUnlockView
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

        protected override void InstantiateViewElements()
        {

        }
    }
}