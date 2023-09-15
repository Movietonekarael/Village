namespace GameCore
{
    namespace GameControls
    {
        public interface ICameraRotationSubscriber
        {
            public void SubscribeForCameraRotateInput();
            public void UnsubscribeForCameraRotateInput();
        }
    }
}