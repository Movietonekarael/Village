using System;


namespace GameCore
{
    namespace GUI
    {
        namespace Windows
        {
            public interface IConnectionCodeWindow
            {
                public event Action<string> OnOkPressed;
            }
        }    
    }
}