using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesseractOCR.Enums;
using TesseractOCR;
using Newtonsoft.Json;
using TesseractOCR.Layout;
using TesseractOCR.Renderers;
using System.ComponentModel.Design;

namespace GenshinAchievement
{
    class AchievementMaster
    {
        public List<AchievementItem> Items = new();
        public readonly List<string> WordsLog = new();
        
        static public readonly string JsonFilename = "AchievementMaster.json";
        [JsonIgnore]
        public string Text_achievement = "";
        [JsonIgnore]
        readonly Engine TesseractOCREngine;//全体

        public AchievementMaster() 
        {
            //OCR設定
            TesseractOCREngine = new Engine(@"./tessdata", Language.Japanese, EngineMode.Default);
            TesseractOCREngine.SetVariable("tessedit_char_whitelist", Text_achievement);
            TesseractOCREngine.DefaultPageSegMode = PageSegMode.SingleLine;
            //TesseractOCREngine.SetVariable("user_words_suffix", "user-words");
            //TesseractOCREngine.SetVariable("user_words", @"./tessdata");
        }
        public List<OCRResult> OCRMulti(Mat mat)
        {
            var grayMat = mat.CvtColor(ColorConversionCodes.BGR2GRAY).Threshold(110, 255, ThresholdTypes.Binary);
            grayMat.FindContours(out var contours, out var hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            List<(int Xmin, int Xmax, int Ymin, int Ymax)> ranges = new();
            int last_minX = 0, last_maxX = 0;
            int last_minY = 0, last_maxY = 0;

            //各行のMaxMin抽出
            foreach (var (minY, point) in contours.Select(a => (a.Min(b => b.Y), a)).OrderBy(a => a.Item1))
            {
                if (minY <= 5) continue;
                var maxY = point.Max(b => b.Y);
                if (maxY >= mat.Height - 5) continue;
                var minX = point.Min(b => b.X);
                var maxX = point.Max(b => b.X);
                if (minY >= last_maxY)
                {//レンジ確定
                    ranges.Add(new(last_minX, last_maxX, last_minY, last_maxY));
                    last_minY = minY;
                    last_maxY = maxY;
                    last_minX = minX;
                    last_maxX = maxX;
                    continue;
                }
                if (last_minY > minY) last_minY = minY;
                if (last_maxY < maxY) last_maxY = maxY;
                if (last_minX > minX) last_minX = minX;
                if (last_maxX < maxX) last_maxX = maxX;
            }
            ranges.Add(new(last_minX, last_maxX, last_minY, last_maxY));

            List<(string ? ImageFile,string Word)> oCRWords = new();
            foreach (var (Xmin, Xmax, Ymin, Ymax) in ranges)
            {
                if (Xmin < 3) continue;
                if (Xmax > mat.Width - 5) continue;
                if (Ymin < 3) continue;
                if (Ymax > mat.Height - 5) continue;

                if (Ymax <= 25) continue;
                if (Ymin >= mat.Height - 25) continue;

                var lineMat = new Mat(mat, new OpenCvSharp.Rect(Xmin - 3, Ymin - 3, Xmax - Xmin + 8, Ymax - Ymin + 8));
                using var TesseractImg = TesseractOCR.Pix.Image.LoadFromMemory(lineMat.ToBytes());
                using var page = TesseractOCREngine.Process(TesseractImg);
                //var word = page.Text.Replace(" ", "").TrimEnd();
                string word;
                if (page == null) word = "";
                else word = page.Text.TrimEnd();
                WordReplace(ref word);

                bool image_out_flag = false;
                //string? filename = null;
                int no_image;
                if (WordsLog.Contains(word))
                {
                    no_image = WordsLog.LastIndexOf(word);
                }
                else
                {
                    image_out_flag= true;
                    no_image = WordsLog.Count;
                    WordsLog.Add(word);
                }
                var filename = "./img/" + no_image + ".png";

                if (Ymin > 90 && Ymax < mat.Height - 90) image_out_flag = true;
                var y = Ymin - 3;
                var h = Ymax - Ymin + 8;
                if (Ymin > 90)
                {
                    y -= 87;
                    h += 86;
                }
                if (Ymax < mat.Height - 90)
                {
                    h += 86;
                }
                if (image_out_flag)
                {
                    var ImageMat = new Mat(mat, new OpenCvSharp.Rect(0, y, 300, h));
                    ImageMat.SaveImage(filename);
                }
                oCRWords.Add(new(filename, word));
                //ocrResults.Add(new OCRResult(filename, word));

            }
            List<OCRResult> ocrResults = new();
            for (var i = 1; i < oCRWords.Count - 1; ++i)
            {
                var file = oCRWords[i].ImageFile;
                if (file == null) continue;
                ocrResults.Add(new OCRResult(file, oCRWords[i].Word, oCRWords[i - 1].Word, oCRWords[i + 1].Word));
            }
            return ocrResults;
        }

        public static double Compare_MatchRatio(string long_word, string short_word)
        {
            var count = 0;
            var index = 0;
            foreach (var b in short_word)
            {
                var index2 = long_word.IndexOf(b, index);
                if (index2 != -1)
                {
                    count++;
                    index = index2 + 1;
                }
            }
            return (double)count / Math.Max(short_word.Length, long_word.Length);

        }
        public List<AchievementItem> CheckAchievement(OCRResult oCRResult,out string? errtext)
        {
            errtext = null;
            List<AchievementItem> hits = new();
            foreach (var a in Items
                .Where(a => oCRResult.Word.Length - 1 <= a.Word.Length && a.Word.Length <= oCRResult.Word.Length + 2))
            {
                var matchRatio = Compare_MatchRatio(a.Word, oCRResult.Word);
                if (matchRatio >= 0.5)
                {
                    hits.Add(a);
                    if (a.HitOCRs.Count == 0 || matchRatio > a.HitOCRs.Max(a => a.MatchRatio))
                        a.HitOCRs.Add(new HitOCRResult(oCRResult, matchRatio));
                    a.Filename = oCRResult.ImageFile;

                    if (matchRatio != 1)
                    {
                        bool next = false;
                        bool prev = false;

                        if (a.Next != null && Compare_MatchRatio(a.Next.Word, oCRResult.NextWord) >= 0.75)
                            next = a.Next.Threshold();
                        if (a.Prev != null && Compare_MatchRatio(a.Prev.Word, oCRResult.PrevWord) >= 0.75)
                            prev = a.Prev.Threshold();

                        if (next && prev)
                        {
                            if (1 > a.HitOCRs.Max(a => a.MatchRatio))
                                a.HitOCRs.Add(new HitOCRResult(new OCRResult(oCRResult.ImageFile, "上下一致の間"), 1));
                        }
                        else if (next || prev)
                        {
                            matchRatio = (matchRatio + 1) / 2;
                            if (matchRatio > a.HitOCRs.Max(a => a.MatchRatio))
                                a.HitOCRs.Add(new HitOCRResult(new OCRResult(oCRResult.ImageFile, "隣接一致補正"), matchRatio));
                        }
                    }

                    a.SetListViewItem();
                }
            }

            if (hits.Count == 0) 
            {
            }
            else if (hits.Count == 1 || hits.Count(a => a.HitOCRs.Last().MatchRatio >= 0.8) == 1)
            {
            }
            else
            {
                errtext = "複数一致あり" + ":" + hits.Last().HitOCRs.Last().OCRResult.Word + string.Join('/', hits.Select(a => a.Word + ":" + a.HitOCRs.Last().MatchRatio.ToString("f2")));
            }
            return hits;
        }

        public void SaveJson()
        {
            var setting = new JsonSerializerSettings();
            setting.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            setting.Formatting = Formatting.Indented;
            File.WriteAllText(JsonFilename, JsonConvert.SerializeObject(this, setting));
        }
        public void LoadJson()
        {
            if (File.Exists(JsonFilename) == false) return;
            var tmp = JsonConvert.DeserializeObject<AchievementMaster>(File.ReadAllText(JsonFilename));
            if (tmp != null)
            {
                WordsLog.Clear();
                WordsLog.AddRange(tmp.WordsLog);
                foreach (var item in tmp.Items)
                {
                    var target = Items.FirstOrDefault(a => a.Word == item.Word);
                    if (target != null)
                    {
                        target.HitOCRs.AddRange(item.HitOCRs);
                        target.Filename = item.Filename;
                    }
                }
            }

        }
        public void Init()
        {
            Items.Clear();
            var text = LoadCSV("list.csv", true);//連続データ
            //text += LoadCSV("list2.csv", false);//追加非連続

            StringBuilder sb = new();
            foreach (var c in text.OrderBy(a => a).Distinct())
            {
                sb.Append(c);
            }
            Text_achievement = sb.ToString();
        }
        public string LoadCSV(string filename, bool series)
        {
            var text = File.ReadAllText(filename);
            WordReplace(ref text);

            var tmp = text.Split("\n").Select(a => new AchievementItem(a)).ToList();
            Items.AddRange(tmp);
            if (series)
            {
                tmp[0].Next = tmp[1];
                for (int i = 1; i < tmp.Count - 1; ++i)
                {
                    tmp[i].Next = tmp[i + 1];
                    tmp[i].Prev = tmp[i - 1];
                }
                tmp[^1].Prev = tmp[^2];
            }

            return text;
        }
        public void WordReplace(ref string text)
        {
            text = text.Replace("\r", "\n")
                .Replace("\n\n", "\n")
                .Replace('!', '！')
                .Replace('?', '？')
                .Replace("...", "…")
                .Replace("･･･", "…")
                .Replace("·", "・")
                .Replace('一', 'ー')
                //.Replace(" ", "")
                ;
        }
    }
    class OCRResult
    {
        public readonly string ImageFile;
        public readonly string Word;
        public readonly string PrevWord;
        public readonly string NextWord;

