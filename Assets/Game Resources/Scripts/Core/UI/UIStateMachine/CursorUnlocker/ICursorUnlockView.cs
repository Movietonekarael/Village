namespace GameCore
{
    namespace GUI
    {
        public interface ICursorUnlockView : ISpecificView
        {
            public void EnableSelection();
            public void DisableSelection();
        }
    }
}