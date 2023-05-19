namespace GameCore.GameControls
{
    public interface IHoldControl
    {
        public void Update();
        public void SetEnable();
        public void SetDisable();
    }

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