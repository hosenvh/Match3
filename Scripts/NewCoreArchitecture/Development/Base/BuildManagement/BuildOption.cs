namespace Medrick.Development.Base.BuildManagement
{
    public interface BuildOption
    {
        void Apply();

        void Reset();

        string Name();
    }
}