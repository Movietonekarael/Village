namespace GameCore
{
    namespace GUI
    {
        public interface IUIState
        {
            public void EnterState(params bool[] args);
            public void ExitState();
        }
    }
}