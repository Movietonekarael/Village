namespace GameCore
{
    namespace GUI
    {
        public interface IUIView<T, P> where T : IUIParameters where P : ISpecificController
        {
            public void Init(T parameters, P controller);
        }
    }
}