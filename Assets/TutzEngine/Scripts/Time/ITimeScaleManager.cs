namespace TutzEngine.TimeUtils
{
    public interface ITimeScaleManager : IStaticObject
    {
        float TimeScale { get; }
        void TemporaryTimeScaleChange(float value, float duration);
        void TimeScaleChange(float value);
    }
}

