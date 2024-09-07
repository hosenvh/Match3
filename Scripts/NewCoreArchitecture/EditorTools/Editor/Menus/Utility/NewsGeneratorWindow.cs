using Match3.Game.NewsSpace;
using UnityEngine;
using UnityEditor;


namespace Match3.EditorTools.Editor.Menus.Utility
{
    public class NewsGeneratorWindow : EditorWindow
    {
        private const string EditorLastNewsId = "EditorLastNewsId";

        private string newsJson;

        private bool manualId;
        private bool overwriteLastId;
        private bool newsGenerated;
        private bool copiedToClipboard;
        private bool stylesInitDone;

        private int newsId;
        private int newsIconIndex;

        private string newsTitle;
        private string newsDescription;
        private string newsLink;

        private float newsSpecificVersion = -1;
        private float newsLessEqualVersion = -1;

        private int lastNewsId = 0;

        private GUIStyle yellowLabelStyle;


        [MenuItem("Golmorad/Utility/News Generator", priority = 524)]
        public static void OpenWindow()
        {
            var window = GetWindow<NewsGeneratorWindow>("News Generator");
            window.Init();
        }

        void Init()
        {
            lastNewsId = EditorPrefs.GetInt(EditorLastNewsId, 0);
        }

        private GUIStyle buttonStyle;

        void InitStyles()
        {
            stylesInitDone = true;
            yellowLabelStyle = new GUIStyle(GUI.skin.label);
            yellowLabelStyle.normal.textColor = Color.yellow;


            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedWidth = 100;
            buttonStyle.fixedHeight = 50;

        }


        private void OnGUI()
        {
            if(!stylesInitDone) InitStyles();

            GUILayout.Space(10);

            DrawGeneratorBody();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            DrawGenerateButton();
            DrawCopyClipboardButton();

            GUILayout.EndHorizontal();

            DrawGeneratedJson();
            DrawClipboardLog();
        }

    //    void DrawTest()
    //    {
    //        var rec = GUILayoutUtility.GetLastRect();
    //        GUILayout.BeginArea(new Rect((position.width * 0.5f)-100, rec.yMax, 200, 50));
    //        GUILayout.BeginHorizontal();
    //        GUILayout.Button("AA");
    //        GUILayout.Button("BB");
    //        GUILayout.EndHorizontal();
    //        GUILayout.EndArea();
    //    }

        void DrawGeneratorBody()
        {
            manualId = GUILayout.Toggle(manualId, "Manual Id");
            if (manualId)
            {
                overwriteLastId = GUILayout.Toggle(overwriteLastId, "Overwrite Last Id");
                newsId = EditorGUILayout.IntField("Id", newsId);
            }

            GUILayout.Space(15);

            newsIconIndex = EditorGUILayout.IntField("Icon Index", newsIconIndex);

            GUILayout.Space(15);

            GUILayout.Label("Title");
            GUILayout.Space(2);
            newsTitle = GUILayout.TextField(newsTitle);

            GUILayout.Space(15);

            GUILayout.Label("Description");
            GUILayout.Space(2);
            newsDescription = GUILayout.TextArea(newsDescription, GUILayout.Height(60));


            GUILayout.Space(15);

            GUILayout.Label("Link");
            GUILayout.Space(2);
            newsLink = GUILayout.TextField(newsLink);

            GUILayout.Space(15);
            newsSpecificVersion = EditorGUILayout.FloatField("Specific Version", newsSpecificVersion);

            GUILayout.Space(5);
            newsLessEqualVersion = EditorGUILayout.FloatField("Less Equal Version", newsLessEqualVersion);
        }

        void DrawGenerateButton()
        {
            if(GUILayout.Button("Generate News Json"))
                GenerateNewsJson();
        }

        void DrawCopyClipboardButton()
        {
            if(GUILayout.Button("Copy Json To Clipboard"))
                CopyToClipboard();
        }


        void GenerateNewsJson()
        {
            if (!manualId)
            {
                ++lastNewsId;
                newsId = lastNewsId;
            }
            else if (overwriteLastId)
            {
                lastNewsId = newsId;
            }

            EditorPrefs.SetInt(EditorLastNewsId, lastNewsId);

            var news = new News()
            {
                title = newsTitle,
                description = newsDescription,
                link = newsLink,
                iconIndex = newsIconIndex,
                id = newsId,
                specificVersion = newsSpecificVersion,
                lessVersion = newsLessEqualVersion
            };

            newsJson = JsonUtility.ToJson(news);
            newsGenerated = true;
            copiedToClipboard = false;
        }

        void CopyToClipboard()
        {
            var textEditor = new TextEditor();
            textEditor.text = newsJson;
            textEditor.SelectAll();
            textEditor.Copy();

            copiedToClipboard = true;
        }

        void DrawGeneratedJson()
        {
            if (newsGenerated)
            {
                GUILayout.Space(15);
                GUILayout.TextArea(newsJson);
            }
        }

        void DrawClipboardLog()
        {
            if (copiedToClipboard)
            {
                GUILayout.Label("Json Copied To Clipboard", yellowLabelStyle);
            }
        }
    }

}