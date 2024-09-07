using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Game.ServerData;
using UnityEditor;
using UnityEngine;
using Match3.EditorTools.Editor.Utility;


namespace Match3.EditorTools.Editor.Menus.Utility
{
    public class GiftCodeGeneratorWindow : EditorWindow
    {
        private class GiftCodeReward
        {
            public string Name { get; }
            public int Amount { get; set; }

            public GiftCodeReward(string name)
            {
                Name = name;
            }
        }

        private class GiftCodeSetScenarioReward
        {
            public int TargetScenario { get; set; }
        }

        private class GiftCodeSeasonPassGoldenTicketReward
        {
            public bool ShouldMarkGoldenTicketAsPurchased { get; set; }
        }

        private static string giftCodeJson;
        private static GiftCodeSetScenarioReward selectedSetScenarioReward;
        private static GiftCodeSeasonPassGoldenTicketReward selectedGoldenTicketReward;
        private static List<GiftCodeReward> selectedNormalRewards;
        private static List<string> serverRewardsNames;
        private static GUIStyle toggleStyle;
        private static GUIStyle labelStyle;

        [MenuItem("Golmorad/Utility/Gift Code Generator", priority = 523)]
        public static void OpenWindow()
        {
            var window = OpenUnityWindow();
            SetupWindowSize();

            GiftCodeGeneratorWindow OpenUnityWindow() => GetWindow<GiftCodeGeneratorWindow>(title: "Gift Code Generator");

            void SetupWindowSize()
            {
                window.minSize = new Vector2(410, 570);
                window.maxSize = new Vector2(410, 670);
            }
        }

        private void OnEnable()
        {
            InitializeFields();

            void InitializeFields()
            {
                selectedSetScenarioReward = null;
                selectedGoldenTicketReward = null;
                selectedNormalRewards = new List<GiftCodeReward>();
                serverRewardsNames = new List<string> {"coin", "doubleBomb", "rainbow", "tntRainbow", "infiniteDoubleBomb", "infiniteRainbow", "infiniteTntRainbow", "infiniteLife", "hammer", "broom", "hand", "seasonPassBadge"};
                toggleStyle = new GUIStyle(other: EditorStyles.toggle) {fixedWidth = 190, fontStyle = FontStyle.Bold};
                labelStyle = new GUIStyle(other: EditorStyles.label) {fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState {textColor = Color.yellow}};
            }
        }

        private void OnGUI()
        {
            InsertRewardsSelectionSection();
            UpdateGiftCodeJson();
            InsertGeneratedGiftCodeJson();
            InsertCopyJsonToClipboardButton();
        }

        private void InsertRewardsSelectionSection()
        {
            InsertBigEmptySpace();
            InsertRewardsSectionLabel();
            InsertSmallEmptySpace();

            InsertAllNormalRewardsSelectionSection();
            InsertSetScenarioRewardSelectionSection();
            InsertGoldenTickerRewardSelectionSection();

            void InsertRewardsSectionLabel() => InsertLabel(text: "Select Rewards");
        }

        private void InsertAllNormalRewardsSelectionSection()
        {
            foreach (string rewardName in serverRewardsNames)
            {
                GUILayout.BeginHorizontal();
                InsertNormalRewardSelectionSectionFor(rewardName);
                GUILayout.EndHorizontal();
                InsertSmallEmptySpace();
            }

            void InsertNormalRewardSelectionSectionFor(string rewardName)
            {
                InsertToggle(enablenessEvaluator: IsRewardSelected, title: rewardName.SplitCamelCase().FirstCharToUpper(), onEnable: SelectReward, onDisable: DeselectReward);
                if (IsRewardSelected())
                    InsertRewardAmountField();

                bool IsRewardSelected() => GetSelectedGiftCodeReward() != null;

                void SelectReward()
                {
                    if (IsRewardSelected() == false)
                        selectedNormalRewards.Add(new GiftCodeReward(rewardName));
                }

                void DeselectReward() => selectedNormalRewards.RemoveAll(reward => reward.Name == rewardName);
                GiftCodeReward GetSelectedGiftCodeReward() => selectedNormalRewards.Find(reward => reward.Name == rewardName);

                void InsertRewardAmountField()
                {
                    EditorGUIUtility.labelWidth = 70;
                    GetSelectedGiftCodeReward().Amount = EditorGUILayout.IntField(label: "Amount:", GetSelectedGiftCodeReward().Amount, GUILayout.ExpandWidth(false));
                }
            }
        }

        private void InsertSetScenarioRewardSelectionSection()
        {
            GUILayout.BeginHorizontal();
            InsertNormalRewardSelectionSection();
            GUILayout.EndHorizontal();
            InsertSmallEmptySpace();

            void InsertNormalRewardSelectionSection()
            {
                InsertToggle(enablenessEvaluator: IsSetScenarioRewardSelected, title: "Set Scenario", onEnable: SelectSetScenarioReward, onDisable: DeselectSetScenarioReward);
                if (IsSetScenarioRewardSelected())
                    InsertSetScenarioRewardTargetScenarioField();

                bool IsSetScenarioRewardSelected() => selectedSetScenarioReward != null;

                void SelectSetScenarioReward()
                {
                    if (selectedSetScenarioReward == null)
                        selectedSetScenarioReward = new GiftCodeSetScenarioReward();
                }

                void DeselectSetScenarioReward() => selectedSetScenarioReward = null;

                void InsertSetScenarioRewardTargetScenarioField()
                {
                    EditorGUIUtility.labelWidth = 100;
                    selectedSetScenarioReward.TargetScenario = EditorGUILayout.IntField(label: "Target Scenario:", selectedSetScenarioReward.TargetScenario, GUILayout.ExpandWidth(false));
                }
            }
        }

