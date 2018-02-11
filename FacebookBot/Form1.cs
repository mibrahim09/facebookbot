using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromoBooster
{
    public partial class Form1 : Form
    {
        WebBrowser groupsGeter = new WebBrowser();
        WebBrowser Poster = new WebBrowser();
        WebBrowser groupsJoinerGet = new WebBrowser();
        WebBrowser groupsJoiner = new WebBrowser();
        DateTime startedOn, AppStartTime;
        string p = Environment.CurrentDirectory + "\\Posts";
        public Form1()
        {
            AppStartTime = DateTime.Now;

            InitializeComponent();
            myBrowser.Navigate("http://m.facebook.com");
            startedOn = DateTime.Now;
            try
            {
                using (var reader = new StreamReader("pictures.txt"))
                {
                    foreach (string line in reader.ReadToEnd().Split('\n'))
                        picturesList.Items.Add(line);
                }
                if (Directory.Exists(p))
                {
                    string[] files = Directory.GetFiles(p);
                    for (int i = 0; i < files.Length; i++)
                        using (var reader = new StreamReader(files[i]))
                        {
                            string text = reader.ReadToEnd();
                            textLists.Items.Add(text);
                        }
                }
            }
            catch
            {

            }
        }

        void groupsJoinerGet_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != groupsJoinerGet.Url) return;
            try
            {
                if (listBox2.Items.Count >= int.Parse(lim.Text))
                {
                    WriteLine("Groups search is done!");
                    currnav = loop + 1;// to end all
                    return;
                }
                HtmlElementCollection elementsByTagName = groupsJoinerGet.Document.GetElementsByTagName("a");
                var sequenceEnum = elementsByTagName.GetEnumerator();
                while (sequenceEnum.MoveNext())
                {
                    HtmlElement current = (HtmlElement)sequenceEnum.Current;
                    if (!current.GetAttribute("href").Contains("/groups/")) continue;
                    if (current.GetAttribute("href").Contains("/search/")) continue;
                    if (current.GetAttribute("href").Contains("/create/")) continue;
                    if (current.GetAttribute("href").Contains("category")) continue;
                    if (current.GetAttribute("href").Contains("?see")) continue;
                    string item = current.GetAttribute("href").ToString();
                    uint members = 0;
                    Uri ur = new Uri(item);
                    foreach (HtmlElement epp in groupsJoinerGet.Document.All)
                    {
                        if (epp.InnerText == null) continue;
                        if (epp.InnerText.Contains(current.InnerText))
                        {
                            string[] p = epp.InnerText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            if (p.Length == 2)
                            {
                                try
                                {
                                    members = uint.Parse(p[1].Replace(",", "").Replace("Members", ""));
                                    break;
                                }
                                catch
                                {
                                    MessageBox.Show("Error : " + p[0] + " __________ " + p[1]);
                                    continue;
                                }
                            }
                        }
                    }
                    AddNwGp(current.InnerText, ur.AbsolutePath.Replace("/groups/", "").Replace("/", ""), members);
                }

                if (currnav <= loop)
                {
                    currnav++;
                    beg += 5;
                    if (keyword.Text != "")
                        this.groupsJoinerGet.Navigate("https://m.facebook.com/search/?query=" + keyword.Text + "&search=group&pn=" + currnav + "&ssid=0&s=" + beg);//+ "&tsid&o=69&refid=46");
                    else
                    {
                        MessageBox.Show("Invalid keyword");
                        currnav = loop + 1;// to end all
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }


        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
        bool st = false;
        private void button1_Click(object sender, EventArgs e)
        {
            groupsGeter.Navigate("https://m.facebook.com/groups/?seemore");
        }

        void groupsGeter_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != groupsGeter.Url) return;
            listBox1.Items.Clear();
            HtmlElementCollection elementsByTagName = groupsGeter.Document.GetElementsByTagName("a");
            var sequenceEnum = elementsByTagName.GetEnumerator();
            while (sequenceEnum.MoveNext())
            {
                HtmlElement current = (HtmlElement)sequenceEnum.Current;
                if (!current.GetAttribute("href").Contains("/groups/")) continue;
                if (current.GetAttribute("href").Contains("/create/")) continue;
                if (current.GetAttribute("href").Contains("category")) continue;
                if (current.GetAttribute("href").Contains("?see")) continue;
                string item = current.GetAttribute("href").ToString();
                if (listBox1.Items.Contains(item)) continue;
                listBox1.Items.Add(item);
            }
            totalGPS.Text = listBox1.Items.Count.ToString();

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            select.Text = (listBox1.SelectedIndex + 1).ToString();
        }

        private void toStartWith_TextChanged(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            try
            {
                listBox1.SelectedIndex = int.Parse(toStartWith.Text) - 1;
            }
            catch
            {
                toStartWith.Text = "1";
                listBox1.SelectedIndex = int.Parse(toStartWith.Text) - 1;
            }
        }
        void WriteLine(string l)
        {
            logger.AppendText(l + Environment.NewLine);
            logger.ScrollToCaret();
        }
        int timespan = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            startPost();
        }
        int finished = 0, truefinished = 0;
        async Task wait(int T)
        {
            await Task.Delay(T);
        }
        string BlaBla = "abcdefghijklmnopqrstuvwxyz123456789!@";
        Random rnd = new Random();
        public string GetRandomText
        {
            get
            {
                string toReturn = "";
                char[] P = BlaBla.ToArray();
                for (int i = 0; i < 10; i++)
                    toReturn += P[rnd.Next(0, P.Length)];
                return toReturn;
            }
        }
        List<string> Posted = new List<string>();
        async void myBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Poster.Url) return;
            if (Posted.Contains(Poster.Url.AbsoluteUri)) return;
            else Posted.Add(Poster.Url.AbsoluteUri);
            if (allowPost)
            {
                try
                {
                    string img = "", TextToPost = "";
                    if (picturesList.Items.Count != 0)
                        img = picturesList.Items[rnd.Next(0, picturesList.Items.Count)].ToString();
                    if (textLists.Items.Count != 0)
                        TextToPost = textLists.Items[rnd.Next(0, textLists.Items.Count)].ToString();
                    if (Poster.Url.AbsoluteUri.Contains("m.facebook.com/groups") && !Poster.Url.AbsoluteUri.Contains("rdr"))
                    {
                        if (Poster.Document.GetElementById("xc_message") != null)
                        {
                            Poster.Document.GetElementById("xc_message").SetAttribute("value", img +
                                "\n" +
                                TextToPost + "\n" +
                                GetRandomText);
                            Poster.Document.GetElementById("view_post").InvokeMember("click");
                        }
                        int towait = rnd.Next(int.Parse(timeToWait.Text), int.Parse(upperLimit.Text)) * 1000;
                        WriteLine("Posted on " + Poster.Url.AbsolutePath.Replace("/groups/", ""));
                        int p = towait;
                        int mins = 0;
                        while (towait >= 60000)
                        {
                            mins++;
                            towait -= 60000;
                        }
                        WriteLine("Waiting for " + mins + " mins and " + (towait / 1000) + " seconds before next post!");
                        await wait(p);
                        WriteLine("Moving to next. Waiting for " + (towait / 1000).ToString());
                        await wait(p);
                        finished++;
                        truefinished++;
                        timer1.Start();
                    }
                }
                catch (Exception ex)
                {
                    WriteLine(ex.ToString());
                    endPost();
                    startPost();
                }
                //catch
                //{
                //    endPost();
                //    startPost();
                //}
            }
        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex == (listBox1.Items.Count - 1))
                {
                    endPost();
                    WriteLine("Task done !");
                }
                timer1.Stop();
                if (finished == 50)
                {
                    int toWait = rnd.Next(10, 20);
                    WriteLine("RESTING FOR " + toWait + " MINUTES TO PREVENT BLOCK!");
                    await wait(toWait * 60 * 1000);// Breaking for 10 mins
                    finished = 0;
                }
                timer2.Start();
                listBox1.SelectedIndex++;
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            Poster.Navigate(listBox1.SelectedItem.ToString());
            postedOn.Text = listBox1.SelectedItem.ToString();
        }
        bool allowPost = false;
        private void button3_Click(object sender, EventArgs e)
        {
            endPost();
            WriteLine("Stopped!");
        }
        public void endPost()
        {
            timer1.Stop();
            timer2.Stop();
            checkBox1.Enabled = true;
            sch.Enabled = true;
            allowPost = false;
            stopbtn.Enabled = false;
            startbtn.Enabled = true;
            gpsBtn.Enabled = true;
        }
        bool added = false;
        public void startPost()
        {
            Posted.Clear();
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Please get the groups first.");
                return;
            }
            if (textLists.Items.Count == 0)
            {
                MessageBox.Show("Please write something to post.");
                return;
            }
            if (!st)
            {
                if (!added)
                {
                    Poster.DocumentCompleted += myBrowser_DocumentCompleted;
                    added = true;
                }
                st = true;
            }
            checkBox1.Enabled = false;
            sch.Enabled = false;
            allowPost = true;
            stopbtn.Enabled = true;
            startbtn.Enabled = false;
            gpsBtn.Enabled = false;

            eta.Text = ((listBox1.Items.Count) * ((int.Parse(timeToWait.Text.ToString()) + int.Parse(upperLimit.Text.ToString())) / 2)) + " seconds";
            finished = 0;
            WriteLine("Started posting");
            timer1.Start();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            sch.Enabled = checkBox1.Checked;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                if (DateTime.Now > startedOn.AddMinutes(int.Parse(rep.Text)))
                {
                    startedOn = DateTime.Now;
                    endPost();
                    startPost();
                }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
            {
                save(richTextBox1.Text);
                textLists.Items.Add(richTextBox1.Text);
                richTextBox1.Clear();
                WriteLine("Added a post to the posts-list. Total : " + textLists.Items.Count);
            }
            else
                MessageBox.Show("You can't post an empty topic.");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (pics.Text != "")
            {
                picturesList.Items.Add(pics.Text);
                pics.Clear();
                WriteLine("Added an img to the imgs-list. Total : " + picturesList.Items.Count);
            }
            else
                MessageBox.Show("Please add the picture URL");
            if (File.Exists(@"pictures.txt"))
                File.Delete(@"pictures.txt");
            using (var stream = new StreamWriter(@"pictures.txt"))
            {
                foreach (string item in picturesList.Items)
                    stream.WriteLine(item);
                stream.Flush();
                stream.Close();
            }
        }

        private void startbtn_Click(object sender, EventArgs e)
        {
            startPost();
        }

        private void gpsBtn_Click(object sender, EventArgs e)
        {
            groupsGeter.Navigate("https://m.facebook.com/groups/?seemore");
            groupsGeter.DocumentCompleted += groupsGeter_DocumentCompleted;
        }

        private void logger_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            select.Text = (listBox1.SelectedIndex + 1).ToString();
        }

        private void toStartWith_TextChanged_1(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            try
            {
                listBox1.SelectedIndex = int.Parse(toStartWith.Text) - 1;
            }
            catch
            {
                toStartWith.Text = "1";
                listBox1.SelectedIndex = int.Parse(toStartWith.Text) - 1;
            }
        }

        private void stopbtn_Click(object sender, EventArgs e)
        {
            endPost();
            WriteLine("Stopped!");
            WriteLine("Posted in total : " + truefinished + " groups!");
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            sch.Enabled = checkBox1.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }
        void save(string text)
        {
            using (var stream = new StreamWriter(@"pictures.txt"))
            {
                foreach (string item in picturesList.Items)
                    stream.WriteLine(item);
                stream.Flush();
                stream.Close();
            }

            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);
            string path = DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond + "-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            using (var stream = new StreamWriter(p + @"\" + path + ".txt"))
            {
                foreach (var st in text.Split(new string[] { "\n" }, StringSplitOptions.None))
                    stream.WriteLine(st);
                stream.Flush();
                stream.Close();
            }
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            picturesList.Items.Clear();
            textLists.Items.Clear();
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            if (picturesList.SelectedIndex == -1) MessageBox.Show("Select an image first.");
            else picturesList.Items.RemoveAt(picturesList.SelectedIndex);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (textLists.SelectedIndex == -1) MessageBox.Show("Select a post first.");
            else textLists.Items.RemoveAt(textLists.SelectedIndex);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) MessageBox.Show("Select a group first.");
            else listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            totalGPS.Text = listBox1.Items.Count.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            picturesList.Items.Clear();
            if (File.Exists(@"pictures.txt"))
                File.Delete(@"pictures.txt");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textLists.Items.Clear();
            if (Directory.Exists(p))
                Directory.Delete(p, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            comboBox1.SelectedIndex = 0;
            groupsJoinerGet.DocumentCompleted += groupsJoinerGet_DocumentCompleted;
            groupsJoiner.DocumentCompleted += groupsJoiner_DocumentCompleted;
            groupsGeter.DocumentCompleted += groupsGeter_DocumentCompleted;
        }

        int beg = 5, loop = 0, currnav = 1;
        bool working = false;
        class FacebookGroups
        {
            public string name;
            public string id;
            public string link;
            public uint members;
        }
        static List<FacebookGroups> fbGps = new List<FacebookGroups>();
        public void AddNwGp(string name, string absoluteUrl, uint members)
        {
            var gp = new FacebookGroups();
            gp.name = name;
            gp.id = absoluteUrl.Replace("/groups/", "");
            gp.members = members;
            gp.link = "https://m.facebook.com/groups/" + gp.id + "/";
            string x = gp.name + "- ID:: " + gp.id;
            if (!listBox2.Items.Contains(x))
            {
                listBox2.Items.Add(x);
                fbGps.Add(gp);
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            fbGps.Clear();
            beg = 5;
            listBox2.Items.Clear();
            loop = int.Parse(lim.Text) / 5;
            loop *= 2;
            if (keyword.Text != "")
                this.groupsJoinerGet.Navigate("https://m.facebook.com/search/?query=" + keyword.Text + "&search=group&pn=" + currnav + "&ssid=0&s=" + beg);//+ "&tsid&o=69&refid=46");
            else
                MessageBox.Show("Invalid keyword");

        }
        bool joinOnline = false;
        private void button9_Click(object sender, EventArgs e)
        {
            joinedL.Clear();
            joined = 0;
            joinOnline = true;
            canwait = false;
            if (listBox2.Items.Count == 0)
            {
                MessageBox.Show("Please search for groups first.");
                return;
            }
            listBox2.SelectedIndex = 0;
            string[] k = listBox2.SelectedItem.ToString().Split(new string[] { "- ID:: " }, StringSplitOptions.RemoveEmptyEntries);
            string link = fbGps.Where(p => p.id == k[1]).SingleOrDefault().link;
            groupsJoiner.Navigate(link);
        }
        int joined = 0; bool canwait = false;
        static List<string> joinedL = new List<string>();
        async void groupsJoiner_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != groupsJoiner.Url) return;
            if (!joinOnline) return;
            var curr = fbGps.Where(p => p.id == e.Url.AbsolutePath.Replace("/groups/", "").Replace("/", "")).SingleOrDefault();
            try
            {

                if (!joinedL.Contains(e.Url.ToString()))
                {
                    int tos = 0, p = 0;
                    if (canwait)
                    {
                        if (joined % 10 != 0)
                        {
                            tos = rnd.Next(int.Parse(gpjsI.Text), int.Parse(gpsJF.Text));
                            tos *= 1000;
                            p = tos;
                        }
                        else
                        {
                            tos = 10 * 1000 * 60;// 10 minutes break between each 10 joins
                            p = tos;
                        }
                        int mins = 0;
                        while (tos >= 60000)
                        {
                            mins++;
                            tos -= 60000;
                        }
                        WriteLine("Waiting for " + mins + " mins and " + (tos / 1000) + " seconds before next join!");
                        await wait(p);
                        canwait = false;
                    }
                    if (listBox2.SelectedIndex >= (listBox2.Items.Count - 1))
                    {
                        MessageBox.Show("Process is done.");
                        WriteLine("Total joined groups : " + joined);
                        return;
                    }
                    bool open = true;
                    foreach (HtmlElement f in groupsJoiner.Document.All)
                    {
                        if (f.InnerHtml == null) continue;
                        if (f.InnerHtml.Contains("Closed Group"))
                        {
                            open = false;
                            break;
                        }
                    }
                    if (curr.members > int.Parse(mmslimit.Text))
                    {
                        if (comboBox1.SelectedIndex == 1 && !open || comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 2 && open)
                        {

                            foreach (HtmlElement form in groupsJoiner.Document.Forms)
                            {
                                if (form.InnerHtml.Contains("Join Group"))
                                {
                                    form.InvokeMember("submit");
                                    joined++;
                                    canwait = true;
                                    joinedL.Add(e.Url.ToString());
                                    WriteLine("Joined group : " + curr.name + " id: " + curr.id);
                                    break;
                                }
                            }
                        }
                        else WriteLine("Skipped " + curr.name + " because it is " + (open ? "Opened" : "Closed") + ".");
                    }
                    else WriteLine("Skipped " + curr.name + " because it has only " + curr.members + " members.");
                }
                if (listBox2.SelectedIndex < (listBox2.Items.Count - 1))
                    listBox2.SelectedIndex++;
                string[] k = listBox2.SelectedItem.ToString().Split(new string[] { "- ID:: " }, StringSplitOptions.RemoveEmptyEntries);
                string link = fbGps.Where(p => p.id == k[1]).SingleOrDefault().link;
                groupsJoiner.Navigate(link);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            joinOnline = false;
            WriteLine("Stopped the joining process!");
            WriteLine("Total joined groups : " + joined);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] k = listBox2.SelectedItem.ToString().Split(new string[] { "- ID:: " }, StringSplitOptions.RemoveEmptyEntries);
            var cur = fbGps.Where(p => p.id == k[1] && p.name == k[0]).SingleOrDefault();
            id.Text = cur.id;
            ng.Text = cur.name;
            mc.Text = cur.members.ToString();
            linkLabel1.Text = cur.link.Replace("m.", "");
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(linkLabel1.Text);
            MessageBox.Show("Link copied to clipboard!");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}