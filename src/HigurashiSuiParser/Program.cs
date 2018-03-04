using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigurashiSuiParser
{
    class Program
    {
        static HashSet<String> translations = new HashSet<String>();
        static void Main(string[] args)
        {
            String ps3ScriptPath = @"D:\Downloads\07th Modding\Higurashi Files\HigurashiPS3-Script\in.txt";
            String outFilePath = @"D:\Downloads\07th Modding\Higurashi Files\HigurashiPS3-Script\out.utf";
            String textOnlyFilePath = @"D:\Downloads\07th Modding\Higurashi Files\HigurashiPS3-Script\text.utf";
            // Read the file and display it line by line.
            System.IO.StreamReader ps3Script =
                new System.IO.StreamReader(ps3ScriptPath);
            
            System.IO.StreamWriter outFile =
                new System.IO.StreamWriter(outFilePath, false, Encoding.UTF8);

            System.IO.StreamWriter textOnlyFile =
                new System.IO.StreamWriter(textOnlyFilePath, false, Encoding.UTF8);

            String line = "";
            while ((line = ps3Script.ReadLine()) != null)
            {
                if (line.Equals(""))
                    continue; //ignore blank lines
                outFile.WriteLine("//" + line);

                if (line.StartsWith("#") || !line.Contains("r"))
                {
                    //do nothing with chapter titles, and lines that don't contain an r
                    //#1 羽入のいない世界
                    //or
                    //デバッグ選択肢
                    outFile.WriteLine();
                    continue;
                }

                line = cleanOutKs(line); //replace any names containing the letter k so we can split on ks easily

                int cursor = 0;
                while (cursor < line.Length)
                {
                    if (line[cursor].Equals('r'))
                        break;
                    cursor++;
                }
                line = line.Substring(cursor+1); //move the line to the first r

                if (line.Contains("k"))
                {
                    //this is a multi-line, split and handle each separately

                    String[] s = line.Split('k');
                    for (int i=0;i<s.Length;i++)
                    {
                        outputLine(outFile, textOnlyFile, s[i], i==(s.Length-1)?true:false);
                    }
                    outputClearMessage(outFile);
                    
                }
                else
                {
                    outputLine(outFile, textOnlyFile, line, true);
                    outputClearMessage(outFile);
                }
                outFile.Flush();
                textOnlyFile.Flush();
            }
            outFile.Close();
            textOnlyFile.Close();
            ps3Script.Close();
        }
        static String cleanOutKs(String line)
        {
            line = line.Replace("rika", "ri*a");
            line = line.Replace("satoko", "sato*o");
            line = line.Replace("makino", "ma*ino");
            line = line.Replace("akasaka", "a*asa*a");
            line = line.Replace("miyuki", "miyu*i");
            line = line.Replace("keiichi", "*eiichi");
            line = line.Replace("yukie", "yu*ie");
            line = line.Replace("kanshiki", "*anshi*i");
            line = line.Replace("kan", "*an");
            line = line.Replace("okono", "o*ono");
            line = line.Replace("ootaka", "oota*a");
            line = line.Replace("yamaoki", "yamao*i");
            line = line.Replace("kumagai", "*umagai");
            line = line.Replace("kataoka", "*atao*a");
            line = line.Replace("oka", "o*a");
            line = line.Replace("miotak", "miota*");
            line = line.Replace("akira", "a*ira");
            line = line.Replace("hatake", "hata*e");
            line = line.Replace("akiyama", "a*iyama");
            line = line.Replace("takaya", "ta*aya");
            line = line.Replace("keiji", "*eiji");
            line = line.Replace("tsukada", "tsu*ada");
            line = line.Replace("kenji", "*enji");
            line = line.Replace("koutsu", "*outsu");
            line = line.Replace("yoshika", "yoshi*a");
            line = line.Replace("kisou", "*isou");
            line = line.Replace("eriko", "eri*o");
            line = line.Replace("syokuin", "syo*uin");
            line = line.Replace("tomitake", "tomita*e");
            line = line.Replace("kanryou", "*anryou");
            line = line.Replace("koizumi", "*oizumi");
            line = line.Replace("nakagawa", "na*agawa");
            line = line.Replace("yaku", "ya*u");
            line = line.Replace("keiuke", "*eiu*e");
            line = line.Replace("keisho", "*eisho");
            line = line.Replace("keika", "*ei*a");
            line = line.Replace("madoka", "mado*a");
            line = line.Replace("koucho", "*oucho");
            line = line.Replace("nikuya", "ni*uya");
            line = line.Replace("hoken", "ho*en");
            line = line.Replace("gaiko", "gai*o");
            line = line.Replace("monka", "mon*a");
            line = line.Replace("sakagu", "sa*agu");
            line = line.Replace("kouhou", "*ouhou");
            line = line.Replace("miyoko", "miyo*o");
            line = line.Replace("kenq", "*enq");
            line = line.Replace("kidou", "*idou");
            line = line.Replace("tamako", "tama*o");
            line = line.Replace("arakawa", "ara*awa");
            line = line.Replace("riku", "ri*u");
            line = line.Replace("ouka", "ou*a");
            line = line.Replace("kimi", "*imi");
            line = line.Replace("shimiko", "shimi*o");
            line = line.Replace("saeki", "sae*i");
            line = line.Replace("chiaki", "chia*i");
            line = line.Replace("machiko", "machi*o");
            line = line.Replace("akuta", "a*uta");
            line = line.Replace("kei", "*ei");
            line = line.Replace("work", "wor*");
            line = line.Replace("dek", "de*");
            line = line.Replace("juku", "ju*u");
            line = line.Replace("kyoku", "*yo*u");
            line = line.Replace("komi", "*omi");
            line = line.Replace("oku", "o*u");
            line = line.Replace("kisya", "*isya");
            line = line.Replace("kaigo", "*aigo");
            line = line.Replace("inuk", "inu*");
            line = line.Replace("buka", "bu*a");
            line = line.Replace("shinko", "shin*o");
            line = line.Replace("syoki", "syo*i");
            line = line.Replace("kameda", "*ameda");
            line = line.Replace("komura", "*omura");
            line = line.Replace("nakai", "na*ai");
            line = line.Replace("kaori", "*aori");
            line = line.Replace("taka", "ta*a");
            line = line.Replace("kumiin", "*umiin");
            line = line.Replace("kuma", "*uma");
            line = line.Replace("desk", "des*");
            line = line.Replace("kisha", "*isha");
            line = line.Replace("kuro", "*uro");
            line = line.Replace("kinYu", "*inYu");
            line = line.Replace("Tkin", "T*in");
            line = line.Replace("!k!", "!*!");
            line = line.Replace("shikai", "shi*ai");
            line = line.Replace("kacho", "*acho");
            line = line.Replace("kojima", "*ojima");
            line = line.Replace("kishim", "*ishim");
            line = line.Replace("kaji", "*aji");
            line = line.Replace("Huke", "*Hu*e");
            line = line.Replace("kouan", "*ouan");
            line = line.Replace("fuuka", "fuu*a");
            line = line.Replace("VTH_k", "VTH_*");
            line = line.Replace("uke", "u*e");
            


            return line;
        }

        static String replaceKs(String line)
        {
            line = line.Replace("*", "k");

            return line;
        }

        static void outputClearMessage(System.IO.StreamWriter outFile)
        {
            outFile.WriteLine("\tClearMessage();\n");
        }

        static void outputLine(System.IO.StreamWriter outFile, System.IO.StreamWriter textOnlyFile, string line, bool lastLine)
        {
            String lineStyle;

            if (lastLine)
                lineStyle = "Line_Normal";
            else
                lineStyle = "Line_WaitForInput";

            line = replaceKs(line);

            while (line.Contains("b") && line.Contains(".") && line.Contains("<") && line.Contains(">"))
            {
                int bIndex = line.IndexOf("b");
                int dotIndex = line.Substring(bIndex).IndexOf(".")+ bIndex;
                int lessIndex = line.IndexOf("<");
                int greaterIndex = line.IndexOf(">");

                String newLine = line.Substring(0, bIndex) + line.Substring(lessIndex + 1, greaterIndex - lessIndex-1) + "(" + line.Substring(bIndex + 1, dotIndex - bIndex-1) + ")" + line.Substring(greaterIndex+1);
                line = newLine;
            }

            line = line.Replace(@"―", "—");

            if (line.StartsWith("r"))
                line = line.Substring(1);
            if (line.Contains("v") && !line.Contains("!R!a!b!i!e!s!v!i!r!u!s") && !line.Contains("!2!0!0!6!"))
            {
                int start = line.IndexOf("v");
                int end = line.LastIndexOf(".");
                String voiceRegion = line.Substring(start+1, end - start - 1);
                String textRegion = line.Substring(end+1);

                if (line.Contains("|"))
                {
                    //this is a multi-voiced line

                    String[]vSplit = voiceRegion.Split('|');
                    for (int i=0;i< vSplit.Length;i++)
                    {
                        String se = "\tPlaySE(" + (i+4) + ", \"" + vSplit[i] + "\", 256, 64);";
                        outFile.WriteLine(se);
                    }
                    String output = "\tOutputLine(NULL, \"" + textRegion + "\",\n\t\t\tNULL, \"\", "+ lineStyle+"); ";
                    outFile.WriteLine(output);
                    if (translations.Contains(textRegion))
                    {
                        Console.WriteLine("");
                    }
                    else
                    {
                        translations.Add(textRegion);
                        textOnlyFile.WriteLine(textRegion);
                    }
                    
                }
                else
                {
                    //this is a single-voice line
                    String se = "\tPlaySE(4, \"" + voiceRegion + "\", 256, 64);";
                    outFile.WriteLine(se);
                    String output = "\tOutputLine(NULL, \"" + textRegion + "\",\n\t\t\tNULL, \"\", "+ lineStyle+"); ";
                    outFile.WriteLine(output);
                    
                    if (translations.Contains(textRegion))
                    {
                        Console.WriteLine("");
                    }
                    else
                    {
                        translations.Add(textRegion);
                        textOnlyFile.WriteLine(textRegion);
                    }
                }
            }
            else
            {
                String output = "\tOutputLine(NULL, \"" + line + "\",\n\t\t\tNULL, \"\", "+ lineStyle+"); ";
                //this is an unvoiced line
                outFile.WriteLine(output);
                
                if (translations.Contains(line))
                {
                    Console.WriteLine("");
                }
                else
                {
                    translations.Add(line);
                    textOnlyFile.WriteLine(line);
                }
            }
        }
    }
}