        private void InsertGoldenTickerRewardSelectionSection()
        {
            GUILayout.BeginHorizontal();
            InsertGoldenTicketSelectionSection();
            GUILayout.EndHorizontal();
            InsertSmallEmptySpace();

            void InsertGoldenTicketSelectionSection()
            {
                InsertToggle(enablenessEvaluator: IsGoldenTicketRewardSelected, title: "Golden Ticket", onEnable: SelectGoldenTicketReward, onDisable: DeselectGoldenTicketReward);
                if (IsGoldenTicketRewardSelected())
                    InsertGoldenTicketRewardTargetScenarioField();

                bool IsGoldenTicketRewardSelected() => selectedGoldenTicketReward != null;

                void SelectGoldenTicketReward()
                {
                    if (selectedGoldenTicketReward == null)
                        selectedGoldenTicketReward = new GiftCodeSeasonPassGoldenTicketReward();
                }

                void DeselectGoldenTicketReward() => selectedGoldenTicketReward = null;

                void InsertGoldenTicketRewardTargetScenarioField()
                {
                    EditorGUIUtility.labelWidth = 100;
                    selectedGoldenTicketReward.ShouldMarkGoldenTicketAsPurchased = EditorGUILayout.Toggle(label: "Season Pass Event Id:", selectedGoldenTicketReward.ShouldMarkGoldenTicketAsPurchased, GUILayout.ExpandWidth(false));
                }
            }
        }

        private void UpdateGiftCodeJson()
        {
            GiftCodeRewardConfig giftCodeConfig = CreateGiftCodeConfig();
            giftCodeJson = ConvertGiftCodeConfigToJson();

            GiftCodeRewardConfig CreateGiftCodeConfig()
            {
                return new GiftCodeRewardConfig()
                       {
                           rewards = CreateServerRewardsFromSelectedNormalRewards().ToArray(),
                           setScenario = selectedSetScenarioReward != null ? new GiftCodeRewardConfig.SetScenarioConfig() {targetScenarioIndex = selectedSetScenarioReward.TargetScenario} : null,
                           goldenTicket = selectedGoldenTicketReward != null ? new GiftCodeRewardConfig.GoldenTicketConfig() {shouldMarkGoldenTicketAsPurchased = selectedGoldenTicketReward.ShouldMarkGoldenTicketAsPurchased} : null,
                       };

                List<ServerReward> CreateServerRewardsFromSelectedNormalRewards() => selectedNormalRewards.Select(selectedNormalReward => new ServerReward {name = selectedNormalReward.Name, value = selectedNormalReward.Amount}).ToList();
            }

            string ConvertGiftCodeConfigToJson()
            {
                string result = JsonUtility.ToJson(giftCodeConfig);
                CleanUpGeneratedJson();
                return result;

                void CleanUpGeneratedJson()
                {
                    RemoveSetScenarioConfigIfNotUsed();
                    RemoveGoldenTicketIfNotUsed();

                    void RemoveSetScenarioConfigIfNotUsed() => result = result.Replace(",\"setScenario\":{\"targetScenarioIndex\":0}", String.Empty);
                    void RemoveGoldenTicketIfNotUsed() => result = result.Replace(",\"goldenTicket\":{\"shouldMarkGoldenTicketAsPurchased\":false}", String.Empty);
                }
            }
        }


        private void InsertGeneratedGiftCodeJson()
        {
            InsertMediumEmptySpace();
            InsertLabel(text: "Result Json:");
            InsertTextArea(text: giftCodeJson);

            void InsertTextArea(string text) => GUILayout.TextArea(text);
        }

        private void InsertCopyJsonToClipboardButton()
        {
            InsertSmallEmptySpace();
            InsertButton(title: "Copy Json To Clipboard", onClick: CopyToClipboard);

            void CopyToClipboard()
            {
                var textEditor = new TextEditor();
                textEditor.text = giftCodeJson;
                textEditor.SelectAll();
                textEditor.Copy();
            }
        }

        private void InsertToggle(Func<bool> enablenessEvaluator, string title, Action onEnable, Action onDisable) => EditorUtilities.InsertToggle(enablenessEvaluator, title, onEnable, onDisable, toggleStyle);
        private void InsertButton(string title, Action onClick) => EditorUtilities.InsertButton(title, onClick);
        private void InsertLabel(string text) => EditorUtilities.InsertLabel(text, labelStyle, GUILayout.ExpandWidth(true));
        private void InsertSmallEmptySpace() => EditorUtilities.InsertEmptySpace(pixels: 10);
        private void InsertMediumEmptySpace() => EditorUtilities.InsertEmptySpace(pixels: 20);
        private void InsertBigEmptySpace() => EditorUtilities.InsertEmptySpace(pixels: 40);
    }
}