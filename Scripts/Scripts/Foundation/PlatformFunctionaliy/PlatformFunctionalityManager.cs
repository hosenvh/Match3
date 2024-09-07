
using Match3.Foundation.Base.ServiceLocating;

namespace Medrick.Foundation.Base.PlatformFunctionality
{
    public interface PlatformFunctionalityManager : Service
    {
        int VersionCode();
    }
}