using System.Collections.Generic;
using System.Linq;

namespace Match3.Game.Gameplay
{
    public class GameplaySystemsExecuter
    {
        // TODO: Mage gameplaysystem tags more generic.
        Dictionary<GameplaySystemTag, HashSet<IGameplaySystem>> systemTags;
        Dictionary<IGameplaySystem, int> systemOrderings = new Dictionary<IGameplaySystem, int>();

        List<IGameplaySystem> gameplaySystems = new List<IGameplaySystem>();

        List<IGameplaySystem> activeSystems = new List<IGameplaySystem>();


        List<IGameplaySystem> gameplaySystemsToDeactivate = new List<IGameplaySystem>();
        List<IGameplaySystem> gameplaySystemsToActivate = new List<IGameplaySystem>();

        public GameplaySystemsExecuter()
        {
            systemTags = new Dictionary<GameplaySystemTag, HashSet<IGameplaySystem>>();

            systemTags[GameplaySystemTag.EndOnly] = new HashSet<IGameplaySystem>();
            systemTags[GameplaySystemTag.StartOnly] = new HashSet<IGameplaySystem>();
            systemTags[GameplaySystemTag.General] = new HashSet<IGameplaySystem>();
        }

        public void AddSystem(IGameplaySystem system, GameplaySystemTag tag)
        {
            gameplaySystems.Add(system);
            systemTags[tag].Add(system);
        }

        public void StoreSystemsOrderings()
        {
            for (int i = 0; i < gameplaySystems.Count; ++i)
                systemOrderings[gameplaySystems[i]] = i;
        }

        public void ActivateSystemOfTag(GameplaySystemTag tag)
        {
            gameplaySystemsToActivate.AddRange(systemTags[tag]);
        }

        public void Start()
        {
            foreach (var system in gameplaySystems)
                system.Start();
            ActivateRequestedSystems();
            DeactivateRequestedSystems();
        }

        public void Update(float dt)
        {
            ActivateRequestedSystems();

            foreach (var system in activeSystems)
                system.Update(dt);

            DeactivateRequestedSystems();
        }

        private void DeactivateRequestedSystems()
        {
            foreach (var system in gameplaySystemsToDeactivate)
            {
                activeSystems.Remove(system);
                system.OnDeactivated();
            }

            gameplaySystemsToDeactivate.Clear();
        }

        private void ActivateRequestedSystems()
        {
            if (gameplaySystemsToActivate.Count == 0)
                return;

            foreach (var system in gameplaySystemsToActivate)
            {
                if (activeSystems.Contains(system) == false)
                {
                    activeSystems.Add(system);
                    system.OnActivated();
                }
            }

            activeSystems.Sort((a, b) => systemOrderings[a].CompareTo(systemOrderings[b]));

            gameplaySystemsToActivate.Clear();
        }

        public T GetSystem<T>() where T : IGameplaySystem
        {
            return (T)gameplaySystems.FirstOrDefault(s => s is T);
        }


        public void RequestActivation<T>() where T : IGameplaySystem
        {
            RequestActivation(GetSystem<T>());
        }

        public void RequestActivation(IGameplaySystem gameplaySystem)
        {
            gameplaySystemsToActivate.Add(gameplaySystem);
        }

        public void RequestDeactivation<T>() where T : IGameplaySystem
        {
            RequestDeactivation(GetSystem<T>());
        }

        public void RequestDeactivation(IGameplaySystem gameplaySystem)
        {
            gameplaySystemsToDeactivate.Add(gameplaySystem);
        }
    }
}