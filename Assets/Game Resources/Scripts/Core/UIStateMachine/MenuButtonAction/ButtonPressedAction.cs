using System;


namespace GameCore
{
    namespace GUI
    {
        public sealed class ButtonPressedAction : MenuButtonAction
        {
            private event Action _onButtonPressed;

            public ButtonPressedAction(/*Action action, */MenuButtonAction nextState = null) : base(nextState)
            {
                //_onButtonPressed = action;
            }

            protected override void Execute()
            {
                HandleEventAction();
            }

            private void HandleEventAction()
            {
                _onButtonPressed?.Invoke();
            }
        }
    }
}