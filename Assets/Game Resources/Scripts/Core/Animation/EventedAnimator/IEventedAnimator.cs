namespace GameCore
{
    namespace Animation
    {
        public interface IEventedAnimator : IIdEventedAnimator
        {
            public void SetBool(string name, bool value);
            public void SetInteger(string name, int value);
            public void SetTrigger(string name);
            public void SetFloat(string name, float value);
            public void SetFloat(string name, float value, float dampTime, float deltaTime);
        }
    }
}