        public OCRResult(string ImageFile, string Word, string PrevWord = "", string NextWord = "")
        {
            this.ImageFile = ImageFile;
            this.Word = Word;
            this.PrevWord = PrevWord;
            this.NextWord = NextWord;
        }
    }
    class HitOCRResult
    {
        public readonly OCRResult OCRResult;
        public readonly double MatchRatio;
        public HitOCRResult(OCRResult OCRResult, double MatchRatio)
        {
            this.OCRResult = OCRResult;
            this.MatchRatio = MatchRatio;
        }
    }
    class AchievementItem
    {
        public readonly string Word;
        public readonly List<HitOCRResult> HitOCRs = new();
        public string Filename = "";

        [JsonIgnore]
        public AchievementItem? Prev;
        [JsonIgnore]
        public AchievementItem? Next;
        [JsonIgnore]
        public ListViewItem _ListViewItem = new();
        public AchievementItem(string Word)
        {
            this.Word = Word;
        }
        public bool Threshold()
        {
            if (HitOCRs.Count == 0) return false;
            var match = HitOCRs.Last().MatchRatio;
            if (match >= 0.85)
            {//完全一致//7文字以上 1文字違い
                return true;
            }
            //if (wordA.Length == wordOCR.Length && wordOCR.Length * match >= wordOCR.Length - 1)
            //{//文字長一致 1文字違い
            //    return true;
            //}
            return false;
        }
        public void SetListViewItem()
        {
            if (HitOCRs.Count == 0) return;
            var match = HitOCRs.Last().MatchRatio;
            var wordOCR = HitOCRs.Last().OCRResult.Word;

            if (Threshold())
            {
                //if (!_ListViewItem.Checked) _ListViewItem.EnsureVisible();
                _ListViewItem.Checked = true;
            }

            if (match == 1)
            {//完全一致
                _ListViewItem.ForeColor = Color.Black;
            }
            else if (match >= 0.85)
            {//7文字以上 1文字違い
                _ListViewItem.ForeColor = Color.DarkGreen;
            }
            else if (Word.Length == wordOCR.Length && wordOCR.Length * match >= wordOCR.Length - 1)
            {//文字長一致 1文字違い
                _ListViewItem.ForeColor = Color.Green;
            }
            else if (HitOCRs.Count >= 2)
            {//複数一致
                _ListViewItem.ForeColor = Color.Red;
            }
            else
            {
                _ListViewItem.ForeColor = Color.DarkKhaki;
            }
        }
    }
}
