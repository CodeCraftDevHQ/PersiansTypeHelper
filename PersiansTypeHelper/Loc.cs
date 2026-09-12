namespace PersianTypeHelper
{

    internal static class Loc
    {
  
        public static string S(int appLanguage, string fa, string en) => appLanguage == 1 ? en : fa;
    }
}