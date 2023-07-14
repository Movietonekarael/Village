namespace GameCore
{
    namespace GUI
    {
        public interface IMainScreenController : ISpecificController
        {
            public void SetActiveItem(int index);
        }
    }
}