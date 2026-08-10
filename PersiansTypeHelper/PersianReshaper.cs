using System.Text;

namespace PersianTypeHelper
{
    public static class PersianReshaper
    {


        private static readonly string unicode =
          "ﺁﺁﺂﺂ" + "ﺃﺃﺄﺄ" + "ﺇﺇﺈﺈ" + "ﺍﺍﺎﺎ" + "ﺏﺑﺒﺐ" + "ﺕﺗﺘﺖ" + "ﺙﺛﺜﺚ" + "ﺝﺟﺠﺞ" + "ﺡﺣﺤﺢ" + "ﺥﺧﺨﺦ" +
          "ﺩﺩﺪﺪ" + "ﺫﺫﺬﺬ" + "ﺭﺭﺮﺮ" + "ﺯﺯﺰﺰ" + "ﺱﺳﺴﺲ" + "ﺵﺷﺸﺶ" + "ﺹﺻﺼﺺ" + "ﺽﺿﻀﺾ" + "ﻁﻃﻄﻂ" + "ﻅﻇﻈﻆ" +
          "ﻉﻋﻌﻊ" + "ﻍﻏﻐﻎ" + "ﻑﻓﻔﻒ" + "ﻕﻗﻘﻖ" + "ﻙﻛﻜﻚ" + "ﻝﻟﻠﻞ" + "ﻡﻣﻤﻢ" + "ﻥﻧﻨﻦ" + "ﻩﻫﻬﻪ" + "ﻭﻭﻮﻮ" +
          "ﻱﻳﻴﻲ" + "ﺓﺓﺔﺔ" + "ﺅﺅﺆﺆ" + "ﺉﺋﺌﺊ" + "ﻯﻯﻰﻰ" + "گﮔﮕﮓ" + "چﭼﭽﭻ" + "پﭘﭙﭗ" + "ژﮊﮋﮋ" + "ﯼﯾﯿﯽ" +
          "کﮐﮑﮏ" + "ﭪﭬﭭﭫ" + "ﻵﻵﻶﻶ" + "ﻷﻷﻸﻸ" + "ﻹﻹﻺﻺ" + "ﻻﻻﻼﻼ";

        private static readonly string arabic =
            "آ" + "أ" + "إ" + "ا" + "ب" + "ت" + "ث" + "ج" + "ح" + "خ" +
            "د" + "ذ" + "ر" + "ز" + "س" + "ش" + "ص" + "ض" + "ط" + "ظ" +
            "ع" + "غ" + "ف" + "ق" + "ك" + "ل" + "م" + "ن" + "ه" + "و" +
            "ي" + "ة" + "ؤ" + "ئ" + "ى" + "گ" + "چ" + "پ" + "ژ" + "ی" +
            "ک" + "ڤ";

        private static readonly string leftChars = "ڤـئظشسيیبلپتنمكکگطضصثقفغعهخچحج";
        private static readonly string rightChars = "ڤـئؤرلالآىیآةوزژظشسيپبللأاأتنمكکگطضصثقفغعهخحچجدذلإإ";
        private static readonly string harakat = "ًٌٍَُِّْ";
        private static readonly string symbols = "ـ.،؟ @#$%^&*-+|\\/=~,:";
        private static readonly string brackets = "(){}[]";
        private static readonly string arnumbs = "٠١٢٣٤٥٦٧٨٩";
        private static readonly string fanumbs = "۰۱۲۳۴۵۶۷۸۹";
        private static readonly string ennumbs = "0123456789";
        private static readonly int laIndex = 168; 

        private static readonly HashSet<char> notEng = new HashSet<char>();


        public static string ProcessText(string input, bool e_numbers, bool f_numbers, bool e_harakat)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var lines = input.Split('\n');
            var resultLines = new List<string>();

            foreach (var line in lines)
            {
                string processedLine = ProcessLine(line, e_numbers, f_numbers, e_harakat);
                resultLines.Add(processedLine);
            }

            return string.Join("\n", resultLines);
        }




