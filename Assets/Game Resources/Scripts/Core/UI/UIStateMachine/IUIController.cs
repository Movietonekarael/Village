namespace GameCore
{
    namespace GUI
    {
        public interface IUIController<T> where T : IUIParameters
        {
            public void Init(T parameters);
        }
    }
}