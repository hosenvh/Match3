using UnityEngine;


namespace Match3.EditorTools.Editor.Utility
{
    public static class EditorTypesExtensions
    {
        public static void SetBackgroundTexture(this GUIStyle self, Texture normalTexture, Texture hoverTexture = null)
        {
            self.SetBackgroundTexture((Texture2D) normalTexture, (Texture2D) hoverTexture);
        }

        public static void SetBackgroundTexture(this GUIStyle self, Texture2D normalTexture, Texture2D hoverTexture)
        {
            SetGuiStyleStateTexture(self.normal, normalTexture);
            SetGuiStyleStateTexture(self.hover, hoverTexture);
        }

        private static void SetGuiStyleStateTexture(GUIStyleState guiStyleState, Texture2D texture)
        {
            guiStyleState.background = texture;
            guiStyleState.scaledBackgrounds = null;
        }

        public static void SetBackgroundColor(this GUIStyle self, Color normalColor)
        {
            self.SetBackgroundColor(normalColor, hoverColor: null);
        }

        public static void SetBackgroundColor(this GUIStyle self, Color normalColor, Color? hoverColor)
        {
            SetGuiStyleStateColor(self.normal, normalColor);
            if (hoverColor != null)
                SetGuiStyleStateColor(self.hover, (Color) hoverColor);

            void SetGuiStyleStateColor(GUIStyleState guiStyleState, Color color)
            {
                SetGuiStyleStateTexture(guiStyleState, texture: MakeTex(1, 1, color));

                Texture2D MakeTex(int width, int height, Color col)
                {
                    Color[] pixelColors = new Color[width * height];
                    for (int i = 0; i < pixelColors.Length; ++i)
                        pixelColors[i] = col;
                    Texture2D resultTexture = new Texture2D(width, height);
                    resultTexture.SetPixels(pixelColors);
                    resultTexture.Apply();
                    return resultTexture;
                }
            }
        }
    }
}