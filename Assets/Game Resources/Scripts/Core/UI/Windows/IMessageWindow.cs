using System;


namespace GameCore
{
    namespace GUI
    {
        namespace Windows
        {
            public interface IMessageWindow
            {
                public event Action OnOkPressed;
            }
        }   
    }
}