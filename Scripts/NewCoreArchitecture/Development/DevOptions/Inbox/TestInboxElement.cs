using System;
using Match3.Game.Inbox;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Development.DevOptions.Inbox
{
    public class TestInboxElementData : InboxElementData
    {
        public Action OnFirstButtonClick { get; }
        public Action OnSecondButtonClick { get; }

        public TestInboxElementData(Action onFirstButtonClick, Action onSecondButtonClick)
        {
            OnFirstButtonClick = onFirstButtonClick;
            OnSecondButtonClick = onSecondButtonClick;
        }
    }

    public class TestInboxElement : InboxElement<TestInboxElementData>
    {
        [SerializeField] private Button firstButton;
        [SerializeField] private Button secondButton;

        protected override void DoInternalSetups(TestInboxElementData data)
        {
            firstButton.onClick.AddListener(data.OnFirstButtonClick.Invoke);
            secondButton.onClick.AddListener(data.OnSecondButtonClick.Invoke);
        }
    }
}