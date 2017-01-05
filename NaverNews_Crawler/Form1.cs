using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

using System.Net.Json;

namespace NaverNews_Crawler
{
    public partial class Form1 : Form
    {
        JsonTextParser parser = new JsonTextParser();

        string sortType = "new";
        string searchTarget = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void getComments(string url)
        {
            string[] temp = url.Split(new string[] { "oid=" }, StringSplitOptions.None);
            if (temp.Length <= 1)
            {
                return;
            }
            String oid = temp[1].Split('&')[0];

            temp = url.Split(new string[] { "aid=" }, StringSplitOptions.None);
            if (temp.Length <= 1)
            {
                return;
            }
            String aid = temp[1].Split('&')[0];

            if (oid=="" || aid=="")
            {
                return;
            }

            int page,
                commentCount=0;
            listView1.Items.Clear();

            for (page=1; page<100; page++)
            {
                String ServerUrl = "https://apis.naver.com/commentBox/cbox/web_naver_list_jsonp.json?ticket=news&templateId=default_politics&pool=cbox5&lang=ko&country=KR&objectId=news" + oid + "%2C" + aid + "&categoryId=&pageSize=100&indexSize=10&groupId=&page="+page+"&initialize=true&userType=&useAltSort=true&replyPageSize=20&moveTo=&sort=" + sortType;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServerUrl);
                request.Method = "post";
                request.Referer = url;


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader readerPost = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8, true);
                string resResult = readerPost.ReadToEnd();
                readerPost.Close();
                response.Close();

                resResult = resResult.Substring(10, resResult.Length - 12);

                JsonObject obj = parser.Parse(resResult);
                JsonObjectCollection objCol = (JsonObjectCollection)obj;
                objCol = (JsonObjectCollection)objCol["result"];

                JsonArrayCollection commentList = (JsonArrayCollection)objCol["commentList"];


                string href_head = "";
                if (ServerUrl.IndexOf("entertain.naver.com") == -1)
                {
                    href_head = "http://news.naver.com/main/read.nhn";
                }
                else
                {
                    href_head = "http://entertain.naver.com/comment/list";
                }

                int i = 0;
                commentCount += commentList.Count;
                if (commentList.Count == 0)
                {
                    label2.Text = "검색결과 : " + commentCount;
                    return;
                }

                for (i = 0; i < commentList.Count; i++)
                {
                    JsonObjectCollection comment = (JsonObjectCollection)commentList[i];

                    string userId    = (String)comment["userIdNo"].GetValue();
                    if (userId != searchTarget && searchTarget != "")
                    {
                        continue;
                    }

                    string commentNo = comment["commentNo"].GetValue().ToString();
                    string contents  = (String)comment["contents"].GetValue();
                    string nickname  = (String)comment["maskedUserId"].GetValue();
                    string date      = (String)comment["regTime"].GetValue();

                    string cLink = href_head + "?commentNo=" + commentNo + "&oid=" + oid + "&aid=" + aid;
                    listView1.Items.Add(new ListViewItem(new String[] { contents, nickname, date, userId, cLink }));
                }
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            getComments(textBox1.Text);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start(listView1.FocusedItem.SubItems[4].Text);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem.ListViewSubItemCollection focusItem=listView1.FocusedItem.SubItems;

            searchTarget = focusItem[3].Text;
            label1.Text = "검색대상 : " + focusItem[3].Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {


        }



        private void checkBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                checkBox1.Checked = true;
                return;
            }
            checkBox2.Checked = false;
            sortType = "favorite";
            getComments(textBox1.Text);
        }
        private void checkBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                checkBox2.Checked = true;
                return;
            }
            checkBox1.Checked = false;

            sortType = "new";

            getComments(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            searchTarget = "";
            label1.Text = "";
        }
    }
}
