namespace GameCore
{
    namespace GUI
    {
        public interface IMultiplayerMenuView : ISpecificView
        {
            public void CreateMessageWindow(string message);
        }
    }
}