namespace GameCore
{
    namespace Animation
    {
        public interface IIdEventedAnimator
        {
            public void SetBool(int id, bool value);
            public void SetInteger(int id, int value);
            public void SetTrigger(int id);
            public void SetFloat(int id, float value);
            public void SetFloat(int id, float value, float dampTime, float deltaTime);
        }
    }
}