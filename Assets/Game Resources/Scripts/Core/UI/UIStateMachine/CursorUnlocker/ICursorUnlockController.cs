namespace GameCore
{
    namespace GUI
    {
        public interface ICursorUnlockController : ISpecificController
        {
            public void SetVirtualMouseAvailability(bool allowed);
        }
    }
}