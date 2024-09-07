namespace Match3.Foundation.Base.Configuration
{
    public static class ConfigurationManagerRegisterationExtension
    {
        public delegate bool ReplacePredicate<T>(Configurer<T> otherConfigurer);

        // NOTE: This functionality can later be added to the ConfigurationManager, to improve 
        // this implementation.
        public static void RegisterAdditive<T>(
            this ConfigurationManager configurationManager,
            Configurer<T> newConfigurer,
            ReplacePredicate<T> replaceMatch)
        {
            var originalConfigurer = configurationManager.FindConfigurer<T>();

            if (originalConfigurer == null)
            {
                configurationManager.Register(newConfigurer);
            }
            else if (replaceMatch.Invoke(originalConfigurer))
            {
                configurationManager.RemoveConfigurer<T>();
                configurationManager.Register(newConfigurer);
            }
            else if (originalConfigurer is CompositeConfigurer<T> compositeConfigurer)
            {
                compositeConfigurer.RemoveConfigurer(configurer => replaceMatch.Invoke(configurer));
                compositeConfigurer.AddConfigurer(newConfigurer);
            }
            else
            {
                configurationManager.RemoveConfigurer<T>();

                var newCompositeConfigurer = new CompositeConfigurer<T>();
                newCompositeConfigurer.AddConfigurer(originalConfigurer);
                newCompositeConfigurer.AddConfigurer(newConfigurer);

                configurationManager.Register(newCompositeConfigurer);
            }
        }


        // TODO: Find a better name.
        public static void RegisterAdditiveOrReplace<T>(
            this ConfigurationManager configurationManager,
            Configurer<T> newConfigurer)
        {
            RegisterAdditive(
                configurationManager,
                newConfigurer,
                replaceMatch: otherConfigurer => otherConfigurer.GetType().Equals(newConfigurer.GetType()));
        }
    }
}