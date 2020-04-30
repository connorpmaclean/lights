namespace Lights.Lifx
{
    public static class LifxModelsExtensions
    {
        public static bool IsOn(this Light light)
        {
            return light.Power == "on";
        }
    }
}