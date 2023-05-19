using GameCore.Inventory;

namespace GameCore.GUI
{
    public interface IMainScreenView
    {
        public void Init(MainScreenViewParameters parameters, MainScreenController controller);
        public void SetActiveButton(int index);
        public void SetItemInformation(int position, GameItem item);
        public void MoveActiveButtonSelection(int direction);
    }
}