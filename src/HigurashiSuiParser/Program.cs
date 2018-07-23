using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HigurashiSuiParser
{
    class Program
    {
        enum colorMode { Regular, Night, Sunset };

        static HashSet<String> translations = new HashSet<String>();
        static void Main(string[] args)
        {
            String xmlScriptPath = @"D:\Downloads\07th Modding\sui_output_joined\sui_output_tokihogushi.xml"; //input
            String CGfolder = @"D:\Steam\steamapps\common\Higurashi 04 - Himatsubushi\HigurashiEp04_Data\StreamingAssets\CG"; //input
            String spritesDirectory = @"D:\Downloads\07th Modding\Higurashi Files\HigurashiPS3-Sprites\m"; //input
            String outFilePath = @"D:\Downloads\07th Modding\Higurashi Files\HigurashiPS3-Script\toki_"; //output
            String textOnlyFilePath = @"D:\Downloads\07th Modding\Higurashi Files\HigurashiPS3-Script\text.utf"; //output
            String filesUsedFilePath = @"D:\Downloads\07th Modding\Higurashi Files\HigurashiPS3-Script\filesUsed.utf"; //output
            String arcName = "Tokihogushi";

            List<String> cgFilesList = new List<String>();
            List<String> seFilesList = new List<String>();
            List<String> bgmFilesList = new List<String>();

            int outFileNbr = 1;
            bool beforeFirstChapter = true;
            bool channel4Active = false;
            bool channel7Active = false;
            bool bgmActive = false;
            bool slot5Active = false;
            bool slot6Active = false;
            bool slot7Active = false;
            String reg17 = "";
            String reg18 = "";
            colorMode activeMode = colorMode.Regular;

            Dictionary<String, String> spriteMapping = new Dictionary<String, String>();

            //Build a sprite file name mapping, because PS3 script uses file names like "m/Si1ADefA1" but we use "si1a_defa1_0"
            foreach (String s in Directory.GetFiles(spritesDirectory))
            {
                int lastSlash = s.LastIndexOf('\\');
                String formattedFileName = s.Substring(lastSlash+1);
                String actualFileName = "";

                actualFileName = formattedFileName;

                actualFileName = actualFileName.Replace("A1", "_a1");
                actualFileName = actualFileName.Replace("A2", "_a2");
                actualFileName = actualFileName.Replace("B1", "_b1");
                actualFileName = actualFileName.Replace("B2", "_b2");
                formattedFileName = formattedFileName.Substring(0, formattedFileName.Length - 6);
                actualFileName = actualFileName.Replace("0.png", "0");
                actualFileName = actualFileName.Replace("1.png", "0");
                actualFileName = actualFileName.Replace("2.png", "0");
                actualFileName = actualFileName.ToLower();

                formattedFileName = formattedFileName.Replace("_", "");
                formattedFileName = formattedFileName.ToLower();

                if (!spriteMapping.ContainsKey(formattedFileName))
                {
                    spriteMapping.Add(formattedFileName, actualFileName);
                }
            }

            XmlTextReader reader = new XmlTextReader(xmlScriptPath);

            System.IO.StreamWriter outFile =
                new System.IO.StreamWriter(GetOutFilePath(outFilePath, outFileNbr), false, Encoding.UTF8);

            OutputFileHeader(outFile, arcName, outFileNbr);

            System.IO.StreamWriter textOnlyFile =
                new System.IO.StreamWriter(textOnlyFilePath, false, Encoding.UTF8);

            System.IO.StreamWriter filesUsed =
                new System.IO.StreamWriter(filesUsedFilePath, false, Encoding.UTF8);

            String line = "";
            int lineNum = 0;
            while (reader.Read())
            {
                if (!reader.Name.Equals("ins"))
                    continue;

                string type = "";
                if (reader.HasAttributes)
                    type = reader.GetAttribute("type");

                line = null;
                if (type.Equals("DIALOGUE"))
                {
                    line = reader.GetAttribute("data");
                    processDialogLine(line, outFile, textOnlyFile);
                }
                else if (type.Equals("SECTION_START"))
                {
                    line = reader.GetAttribute("section");
                    type = reader.GetAttribute("section_type");

                    if (type.Equals("CHAPTER"))
                    {
                        if (beforeFirstChapter)
                        {
                            //don't bother with rollover on the very first chapter
                            //this ensures the text before the section is still output and the file numbers start at 1
                            beforeFirstChapter = false;
                        }
                        else
                        {
                            //rollover to the next script file and increment the file number
                            OutputFileFooter(outFile);
                            outFile.Close();
                            outFileNbr++;
                            outFile = new System.IO.StreamWriter(GetOutFilePath(outFilePath, outFileNbr), false, Encoding.UTF8);
                            OutputFileHeader(outFile, arcName, outFileNbr);
                        }
                    }

                    processDialogLine(line, outFile, textOnlyFile);
                }
                else if (type.Equals("PIC_LOAD"))
                {
                    string file = reader.GetAttribute("file").ToLower();
                    if (file.StartsWith("e/")) //this is a CG image
                    {
                        file = file.Substring(2);
                        outFile.WriteLine("	DrawScene(\"scene\\" + file + "\", 1000 );\r\n");
                    }
                    else
                    {
                        outFile.WriteLine("	DrawScene(\"background\\" + file + "\", 1000 );\r\n");
                    }

                    slot5Active = false;
                    slot6Active = false;
                    slot7Active = false;

                    if (!cgFilesList.Contains(file))
                        cgFilesList.Add(file);
                }
                else if (type.Equals("LOAD_SIMPLE"))
                {
                    outFile.WriteLine("	DrawScene(\"black\", 1000 );\r\n");

                    slot5Active = false;
                    slot6Active = false;
                    slot7Active = false;
                }
                else if (type.Equals("SFX_PLAY"))
                {
                    string sfxfile = reader.GetAttribute("sfx_file").ToLower();
                    string sfxchannel = reader.GetAttribute("sfx_channel").ToLower();
                    string singleplay = reader.GetAttribute("single_play");
                    if (singleplay.Equals("0"))
                    {
                        if (sfxchannel.Equals("4"))
                        {
                            channel4Active = true;
                            outFile.WriteLine("	PlayBGM( 1, \"" + sfxfile + "\", 128, 0 );\r\n");
                        }
                        else if (sfxchannel.Equals("7"))
                        {
                            channel7Active = true;
                            outFile.WriteLine("	PlayBGM( 0, \"" + sfxfile + "\", 128, 0 );\r\n");
                        }
                        
                        if (!bgmFilesList.Contains(sfxfile))
                            bgmFilesList.Add(sfxfile);
                    }
                    else
                    {
                        outFile.WriteLine("	PlaySE(3, \"" + sfxfile + "\", 256, 64);\r\n");
                        if (!seFilesList.Contains(sfxfile))
                            seFilesList.Add(sfxfile);
                    }
                    
                }
                else if (type.Equals("CHANNEL_FADE"))
                {
                    string sfxchannel = reader.GetAttribute("channel").ToLower();
                    if (sfxchannel.Equals("4") && channel4Active)
                    {
                        channel4Active = false;
                        outFile.WriteLine("	FadeOutBGM(1, 200, TRUE);\r\n");
                        
                    }
                    else if (sfxchannel.Equals("7") && channel7Active)
                    {
                        channel7Active = false;
                        outFile.WriteLine("	FadeOutBGM(0, 200, TRUE);\r\n");
                    }
                }
                else if (type.Equals("BGM_PLAY"))
                {
                    string bgmfile = reader.GetAttribute("bgm_file").ToLower();
                    bgmActive = true;
                    outFile.WriteLine("	PlayBGM(2, \"" + bgmfile + "\", 128, 0);\r\n");

                    if (!bgmFilesList.Contains(bgmfile))
                        bgmFilesList.Add(bgmfile);

                }
                else if (type.Equals("BGM_FADE"))
                {
                    if (bgmActive)
                    {
                        bgmActive = false;
                        outFile.WriteLine("	FadeOutBGM(2, 200, FALSE);\r\n");
                    }
                    
                }
                else if (type.Equals("SPRITE_LOAD"))
                {
                    string slot = reader.GetAttribute("slot");
                    string file = reader.GetAttribute("file");
                    string actualFile = "";

                    bool isZoomSprite = file.StartsWith("l/");

                    file = file.Replace("_", "");
                    file = file.Replace("m/", "");
                    file = file.ToLower();
                    spriteMapping.TryGetValue(file, out actualFile);
                    if (activeMode == colorMode.Night)
                        actualFile = "night\\" + actualFile;
                    else if (activeMode == colorMode.Sunset)
                        actualFile = "sunset\\" + actualFile;
                    else
                        actualFile = "normal\\" + actualFile;

                    if (isZoomSprite)
                        actualFile = "portrait\\" + actualFile;
                    else
                        actualFile = "sprite\\" + actualFile;

                    if (slot.Equals("5"))
                    {
                        slot5Active = true;
                        outFile.WriteLine("	DrawBustshot( 3, \"" + actualFile + "\", 160, 0, 0, FALSE, 0, 0, 0, 0, 0, 0, 0, 20, 200, TRUE );\r\n");
                    }
                    else if (slot.Equals("6"))
                    {
                        slot6Active = true;
                        outFile.WriteLine("	DrawBustshot( 4, \"" + actualFile + "\", 0, 0, 0, FALSE, 0, 0, 0, 0, 0, 0, 0, 19, 200, TRUE );\r\n");
                    }
                    else if (slot.Equals("7"))
                    {
                        slot7Active = true;
                        outFile.WriteLine("	DrawBustshot( 5, \"" + actualFile + "\", -160, 0, 0, FALSE, 0, 0, 0, 0, 0, 0, 0, 18, 200, TRUE );\r\n");
                    }

                    if (!cgFilesList.Contains(actualFile))
                        cgFilesList.Add(actualFile);

                }
                else if (type.Equals("REMOVE_SLOT"))
                {
                    string slot = reader.GetAttribute("slot");

                    if (slot.Equals("5") && slot5Active)
                    {
                        outFile.WriteLine("	FadeBustshot(3, FALSE, 0, 0, 0, 0, 200, TRUE);\r\n");
                        slot5Active = false;
                    }
                    else if (slot.Equals("6") && slot6Active)
                    {
                        outFile.WriteLine("	FadeBustshot(4, FALSE, 0, 0, 0, 0, 200, TRUE);\r\n");
                        slot6Active = false;
                    }
                    if (slot.Equals("7") && slot7Active)
                    {
                        outFile.WriteLine("	FadeBustshot(5, FALSE, 0, 0, 0, 0, 200, TRUE);\r\n");
                        slot7Active = false;
                    }
                }
                else if (type.Equals("REGISTER_MODIFY"))
                {
                    string data = reader.GetAttribute("data");
                    string[] splitData = data.Split('=');
                    if (splitData[0].Equals("reg17"))
                        reg17 = splitData[1];
                    else if (splitData[0].Equals("reg18"))
                        reg18 = splitData[1];

                    if (reg17.Equals("196u;") && reg18.Equals("196u;"))
                        activeMode = colorMode.Night;
                    else if (reg17.Equals("255u;") && reg18.Equals("232u;"))
                        activeMode = colorMode.Sunset;
                    else
                        activeMode = colorMode.Regular;
                }

                lineNum++;

                outFile.Flush();
                textOnlyFile.Flush();
            }

            OutputFileFooter(outFile);
            outFile.Flush();

            filesUsed.WriteLine("Images needed in CG directory:\r\n");
            cgFilesList.Sort();
            foreach (String s in cgFilesList)
            {
                String actualFile = "";
                spriteMapping.TryGetValue(s, out actualFile);
                if (actualFile == null)
                    actualFile = s;
                bool found = false;
                foreach (String s2 in Directory.GetFiles(CGfolder, "*", SearchOption.AllDirectories))
                {
                    int lastSlash = s2.LastIndexOf('\\');
                    String formattedFileName = s2.Substring(lastSlash + 1);
                    formattedFileName = formattedFileName.Substring(0, formattedFileName.Length - 4);
                    if (formattedFileName.ToLower().Equals(actualFile.ToLower()))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    String CGfolder2 = CGfolder + "\\sprite";
                    foreach (String s2 in Directory.GetFiles(CGfolder2, "*", SearchOption.AllDirectories))
                    {
                        int lastSlash = s2.IndexOf("\\sprite");
                        String formattedFileName = s2.Substring(lastSlash + 1);
                        formattedFileName = formattedFileName.Substring(0, formattedFileName.Length - 4);
                        if (formattedFileName.ToLower().Equals(actualFile.ToLower()))
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    if (actualFile==null)
                        filesUsed.WriteLine(s);
                    else
                        filesUsed.WriteLine(actualFile);
                }
            }
            filesUsed.WriteLine("Music needed in BGM directory:\r\n");
            bgmFilesList.Sort();
            foreach (String s in bgmFilesList)
            {
                filesUsed.WriteLine(s);
            }
            filesUsed.WriteLine("Audio needed in SE directory:\r\n");
            seFilesList.Sort();
            foreach (String s in seFilesList)
            {
                filesUsed.WriteLine(s);
            }
            filesUsed.Flush();

            filesUsed.Close();
            outFile.Close();
            textOnlyFile.Close();
            reader.Close();
        }

        static void processDialogLine(String line, System.IO.StreamWriter outFile, System.IO.StreamWriter textOnlyFile)
        {
            if (line == null || line.Equals(""))
                return; //ignore blank lines

            line = fixEncoding(line);
            outFile.WriteLine("//" + line);

            if (line.StartsWith("#") || !line.Contains("r"))
            {
                //do nothing with chapter titles, and lines that don't contain an r
                //#1 羽入のいない世界
                //or
                //デバッグ選択肢
                outFile.WriteLine();
                return;
            }

            line = cleanOutKs(line); //replace any names containing the letter k so we can split on ks easily
            line = cleanOutBs(line);

            int cursor = 0;
            while (cursor < line.Length)
            {
                if (line[cursor].Equals('r'))
                    break;
                cursor++;
            }
            line = line.Substring(cursor + 1); //move the line to the first r

            if (line.Contains("k"))
            {
                //this is a multi-line, split and handle each separately

                String[] s = line.Split('k');
                for (int i = 0; i < s.Length; i++)
                {
                    outputLine(outFile, textOnlyFile, s[i], i == (s.Length - 1) ? true : false);
                }
                outputClearMessage(outFile);

            }
            else
            {
                outputLine(outFile, textOnlyFile, line, true);
                outputClearMessage(outFile);
            }
        }
        static String fixEncoding(String line)
        {
            line = line.Replace('ｱ', 'あ');
            line = line.Replace('ｲ', 'い');
            line = line.Replace('ｳ', 'う');
            line = line.Replace('ｴ', 'え');
            line = line.Replace('ｵ', 'お');
            line = line.Replace('ｶ', 'か');
            line = line.Replace('ｷ', 'き');
            line = line.Replace('ｸ', 'く');
            line = line.Replace('ｹ', 'け');
            line = line.Replace('ｺ', 'こ');
            //line = line.Replace('', 'が');
            //line = line.Replace('', 'ぎ');
            //line = line.Replace('', 'ぐ');
            //line = line.Replace('', 'げ');
            //line = line.Replace('', 'ご');
            line = line.Replace('ｻ', 'さ');
            line = line.Replace('ｼ', 'し');
            line = line.Replace('ｽ', 'す');
            line = line.Replace('ｾ', 'せ');
            line = line.Replace('ｿ', 'そ');
            //line = line.Replace('', 'ざ');
            //line = line.Replace('', 'じ');
            //line = line.Replace('', 'ず');
            //line = line.Replace('', 'ぜ');
            //line = line.Replace('', 'ぞ');
            line = line.Replace('ﾀ', 'た');
            line = line.Replace('ﾁ', 'ち');
            line = line.Replace('ﾂ', 'つ');
            line = line.Replace('ﾃ', 'て');
            line = line.Replace('ﾄ', 'と');
            //line = line.Replace('', 'だ');
            //line = line.Replace('', 'ぢ');
            //line = line.Replace('', 'づ');
            //line = line.Replace('', 'で');
            //line = line.Replace('', 'ど');
            line = line.Replace('ﾅ', 'な');
            line = line.Replace('ﾆ', 'に');
            line = line.Replace('ﾇ', 'ぬ');
            line = line.Replace('ﾈ', 'ね');
            line = line.Replace('ﾉ', 'の');
            line = line.Replace('ﾊ', 'は');
            line = line.Replace('ﾋ', 'ひ');
            line = line.Replace('ﾌ', 'ふ');
            line = line.Replace('ﾍ', 'へ');
            line = line.Replace('ﾎ', 'ほ');
            //line = line.Replace('', 'ば');
            //line = line.Replace('', 'び');
            //line = line.Replace('', 'ぶ');
            //line = line.Replace('', 'べ');
            //line = line.Replace('', 'ぼ');
            //line = line.Replace('', 'ぱ');
            //line = line.Replace('', 'ぴ');
            //line = line.Replace('', 'ぷ');
            //line = line.Replace('', 'ぺ');
            //line = line.Replace('', 'ぽ');
            line = line.Replace('ﾏ', 'ま');
            line = line.Replace('ﾐ', 'み');
            line = line.Replace('ﾑ', 'む');
            line = line.Replace('ﾒ', 'め');
            line = line.Replace('ﾓ', 'も');
            line = line.Replace('ﾔ', 'や');
            line = line.Replace('ﾕ', 'ゆ');
            line = line.Replace('ﾖ', 'よ');
            line = line.Replace('ﾗ', 'ら');
            line = line.Replace('ﾘ', 'り');
            line = line.Replace('ﾙ', 'る');
            line = line.Replace('ﾚ', 'れ');
            line = line.Replace('ﾛ', 'ろ');
            line = line.Replace('ﾜ', 'わ');
            line = line.Replace('ｦ', 'を');
            line = line.Replace('ﾝ', 'ん');
            line = line.Replace('ｯ', 'っ');
            line = line.Replace('ｬ', 'ゃ');
            line = line.Replace('ｭ', 'ゅ');
            line = line.Replace('ｮ', 'ょ');
            line = line.Replace('ｧ', 'ぁ');
            line = line.Replace('ｨ', 'ぃ');
            line = line.Replace('ｩ', 'ぅ');
            line = line.Replace('ｪ', 'ぇ');
            line = line.Replace('ｫ', 'ぉ');

            //punctuation
            line = line.Replace('｡', '。');
            line = line.Replace('､', '、');
            line = line.Replace('･', '…');
            line = line.Replace('ﾟ', '？');
            line = line.Replace('｢', '「');
            line = line.Replace('｣', '」');
            line = line.Replace('�', '　');
            line = line.Replace('ｰ', 'ー');
            line = line.Replace('ﾞ', '！');
            line = line.Replace('〜', '～');

            return line;
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

        static String cleanOutBs(String line)
        {
            line = line.Replace("hiba", "hi&a");
            line = line.Replace("VT4Bb_", "vt4B&_");

            return line;
        }

        static String replaceKs(String line)
        {
            line = line.Replace("*", "k");

            return line;
        }

        static String replaceBs(String line)
        {
            line = line.Replace("&", "b");

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

            line = replaceBs(line);

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

        static void OutputFileHeader(System.IO.StreamWriter outFile, String arcName, int outFileNbr)
        {
            outFile.WriteLine("void main()");
            outFile.WriteLine("{");
            outFile.WriteLine("");
            outFile.WriteLine("");
            outFile.WriteLine("//*"+arcName+" Day "+outFileNbr);
            outFile.WriteLine("//＜＞＜＞＜＞＜＞＜＞＜＞＜＞＜＞");
            outFile.WriteLine("//＜＞＜＞＜＞＜＞＜＞＜＞＜＞＜＞");
            outFile.WriteLine("");
            outFile.WriteLine("	FadeOutBGM( 0, 1000, FALSE  );");
            outFile.WriteLine("	FadeOutBGM( 1, 1000, FALSE  );");
            outFile.WriteLine("	FadeOutBGM( 2, 1000, FALSE  );");
            outFile.WriteLine("	FadeOutBGM( 3, 1000, TRUE );");
            outFile.WriteLine("");
        }

        static void OutputFileFooter(System.IO.StreamWriter outFile)
        {
            outFile.WriteLine("	DisableWindow();");
            outFile.WriteLine("	SetValidityOfInput( FALSE );");
            outFile.WriteLine("	Wait( 3000 );");
            outFile.WriteLine("	DrawBustshotWithFiltering(6, \"cinema\", \"x\", 1, 0, 0, FALSE, 0, 0, 0, 0, 0, 25, 1300, TRUE );");
            outFile.WriteLine("	DrawBustshot(7, \"title02\", 0, 0, 0, FALSE, 0, 0, 0, 0, 0, 0, 0, 26, 3000, TRUE );");
            outFile.WriteLine("	Wait( 2000 );");
            outFile.WriteLine("	DrawBustshot(6, \"black\", 0, 0, 0, FALSE, 0, 0, 0, 0, 0, 0, 0, 25, 3000, TRUE );");
            outFile.WriteLine("	Wait( 1000 );");
            outFile.WriteLine("	FadeBustshotWithFiltering( 7, \"x\", 1, FALSE, 0, 0, 1000, TRUE );");
            outFile.WriteLine("	DrawScene( \"black\", 3000 );");
            outFile.WriteLine("	SetValidityOfInput( TRUE );");
            outFile.WriteLine("");
            outFile.WriteLine("}");
        }

        static String GetOutFilePath(String outFilePath, int outFileNbr)
        {
            return outFilePath + outFileNbr.ToString("D3") + ".txt";
        }

    }
}
