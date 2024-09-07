using Match3.Foundation.Base.Configuration;


namespace Match3.Development.DevOptions.Configurers
{
    public class FailServerConfigRequestConfigurer : Configurer<ServerConfigRequest>
    {
        public void Configure(ServerConfigRequest entity)
        {
            entity.SetServerURL("https://srv1.trantor.medrick.games/FAIL");
        }

        public void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}