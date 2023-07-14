namespace GameCore
{
    namespace GUI
    {
        public interface ICursorUnlockView : ISpecificView
        {
            public void RememberSubmitButton();
            public void SetLastSubmitButton();
        }
    }
}