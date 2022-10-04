using System;
using static System.Windows.Forms.LinkLabel;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace dataset_processor
{

    public partial class Form1 : Form
    {
        int m_index = 1;
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "C:\\Users\\Lee\\FFCO\\ai\\datasets\\chao\\set\\";
        }

        public class DataRecord
        {
            public string file_name;
            public string type;
            public string name;
            public string gender;
            public string descriptor;
            public string form;
            public string affinity;
            public string eyes;
            public string eyeColor;
            public string iris;
            public string pupils;
            public string wearing;
            public string colors;
            public string primary;
            public string accent;
            public string background;
            public string pose;
            public string pose2;
            public string view;
            public string quality;
            public string text;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Size = new Size(512, 512);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }

        List<DataRecord> m_metadata = new List<DataRecord>();
        List<PictureBox> m_picBoxes = new List<PictureBox>();
        private void button1_Click(object sender, EventArgs e)
        {
            m_metadata.Clear();
            m_picBoxes.Clear();

            if (textBox1.Text.Last() != '\\')
            {
                textBox1.Text += "\\";
            }

            if (!File.Exists(textBox1.Text + "metadata.jsonl"))
            {
                m_metadata.Clear();
                DirectoryInfo d = new DirectoryInfo(textBox1.Text); //Assuming Test is your Folder

                FileInfo[] Files = d.GetFiles("*.png"); //Getting Text files

                foreach (FileInfo file in Files)
                {
                    var dataRecord = new DataRecord()
                    {
                        file_name = file.Name
                    };
                    m_metadata.Add(dataRecord);
                }
                Save();
            }

            var metadatastring = System.IO.File.ReadAllText(textBox1.Text + "metadata.jsonl");


            var lines = metadatastring.Split("\n");

            int y = 0;
            int x = 0;
            int maxX = 7;
            foreach (var line in lines)
            {
                var record = JsonConvert.DeserializeObject<DataRecord>(line);
                if (record != null)
                {
                    m_metadata.Add(record);
                    var picBx = new PictureBox();
                    picBx.Image = Image.FromFile(textBox1.Text + record.file_name);
                    pnlImages.Controls.Add(picBx);
                    picBx.Location = new Point(128 * x, 128 * (y));
                    picBx.Size = new Size(128, 128);
                    picBx.SizeMode = PictureBoxSizeMode.StretchImage;
                    picBx.BringToFront();
                    picBx.Click += PicBx_Click;
                    picBx.Name = m_metadata.Count.ToString();
                    ++x;
                    if (x > maxX)
                    {
                        x = 0;
                        ++y;
                    }
                }
            }
            LoadIndex(m_index);

        }

        private void PicBx_Click(object? sender, EventArgs e)
        {
            m_index = int.Parse(((PictureBox)sender).Name);
            LoadIndex(m_index);
        }

        private void LoadIndex(int i, bool t_forceNoLoad = false)
        {
            var current = m_metadata[m_index - 1];

            label1.Image = Image.FromFile(textBox1.Text + current.file_name);
            if (string.IsNullOrEmpty(current.type))
            {
                return;
            }
            if (t_forceNoLoad)
            {
                return;
            }
            tbClass.Text = current.type;
            tbName.Text = current.name;
            tbGender.Text = current.gender;
            tbAdj.Text = current.descriptor;
            tbOid.Text = current.form;
            tbAffinity.Text = current.affinity;
            tbEyeMod.Text = current.eyes;
            tbEyeColor.Text = current.eyeColor;
            tbIrisColor.Text = current.iris;
            tb_pupilColor.Text = current.pupils;
            tbWearing.Text = current.wearing;
            tbMisc.Text = current.colors;
            tbPrimaryColor.Text = current.primary;
            tbAccent1.Text = current.accent;
            tbAccent2.Text = current.background;
            tbMisc.Text = current.colors;
            tbPose1.Text = current.pose;
            tbPose2.Text = current.pose2;
            tbView.Text = current.view;
            tbQuality.Text = current.quality;
        }

        private void Save()
        {

            var newData = new DataRecord()
            {
                file_name = m_index.ToString().PadLeft(4, '0') + ".png",
                type = tbClass.Text.Trim().ToLower(),
                name = tbName.Text.Trim().ToLower(),
                gender = tbGender.Text.Trim().ToLower(),
                descriptor = tbAdj.Text.Trim().ToLower(),
                form = tbOid.Text.Trim().ToLower(),
                affinity = tbAffinity.Text.Trim().ToLower(),
                eyes = tbEyeMod.Text.Trim().ToLower(),
                eyeColor = tbEyeColor.Text.Trim().ToLower(),
                iris = tbIrisColor.Text.Trim().ToLower(),
                pupils = tb_pupilColor.Text.Trim().ToLower(),
                wearing = tbWearing.Text.Trim().ToLower(),
                colors = tbMisc.Text.Trim().ToLower(),
                primary = tbPrimaryColor.Text.Trim().ToLower(),
                accent = tbAccent1.Text.Trim().ToLower(),
                background = tbAccent2.Text.Trim().ToLower(),
                pose = tbPose1.Text.Trim().ToLower(),
                pose2 = tbPose2.Text.Trim().ToLower(),
                view = tbView.Text.Trim().ToLower(),
                quality = tbQuality.Text.Trim().ToLower()
            };

            var poseText = "";
            if (!string.IsNullOrEmpty(newData.pose)) { poseText += "pose_" + newData.pose; }
            if (!string.IsNullOrEmpty(newData.pose2)) { poseText += " pose_" + newData.pose2; }


            var colorText = "";
            if (!string.IsNullOrEmpty(newData.primary)) { colorText += "primary_" + newData.primary; }
            if (!string.IsNullOrEmpty(newData.accent)) { colorText += " accent_" + newData.accent; }
            if (!string.IsNullOrEmpty(newData.background)) { colorText += " background_" + newData.background; }

            var eyeColorText = "";
            if (!string.IsNullOrEmpty(newData.eyeColor)) { eyeColorText += "eyes_" + newData.eyeColor; }
            if (!string.IsNullOrEmpty(newData.iris)) { eyeColorText += " iris_" + newData.iris; }
            if (!string.IsNullOrEmpty(newData.pupils)) { eyeColorText += " pupils_" + newData.pupils; }


            newData.text = newData.type + ", "
                + newData.name + " "
                + newData.gender + ", "
                + newData.descriptor + " "
                + newData.form + " "
                + newData.affinity + ", "
                + eyeColorText + ((string.IsNullOrEmpty(newData.eyes)) ? "" : " ") + newData.eyes + ", "
                + newData.wearing + ", "
                + colorText + ", "
                + poseText + ", "
                + newData.view + ", "
                + newData.quality;

            m_metadata[m_index - 1] = newData;

            var output = "";
            foreach (var data in m_metadata)
            {
                output += JsonConvert.SerializeObject(data) + "\n";
            };

            File.WriteAllText(textBox1.Text + "\\metadata.jsonl", output);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Save();
            m_index += 1;
            if (m_index > m_metadata.Count)
            {
                m_index = 1;
            }
            if (m_index < 1)
            {
                m_index = m_metadata.Count;
            }
            LoadIndex(m_index);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Save();
            m_index -= 1;
            if (m_index >= m_metadata.Count)
            {
                m_index = 1;
            }
            if (m_index < 1)
            {
                m_index = m_metadata.Count;
            }
            LoadIndex(m_index);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var directory = tbOutput.Text + "\\dataset";
            var imgDirectory = directory + "\\img";
            var txtDirectory = directory + "\\txt";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!Directory.Exists(imgDirectory))
            {
                Directory.CreateDirectory(imgDirectory);
            }
            if (!Directory.Exists(txtDirectory))
            {
                Directory.CreateDirectory(txtDirectory);
            }
            foreach (var data in m_metadata)
            {
                var img = Image.FromFile(textBox1.Text + data.file_name);
                img.Save(imgDirectory + "\\" + data.file_name);
                File.WriteAllText(txtDirectory + "\\" + data.file_name.Replace(".png", ".txt"), data.text);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tbEyeMod.Text = "";
            Save();
            m_index += 1;
            if (m_index >= m_metadata.Count)
            {
                m_index = 1;
            }
            if (m_index < 1)
            {
                m_index = m_metadata.Count;
            }
            LoadIndex(m_index);
        }

        private void btnApplyAll_Click(object sender, EventArgs e)
        {
            Save();
            m_index += 1;
            if (m_index >= m_metadata.Count)
            {
                m_index = 1;
            }
            if (m_index < 1)
            {
                m_index = m_metadata.Count;
            }
            LoadIndex(m_index, true);
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}