using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


// TODO: Refactor this whole shit.
namespace Match3.Development.Base.DevelopmentConsole
{
    public class DevelopmentToolManager : MonoBehaviour
    {
        public DevelopmentGroup groupPrefab;
        public CommandInputPrompt commandInputPrompt;
        public ScrollRect toolsPanel;
        public Button devButton;
        public Text devButtonText;
        public GameObject groupsContainer;

        public UnityEvent onErrorDetected;
        public UnityEvent onToolsPanelOpened;
        public UnityEvent onToolsPanelClosed;

        private readonly List<ShortCutInfo> shortCutInfos = new List<ShortCutInfo>();

        private void Start()
        {
            toolsPanel.verticalNormalizedPosition = 1;
            toolsPanel.gameObject.SetActive(false);
            //reporter.gameObject.SetActive(false);
        }

        public void Init()
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);

            if (gameObject.activeSelf == false)
                return;

            InitDefinitionTypes();
            Application.logMessageReceived += CheckForErrors;
        }

        private void CheckForErrors(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                onErrorDetected.Invoke();
        }

        private void InitDefinitionTypes()
        {
            List<Type> definitionTypes = ReflectionUtilities.FindTypesOf(typeof(DevelopmentOptionsDefinition), false).Select(s => Type.GetType(s)).ToList();
            List<CommandGroupInfo> commandGroups = ExtractCommandGroupInfos(definitionTypes);

            CreateGroupObjectsFor(commandGroups);
            InitAllShortcuts(definitionTypes);
        }

        private void InitAllShortcuts(List<Type> allDefinitionTypes)
        {
            foreach (Type definitionType in allDefinitionTypes)
                InitShortCuts(definitionType);
        }

        // TODO: Refactor this shit.
        public void InitShortCuts(Type type)
        {
            var methods = GetMethodsWithAttribute(type, typeof(ShortCutAttribute));
            foreach (var method in methods)
            {
                if (method.IsStatic == false)
                {
                    Debug.LogErrorFormat("Method {0} in {1} is not static.", method, method.DeclaringType);
                    continue;
                }

                var attributes = method.GetCustomAttributes(typeof(ShortCutAttribute), false);
                var attribute = (attributes[0] as ShortCutAttribute);

                var shortCutInfo = new ShortCutInfo(attribute.keyCodes, () => method.Invoke(null, null));

                shortCutInfos.Add(shortCutInfo);
            }
        }

        private void Update()
        {
            foreach (var shortCut in shortCutInfos)
            {
                if (ArePressed(shortCut.keyCodes))
                    shortCut.action();
            }
        }

        private bool ArePressed(KeyCode[] keyCodes)
        {
            bool atleastOneIsDown = false;

            foreach (var key in keyCodes)
            {
                if (Input.GetKeyDown(key))
                    atleastOneIsDown = true;

                if (Input.GetKeyDown(key) == false && Input.GetKey(key) == false)
                    return false;
            }

            return atleastOneIsDown;
        }

        private List<CommandGroupInfo> ExtractCommandGroupInfos(List<Type> allDefinitionTypes)
        {
            var groups = new List<CommandGroupInfo>();

            foreach (Type definitionType in allDefinitionTypes)
                groups.Add(CreateCommandGroupFor(definitionType));

            return groups;
        }

        private CommandGroupInfo CreateCommandGroupFor(Type definitionType)
        {
            DevOptionGroupAttribute attribute = (DevOptionGroupAttribute) definitionType.GetCustomAttribute(typeof(DevOptionGroupAttribute), false);
            List<CommandInfo> commands = ExtractCommandsOf(definitionType);
            return new CommandGroupInfo(attribute.groupName, attribute.priority, commands);
        }

        private List<CommandInfo> ExtractCommandsOf(Type type)
        {
            var commands = new List<CommandInfo>();

            var methods = GetMethodsWithAttribute(type, typeof(DevOptionAttribute));

            foreach (var method in methods)
            {
                if (method.IsStatic == false)
                {
                    Debug.LogErrorFormat("Method {0} in {1} is not static.", method, method.DeclaringType);
                    continue;
                }
                var attributes = method.GetCustomAttributes(typeof(DevOptionAttribute), false);
                var attribute = (attributes[0] as DevOptionAttribute);

                if (attribute.ignore == true)
                    continue;

                var commandInfo = new CommandInfo(attribute.commandName, method, attribute.shouldAutoClose, attribute.Color(), HandleCommandExecution);

                var shortcuts = method.GetCustomAttributes(typeof(ShortCutAttribute), false);
                if (shortcuts.Length > 0)
                {
                    commandInfo.SetShortcut((shortcuts[0] as ShortCutAttribute).keyCodes);
                }

                commands.Add(commandInfo);
            }

            return commands;
        }

        private void CreateGroupObjectsFor(List<CommandGroupInfo> groups)
        {
            groups.Sort((group1, group2) => group1.priority.CompareTo(group2.priority));

            foreach (var group in groups)
            {
                var groupObject = Instantiate(groupPrefab, groupsContainer.transform, worldPositionStays: false);
                groupObject.Init(group.name);

                foreach (CommandInfo commandInfo in group.commands)
                    groupObject.AddCommand(commandInfo);
            }
        }

        private void HandleCommandExecution(CommandInfo commandInfo)
        {
            if (commandInfo.shouldAutoClose)
                Toggle();
        }

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute(Type classType, Type attributeType)
        {
            return classType.GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes(attributeType, true).Length > 0);
        }

        public static IEnumerable<MemberInfo> GetMembersWithAttribute(Type classType, Type attributeType)
        {
            return classType.GetMembers().Where(memberInfo => memberInfo.GetCustomAttributes(attributeType, true).Length > 0);
        }


        [ShortCut(KeyCode.LeftShift, KeyCode.D)]
        public void Toggle()
        {
            toolsPanel.gameObject.SetActive(!toolsPanel.gameObject.activeSelf);
            if (toolsPanel.gameObject.activeSelf)
                onToolsPanelOpened.Invoke();
            else
                onToolsPanelClosed.Invoke();
        }

        public void OpenCommandInputPromptFor(CommandInfo commandInfo)
        {
            commandInputPrompt.Init(commandInfo);
        }
    }
}