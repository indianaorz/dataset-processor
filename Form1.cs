using System;
using static System.Windows.Forms.LinkLabel;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace dataset_processor
{

    public partial class Form1 : Form
    {
        int m_index = 1;
        string m_filename = "";
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "C:\\Users\\Lee\\FFCO\\ai\\datasets\\bestiaryhk\\set\\";
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
            public string guid;
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
                m_metadata.Clear();//Getting Text files

                RenameFromFileName();

                m_filename = m_metadata[0].file_name;


                var output = "";
                foreach (var data in m_metadata)
                {
                    output += JsonConvert.SerializeObject(data) + "\n";
                };

                File.WriteAllText(textBox1.Text + "\\metadata.jsonl", output);

                //Save();
            }


            pnlImages.Controls.Clear();

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


            ////special append logic
            //if (File.Exists(textBox1.Text + "metadata.jsonl"))
            //{
            //    DirectoryInfo d = new DirectoryInfo(textBox1.Text); //Assuming Test is your Folder

            //    FileInfo[] Files = d.GetFiles("*.png"); //Getting Text files

            //    foreach (FileInfo file in Files)
            //    {

            //        var dataRecord = new DataRecord()
            //        {
            //            file_name = file.Name
            //        };

            //        if (file != null && file.Name.Contains('-'))
            //        {
            //            var name = "";
            //            var split = file.Name.Split('-');
            //            if(split.Length > 1)
            //            {
            //                var orig = m_metadata.Where(o => o.file_name == split[0] + ".png").First();
            //                var json = JsonConvert.SerializeObject(orig);
            //                dataRecord = JsonConvert.DeserializeObject<DataRecord>(json);
            //                if (split[1] == "1.png")
            //                {
            //                    dataRecord.view = "view_three_quarter";
            //                }
            //                if (split[1] == "2.png")
            //                {
            //                    dataRecord.view = "view_side";
            //                }
            //                if (split[1] == "3.png")
            //                {
            //                    dataRecord.view = "view_three_quarter_back";
            //                    dataRecord.eyes = "no_eyes";
            //                }
            //                name = name.Trim();
            //                dataRecord.file_name = file.Name;


            //                m_metadata.Insert(m_metadata.IndexOf(orig), dataRecord);
            //            }
            //        }
            //    }
            //    m_filename = m_metadata[0].file_name;


            //    var output = "";
            //    foreach (var data in m_metadata)
            //    {
            //        output += JsonConvert.SerializeObject(data) + "\n";
            //    };

            //    File.WriteAllText(textBox1.Text + "\\metadata.jsonl", output);

            //    //Save();
            //}
            LoadIndex(m_index);

        }

        private void RenameFromFileName()
        {
            DirectoryInfo d = new DirectoryInfo(textBox1.Text); //Assuming Test is your Folder

            var Files = d.GetFiles("*.png").ToList();

            bool metaDataExist = false;
            if (m_metadata.Count > 0)
            {
                metaDataExist = true;
            }
            foreach (FileInfo file in Files)
            {
                var dataRecord = new DataRecord()
                {
                    file_name = file.Name
                };

                if (metaDataExist)
                {
                    dataRecord = m_metadata[Files.IndexOf(file)];
                }

                if (file.Name.Contains('_')
                    || metaDataExist && string.IsNullOrEmpty(dataRecord.name))
                {
                    var name = "";
                    var split = file.Name.Split('_');
                    foreach (var s in split)
                    {
                        int.TryParse(s, out int value);
                        if (s != "b"
                            && value <= 0)
                        {
                            name += s + " ";
                            name = name.Replace(".png", "");
                        }
                    }
                    name = name.Trim();
                    dataRecord.name = name;
                }


                m_metadata.Add(dataRecord);
            }
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
            m_filename = current.file_name;
            //if (string.IsNullOrEmpty(current.type))
            //{
            //    return;
            //}
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
            tbPrimaryColor.Text = current.primary;
            tbAccent1.Text = current.accent;
            tbAccent2.Text = current.background;
            tbMisc.Text = current.colors;
            tbPose1.Text = current.pose;
            tbPose2.Text = current.pose2;
            tbView.Text = current.view;
            tbQuality.Text = current.quality;
            tbGuid.Text = current.guid;
            tbText.Text = current.text;
        }

        private void Save()
        {

            var newData = new DataRecord()
            {
                file_name = (!string.IsNullOrEmpty(m_filename)) ? m_filename : m_index.ToString().PadLeft(4, '0') + ".png",
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
                quality = tbQuality.Text.Trim().ToLower(),
                guid = tbGuid.Text.Trim().ToLower()
            };

            SetTextField(newData);

            m_metadata[m_index - 1] = newData;

            var output = "";
            foreach (var data in m_metadata)
            {
                output += JsonConvert.SerializeObject(data) + "\n";
            };

            File.WriteAllText(textBox1.Text + "\\metadata.jsonl", output);
        }

        private static void SetTextField(DataRecord newData)
        {
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
                + newData.quality
                + ((string.IsNullOrEmpty(newData.guid)) ? "" : " guid_" + newData.guid);
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

        private void button6_Click(object sender, EventArgs e)
        {
            var txt = "";

            foreach (var data in m_metadata)
            {
                txt += data.text + "\n";
            }
            var directory = tbOutput.Text + "\\val";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(directory + "\\val.txt", txt);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    button2_Click(sender, e);
                    break;
                case Keys.Up:
                    button3_Click(sender, e);
                    break;
                case Keys.PageDown:
                    AutoColor(m_metadata[m_index - 1]);
                    break;
            }
        }

        private void AutoColor(DataRecord data)
        {
            var myBitmap = new Bitmap(textBox1.Text + data.file_name);

            int red = 0;
            int orange = 0;
            int yellow = 0;
            int green = 0;
            int teal = 0;
            int aqua = 0;
            int blue = 0;
            int indigo = 0;
            int purple = 0;
            int violet = 0;
            int pink = 0;
            SortedDictionary<string, int> colorBalance = new SortedDictionary<string, int>();
            colorBalance.Add("red", 0);
            colorBalance.Add("orange", 0);
            colorBalance.Add("yellow", 0);
            colorBalance.Add("green", 0);
            colorBalance.Add("teal", 0);
            colorBalance.Add("aqua", 0);
            colorBalance.Add("blue", 0);
            colorBalance.Add("indigo", 0);
            colorBalance.Add("purple", 0);
            colorBalance.Add("violet", 0);
            colorBalance.Add("pink", 0);
            colorBalance.Add("white", 0);
            colorBalance.Add("black", 0);
            colorBalance.Add("tan", 0);
            colorBalance.Add("grey", 0);
            colorBalance.Add("brown", 0);
            for (int x = 0; x < myBitmap.Width; x++)
            {
                for (int y = 0; y < myBitmap.Height; y++)
                {
                    Color pixelColor = myBitmap.GetPixel(x, y);
                    if (pixelColor.GetBrightness() > .95f
                        && pixelColor.GetSaturation() < .1f)
                    {
                        continue;
                    }
                    if (pixelColor.GetBrightness() < .01f
                        && pixelColor.GetSaturation() < .01f)
                    {
                        colorBalance["black"]++;
                        continue;
                    }

                    var h = pixelColor.GetHue();
                    var s = pixelColor.GetSaturation();
                    var l = pixelColor.GetBrightness();
                    if (s < .3f && l > .65f)
                    {
                        colorBalance["grey"]++;
                    }
                    if (h >= 0
                        && h <= 25)
                    {
                        if (l < .2f)
                        {
                            colorBalance["brown"]++;
                        }
                        if (l > .4f
                            && s < .7f)
                        {
                            colorBalance["tan"]++;
                        }
                        else
                        {
                            //red
                            colorBalance["red"]++;
                        }
                    }
                    else if (h >= 25
                        && h <= 50)
                    {
                        if (l > .8f
                            && s < .3f)
                        {
                            colorBalance["tan"]++;
                        }
                        else
                        {
                            //orange
                            colorBalance["orange"]++;
                        }
                    }
                    else if (h >= 50
                        && h <= 90)
                    {
                        if (l > .9f
                            && s < .2f)
                        {
                            colorBalance["tan"]++;
                        }
                        else
                        {
                            //yellow
                            colorBalance["yellow"]++;
                        }
                    }
                    else if (h >= 90
                        && h <= 130)
                    {
                        //green
                        colorBalance["green"]++;
                    }
                    else if (h >= 130
                        && h <= 160)
                    {
                        //teal
                        colorBalance["teal"]++;
                    }
                    else if (h >= 160
                        && h <= 180)
                    {
                        //aqua
                        colorBalance["aqua"]++;
                    }
                    else if (h >= 180
                        && h <= 200)
                    {
                        //blue
                        colorBalance["blue"]++;
                    }
                    else if (h >= 200
                        && h <= 240)
                    {
                        //indigo
                        colorBalance["indigo"]++;
                    }
                    else if (h >= 260
                        && h <= 280)
                    {
                        //
                        //violet
                        if (s > .8f && l > .5f)
                        {
                            colorBalance["pink"]++;
                        }
                        else
                        {
                            colorBalance["purple"]++;
                        }
                    }
                    else if (h >= 280
                        && h <= 320)
                    {
                        //violet
                        if (s > .8f && l > .5f)
                        {
                            colorBalance["pink"]++;
                        }
                        else
                        {
                            colorBalance["violet"]++;
                        }
                    }
                    else if (h >= 320
                        && h <= 360)
                    {
                        if (l < .5)
                        {
                            colorBalance["brown"]++;
                        }
                        else
                        {
                            //pink
                            colorBalance["pink"]++;
                        }
                    }
                    // things we do with pixelColor
                }
            }

            var sortedDict = from entry in colorBalance orderby entry.Value descending select entry;
            var primary = sortedDict.ElementAt(0);
            var secondary = sortedDict.ElementAt(1);
            tbPrimaryColor.Text = primary.Key.ToString();
            tbAccent1.Text = secondary.Key.ToString();
            data.primary = primary.Key.ToString();
            data.accent = secondary.Key.ToString();

        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (cbClass.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.type = tbClass.Text;
                }
            }
            if (cbNameGender.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.name = tbName.Text;
                    data.gender = tbGender.Text;
                }
            }
            if (cbDesc.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.descriptor = tbAdj.Text;
                    data.form = tbOid.Text;
                    data.affinity = tbAffinity.Text;
                }
            }
            if (cbEyes.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.eyeColor = tbEyeColor.Text;
                    data.iris = tbIrisColor.Text;
                    data.pupils = tb_pupilColor.Text;
                }
            }
            if (cbEyeMod.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.eyes = tbEyeMod.Text;
                }
            }
            if (cbWearing.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.wearing = tbWearing.Text;
                }
            }
            if (cbColors.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.primary = tbPrimaryColor.Text;
                    data.accent = tbAccent1.Text;
                    data.background = tbAccent2.Text;
                }
            }
            if (cbColorMisc.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.colors = tbMisc.Text;
                }
            }
            if (cbPose.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.pose = tbPose1.Text;
                    data.pose2 = tbPose2.Text;
                }
            }
            if (cbView.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.view = tbView.Text;
                }
            }
            if (cbQuality.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.quality = tbQuality.Text;
                }
            }
            if (cbGuid.Checked)
            {
                foreach (var data in m_metadata)
                {
                    data.guid = tbGuid.Text;
                }
            }

            SaveAll();
        }

        private void SaveAll()
        {
            foreach (var data in m_metadata)
            {
                SetTextField(data);
            }

            var output = "";
            foreach (var data in m_metadata)
            {
                output += JsonConvert.SerializeObject(data) + "\n";
            };

            File.WriteAllText(textBox1.Text + "\\metadata.jsonl", output);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AutoColor(m_metadata[m_index - 1]);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            foreach (var data in m_metadata)
            {
                data.guid = Guid.NewGuid().ToString().Replace('-', '_');
                SetTextField(data);
            }

            var output = "";
            foreach (var data in m_metadata)
            {
                output += JsonConvert.SerializeObject(data) + "\n";
            };

            File.WriteAllText(textBox1.Text + "\\metadata.jsonl", output);
        }

        private void button9_Click(object sender, EventArgs e)
        {

            foreach (var data in m_metadata)
            {
                AutoColor(data);
                SetTextField(data);
            }
            var output = "";
            foreach (var data in m_metadata)
            {
                output += JsonConvert.SerializeObject(data) + "\n";
            };

            File.WriteAllText(textBox1.Text + "\\metadata.jsonl", output);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            RenameFromFileName();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            var metadatastring = System.IO.File.ReadAllText(textBox1.Text + "metadata.jsonl");


            var lines = metadatastring.Split("\n");

            int y = 0;
            foreach (var line in lines)
            {
                var record = JsonConvert.DeserializeObject<DataRecord>(line);
                if (record != null)
                {
                    m_metadata.Add(record);
                    var picBx = new PictureBox();
                    picBx.Image = Image.FromFile(textBox1.Text + record.file_name);


                    var width = (int)((picBx.Image.Width / 64) * 64) * 2;
                    var height = (int)((picBx.Image.Height / 64) * 64) * 2;

                    //width = Math.Min(width, 1600);
                    //height = Math.Min(height, 1600);

                    byte[] imageArray = System.IO.File.ReadAllBytes(textBox1.Text + record.file_name);
                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                    Img2ImgBody body = new Img2ImgBody()
                    {
                        init_images = new string[] { "data:image/png;base64," + base64ImageRepresentation },
                        width = width,
                        height = height,
                        cfg_scale = 8,
                        denoising_strength = .5f,
                        steps = 40,
                        sampler_index = "Euler a",
                        n_iter = 1,
                        prompt = "bg sotn highres painted background, sotn_style, full_body castlevania, death ,  skeletonoid , skull_face robed_reaper chest_ornament flowing_robe, primary_purple accent_white, kraid alien, legendary reptilian boss, third_eye, corpulent, green reptilian extraterrestrial gigantic spiked_head fangs nose_horn large_claws belly_holes, primary_green accent_yellow"
                    };

                    var content = JsonConvert.SerializeObject(body);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:7860/sdapi/v1/img2img");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(content);
                    }
                    try
                    {
                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            Response res = JsonConvert.DeserializeObject<Response>(result);
                            int i = 0;
                            foreach (var img in res.images)
                            {
                                string filePath = "output_" + i.ToString() + "_" + record.file_name;
                                File.WriteAllBytes(filePath, Convert.FromBase64String(img));
                                ++i;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        [Serializable]
        public class Response
        {
            public string[] images;
        }
        [Serializable]
        public class Img2ImgBody
        {
            public string[] init_images;
            public float denoising_strength;
            public string prompt;
            public int width;
            public int height;
            public float cfg_scale;
            public int steps;
            public string mask;
            public string[] styles;
            public string sampler_index;
            public int n_iter;
        }
        public string WebPostMethod(string postData, string URL)
        {
            string responseFromServer = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;
            ((HttpWebRequest)request).UserAgent =
                              "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 7.1; Trident/5.0)";
            request.Accept = "/";
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
    }
}