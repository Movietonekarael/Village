namespace GameCore
{
    namespace GUI
    {
        public interface IMainMenuView : ISpecificView
        {
            public void SetStartupAnimationAvailability(bool allowed);
        }
    }
}