        private static string ProcessLine(string line, bool e_numbers, bool f_numbers, bool e_harakat)
        {
            var pieces = new List<string>(); 
            char[] chars = line.ToCharArray();
            int len = chars.Length;

            for (int g = 0; g < len; g++)
            {
                char current = chars[g];

                
                if (current == '\r')
                    continue;

                
                if (current == '\n')
                {
                    pieces.Add("\n");
                    continue;
                }

                
                int b = 1, a = 1;
                while (g - b >= 0 && harakat.Contains(chars[g - b])) b++;
                while (g + a < len && harakat.Contains(chars[g + a])) a++;

                
                int pos = 0;
                if (g == 0)
                {
                    
                    if (g + a < len && rightChars.Contains(chars[g + a]))
                        pos = 1;
                    else
                        pos = 0;
                }
                else if (g == len - 1)
                {
                    
                    if (g - b >= 0 && leftChars.Contains(chars[g - b]))
                        pos = 3;
                    else
                        pos = 0;
                }
                else
                {
                    bool prevConnectsLeft = (g - b >= 0) && leftChars.Contains(chars[g - b]);
                    bool nextConnectsRight = (g + a < len) && rightChars.Contains(chars[g + a]);

                    if (!prevConnectsLeft)
                    {
                        pos = nextConnectsRight ? 1 : 0;
                    }
                    else
                    {
                        pos = nextConnectsRight ? 2 : 3;
                    }
                }

            
                if (current == 'ء')
                {
                    pieces.Add("ﺀ");
                }
                else if (brackets.Contains(current))
                {
                    int idx = brackets.IndexOf(current);
                    char opposite = (idx % 2 == 0) ? brackets[idx + 1] : brackets[idx - 1];
                    pieces.Add(opposite.ToString());
                }
                else if (arabic.Contains(current))
                {
                    
                    if (current == 'ل' && g + 1 < len)
                    {
                        char nextChar = chars[g + 1];
                        int arPos = arabic.IndexOf(nextChar);
                        if (arPos >= 0 && arPos < 4) 
                        {
                            
                            int ligIndex = (arPos * 4) + pos + laIndex;
                            if (ligIndex < unicode.Length)
                            {
                                pieces.Add(unicode[ligIndex].ToString());
                                g++; 
                            }
                            else
                            {
                                pieces.Add(GetUnicodeForm(current, pos));
                            }
                        }
                        else
                        {
                            pieces.Add(GetUnicodeForm(current, pos));
                        }
                    }
                    else
                    {
                        pieces.Add(GetUnicodeForm(current, pos));
                    }
                }
                else if (symbols.Contains(current))
                {
                    pieces.Add(current.ToString());
                }
                else if (harakat.Contains(current))
                {
                    if (e_harakat)
                        pieces.Add(current.ToString());
                }
                else if (unicode.Contains(current))
                {

                    pieces.Add(current.ToString());
                }
                else
                {

                    var engRun = new StringBuilder();
                    int h = g;
                    while (h < len && !IsSpecialChar(chars[h]))
                    {
                        char c = chars[h];
                        
                        if (ennumbs.Contains(c))
                        {
                            int idx = ennumbs.IndexOf(c);
                            if (e_numbers)
                                c = arnumbs[idx];
                            else if (f_numbers)
                                c = fanumbs[idx];
                        }
                        else if (arnumbs.Contains(c))
                        {
                            int idx = arnumbs.IndexOf(c);
                            if (!e_numbers)
                                c = ennumbs[idx];
                        }
                        else if (fanumbs.Contains(c))
                        {
                            int idx = fanumbs.IndexOf(c);
                            if (!f_numbers)
                                c = ennumbs[idx];
                        }
                        engRun.Append(c);
                        h++;
                    }
                    string run = engRun.ToString();

                    if (run.Length > 1 && run[run.Length - 1] == ' ')
                    {
                        run = " " + run.Substring(0, run.Length - 1);
                    }

                    pieces.Add(run);
                    g = h - 1; 
                }
            }

            pieces.Reverse();
            return string.Concat(pieces);
        }

        private static string GetUnicodeForm(char baseChar, int pos)
        {
            int idx = arabic.IndexOf(baseChar);
            if (idx < 0) return baseChar.ToString();
            int unicodeIdx = idx * 4 + pos;
            if (unicodeIdx >= unicode.Length) return baseChar.ToString();
            return unicode[unicodeIdx].ToString();
        }

        private static bool IsSpecialChar(char c)
        {
            return notEng.Contains(c) || unicode.Contains(c) || brackets.Contains(c);
        }
    }
}

