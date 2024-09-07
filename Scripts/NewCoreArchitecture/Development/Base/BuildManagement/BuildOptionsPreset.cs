namespace Medrick.Development.Base.BuildManagement
{
    public interface BuildOptionsPreset
    {
        void Apply();

        string Name();
    }
}