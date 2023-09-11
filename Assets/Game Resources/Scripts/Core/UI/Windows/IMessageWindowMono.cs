namespace GameCore
{
    namespace GUI
    {
        namespace Windows
        {
            public interface IMessageWindowMono : IMessageWindow, IWindow
            {
                public void SetMessage(string message);
            }
        }
    }
}