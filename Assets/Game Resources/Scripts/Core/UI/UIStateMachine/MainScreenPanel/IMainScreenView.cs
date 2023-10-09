using GameCore.Inventory;


namespace GameCore
{
    namespace GUI
    {
        public interface IMainScreenView : ISpecificView
        {
            public void SetActiveButton(int index);
            public void SetItemInformation(int position, GameItem item);
            public void MoveActiveButtonSelection(int direction);
        }
    }
}