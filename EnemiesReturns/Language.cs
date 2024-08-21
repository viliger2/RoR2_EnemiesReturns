using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns
{
    public class Language
    {
        public const string LanguageFolder = "Language";

        public static event Action<RoR2.Language, List<KeyValuePair<string, string>>> onCurrentLangaugeChanged;

        public static void Language_onCurrentLanguageChanged()
        {
            RoR2.Language currentLanguage = RoR2.Language.currentLanguage;

            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(EnemiesReturnsPlugin).Assembly.Location), Language.LanguageFolder, RoR2.Language.currentLanguageName);
            if (System.IO.Directory.Exists(path))
            {
                List<KeyValuePair<string, string>> output = new List<KeyValuePair<string, string>>();
                RoR2.Language.LoadAllTokensFromFolder(path, output);
                onCurrentLangaugeChanged?.Invoke(currentLanguage, output);
            }
        }
    }
}
