using System;

namespace GameCore
{
    namespace Animation
    {
        public interface IEventedAnimatorEvents
        {
            public event Action<int, bool> OnBoolParameterSetted;
            public event Action<int, int> OnIntegerParameterSetted;
            public event Action<int> OnTriggerParameterSetted;
            public event Action<int, float> OnFloatParameterSetted;
            public event Action<int, float, float, float> OnFloatWithDampParameterSetted;
        }
    }
}