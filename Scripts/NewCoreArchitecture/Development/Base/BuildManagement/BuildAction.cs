namespace Medrick.Development.Base.BuildManagement
{
    public interface BuildAction
    {
        void Execute();
        void Revert();
    }
}