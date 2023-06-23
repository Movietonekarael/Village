namespace GameCore
{
    public interface IActivatable<T>
    {
        public void Activate();
        public void Deactivate();
    }
}