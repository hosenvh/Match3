using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Inbox;
using UnityEngine;
using static Base;


namespace Match3.Development.DevOptions.Inbox
{
    [DevOptionGroup(groupName: "Inbox", priority: 30)]
    public class InboxDevOptions : DevelopmentOptionsDefinition
    {
        private static InboxManager InboxManager => ServiceLocator.Find<InboxManager>();

        [DevOption(commandName: "Register Test Inbox Element", shouldAutoClose: true)]
        public static void RegisterTestInboxElements(int elementsCount)
        {
            TestInboxElement inboxElementPrefab = GetInboxElementPrefab();
            for (int i = 0; i < elementsCount; i++)
            {
                InboxManager.RegisterInboxElement(
                    id: $"TestDevOptions_{i}",
                    orderIndex: 0,
                    inboxElementPrefab,
                    elementSetupData: GetDataForElement(i));
            }

            TestInboxElement GetInboxElementPrefab() => Resources.Load<TestInboxElement>("InboxDevOptions/TestInboxElement");

            TestInboxElementData GetDataForElement(int elementIndex)
            {
                return new TestInboxElementData(
                    onFirstButtonClick: () => OpenTestConfirmPopup(text: $"First Button Of elementIndex: {elementIndex} Clicked"),
                    onSecondButtonClick: () => OpenTestConfirmPopup(text: $"Second Button Of elementIndex: {elementIndex} Clicked"));

                void OpenTestConfirmPopup(string text) => gameManager.OpenPopup<Popup_ConfirmBox>().Setup(messageString: text, confirmString: "Okay", cancelString: "", closeOnConfirm: true);
            }
        }

        [DevOption(commandName: "Clear Inbox", shouldAutoClose: true)]
        public static void ClearRegisteredInboxElement()
        {
            InboxManager.ClearAllRegisteredInboxElements_Debug();
        }
    }
}