namespace GameCore
{
    namespace GUI
    {
        public sealed class CursorUnlockUIState : BaseUIState<CursorUnlockViewParameters, ICursorUnlockController>
        {
            private bool _virtualPointerAllowed = false;

            protected override void EndState() { }

            protected override void StartState(params bool[] args)
            {
                SetVirtualPointBool(args);
                _Controller.SetVirtualMouseAvailability(_virtualPointerAllowed);
            }

            private void SetVirtualPointBool(bool[] args)
            {
                if (args.Length > 0)
                    _virtualPointerAllowed = args[0];
                else
                    _virtualPointerAllowed = false;
            }
        }
    }
}