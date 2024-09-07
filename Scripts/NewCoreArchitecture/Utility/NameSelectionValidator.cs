namespace Match3.Utility
{
    public static class NameSelectionValidator
    {
        private const int DEFAULT_MINIMUM_VALID_LENGTH = 3;

        public static bool IsSelectedNameValid(string name, int minimumValidLength = DEFAULT_MINIMUM_VALID_LENGTH)
        {
            string selectedProfileName = name.Trim().CleanFromCode().CleanForPersian();
            return selectedProfileName.HasContent(minimumValidLength) && selectedProfileName.IsLetterOrDigit();
        }
    }
}