using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.MainMenuLayout;
using UnityEngine;


namespace Match3.Presentation.MainMenu
{
    public delegate Transform MainMenuButtonCreator(Transform buttonParent, Transform buttonControllerParent);
    public delegate bool CreationCondition();

    public class MainMenuButtonBoard : Service
    {
        [Serializable]
        public class CreatableButton
        {
            public string id;
            public MainMenuButtonSetting setting;
            public MainMenuButtonCreator create;
            public CreationCondition condition;
            
            public CreatableButton(string id, MainMenuButtonSetting setting, CreationCondition condition, MainMenuButtonCreator create)
            {
                this.id = id;
                this.setting = setting;
                this.condition = condition;
                this.create = create;
            }
        }

        private Dictionary<string, CreatableButton> buttons = new Dictionary<string, CreatableButton>(); 
    
        public void RegisterButton(string id, MainMenuButtonSetting setting, CreationCondition condition, MainMenuButtonCreator createCommand)
        {
            CreatableButton creatableButton;
            if (buttons.TryGetValue(id, out creatableButton))
            {
                UpdateCreatableButton(creatableButton);
                return;
            }

            creatableButton = new CreatableButton(id, setting , condition, createCommand);
            buttons.Add(id, creatableButton);

            void UpdateCreatableButton(CreatableButton button)
            {
                button.setting = setting;
                button.condition = condition;
                button.create = createCommand;
            }
        }

        public void UnregisterButton(string id)
        {
            buttons.Remove(id);
        }
        
        public void CreateButtons(Transform leftColumn, Transform rightColumn, Transform controllersParent)
        {
            var listedButtons = buttons.Values.ToList();
            Sort(listedButtons);

            foreach (var button in listedButtons)
            {
                if (button.condition())
                {
                    var column = button.setting.AlignmentSide == AlignmentSide.Left ? leftColumn : rightColumn;
                    button.create(column, controllersParent);
                }
            }
        }

        private void Sort(List<CreatableButton> creatableButtons)
        {
            creatableButtons.Sort((a, b) => a.setting.Priority < b.setting.Priority ? -1 : 1);
        }
    
    }

}

