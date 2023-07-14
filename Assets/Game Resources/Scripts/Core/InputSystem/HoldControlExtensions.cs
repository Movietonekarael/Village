namespace GameCore
{
    namespace GameControls
    {
        public static class HoldControlExtensions
        {
            public static void UpdateAll(this IHoldControl[] array)
            {
                foreach (var item in array)
                {
                    item.Update();
                }
            }
        }
    }
}