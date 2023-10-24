using TesseractOCR.Enums;
using TesseractOCR;
using OpenCvSharp;
using System.Text;

namespace GenshinAchievement
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly AchievementMaster AchievementMaster = new();

        private void timer1_Tick(object sender, EventArgs e)
        {
            var bmp = WindowCapture.WindowCapture.Capture("å¥ê_", 580, 120, 460, 560);
            if (bmp == null) return;
            //bmp.Save("aaaa.bmp");

            pictureBox1.Image = bmp;

            var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);

            StringBuilder text = new();

            List<AchievementItem> hits = new();
            var ocrs = AchievementMaster.OCRMulti(mat);
            foreach (var ocr in ocrs)
            {
                var tmp = AchievementMaster.CheckAchievement(ocr, out var errtext);
                //if (tmp.Any()) hits.Add(tmp.OrderByDescending(a => a.HitOCRs.Last().MatchRatio).First());
                if (tmp.Count == 1) hits.Add(tmp[0]);
                if (errtext != null)
                    textBox1.AppendText(errtext + "\r\n");
                if (textBox1.Text.Length > 1000) textBox1.Clear();
                text.AppendLine(ocr.Word);
            }
            foreach (var ocr in ocrs.GetRange(0, ocrs.Count - 1).AsEnumerable().Reverse())
            {
                AchievementMaster.CheckAchievement(ocr, out _);
            }
            if (text.Length > 0)
                textBox2.Text = text.ToString();

            if (hits.Any())
            {
                if (checkBox2.Checked)
                {
                    listView1.SelectedItems.Clear();
                    foreach (var hit in hits) hit._ListViewItem.Selected = true;
                }
                var v = hits.OrderByDescending(a => a.HitOCRs.Last().MatchRatio).First()._ListViewItem;
                v.EnsureVisible();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Enabled = true;
                //listView1.Enabled = false;
                listView1.MultiSelect = true;
                timer1.Interval = 100;
                timer1.Start();
            }
            else
            {
                checkBox2.Enabled = false;
                //listView1.Enabled = true;
                //listView1.MultiSelect = false;
                timer1.Stop();
                AchievementMaster.SaveJson();
            }

        }
        private void Init_listView1()
        {
            listView1.Items.Clear();
            foreach (var achievement in AchievementMaster.Items)
            {
                var item = new ListViewItem
                {
                    Text = achievement.Word,
                    ForeColor = Color.Gray,
                };
                listView1.Items.Add(item);

                item.Tag = achievement;
                achievement._ListViewItem = item;
                if (achievement.HitOCRs.Count != 0)
                {
                    achievement.SetListViewItem();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            AchievementMaster.Init();
            AchievementMaster.LoadJson();
            Init_listView1();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            AchievementMaster.Init();
            Init_listView1();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox2.Image = null;
            if (listView1.SelectedItems.Count == 0) return;
            var a = (AchievementItem)listView1.SelectedItems[0].Tag;
            var filename = a.Filename;
            if (File.Exists(filename))
                pictureBox2.Image = new Bitmap(filename);
            textBox3.Text = "";
            if (a.HitOCRs.Count != 0)
            {
                foreach (var h in a.HitOCRs.AsEnumerable().Reverse())
                {
                    textBox3.AppendText("[" + h.MatchRatio.ToString("f2") + "]" + h.OCRResult.Word + "\r\n" + h.OCRResult.PrevWord + "/" + h.OCRResult.NextWord + "\r\n");
                    //textBox3.AppendText("[" + h.MatchRatio.ToString("f2") + "]" + h._OCRResult.Word + "\r\n");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var a = (AchievementItem)listView1.SelectedItems[0].Tag;
            a.HitOCRs.Add(new HitOCRResult(new OCRResult("", "user"), 1));
            a.SetListViewItem();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var bmp = new Bitmap("aaaa.bmp");
            if (bmp == null) return;

            pictureBox1.Image = bmp;

            var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);

            StringBuilder text = new();

            foreach (var resultOCR in AchievementMaster.OCRMulti(mat))
            {
                var errtext = AchievementMaster.CheckAchievement(resultOCR, out _);
                if (errtext != null)
                    textBox1.AppendText(errtext + "\r\n");
                text.AppendLine(resultOCR.Word);
            }
            if (text.Length > 0)
                textBox2.Text = text.ToString();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText(listView1.SelectedItems[0].Text);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AchievementMaster.SaveJson();
            StringBuilder s = new();
            foreach(var item in AchievementMaster.Items)
            {
                s.AppendLine(
                    (item.Threshold() ? "1" : "0") + ","
                    + item.Word.ToString() + ","
                    + (item.HitOCRs.Any() ? string.Join("|", item.HitOCRs.Select(a => a.OCRResult.Word)) : "") + ","
                    + (item.HitOCRs.Any() ? item.HitOCRs.Last().MatchRatio.ToString("f2") : "0")
                    );
            }
            File.WriteAllText("out.csv", s.ToString());
        }

    }
}