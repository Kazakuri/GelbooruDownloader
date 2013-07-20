using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading.Tasks;

namespace GelbooruDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int XML_ELEMENT_IMAGE_URL = 7;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdSelectDestination_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = dialog.SelectedPath;
            }
        }

        private void cmdSearch_Click(object sender, RoutedEventArgs e)
        {
            Download(txtTags.Text);
            //var task = Task.Factory.StartNew(() => Download(txtTags.Text), TaskCreationOptions.LongRunning);            
        }

        public void Download(string tag)
        {
            string search = "http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags=" + tag;
            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(search);

            HttpWebResponse response = (HttpWebResponse)
            request.GetResponse();

            Stream ReceiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(ReceiveStream);
            string contents = readStream.ReadToEnd();

            XmlReader reader = XmlReader.Create(new StringReader(contents));
            reader.ReadToFollowing("posts");
            reader.MoveToFirstAttribute();
            int images = int.Parse(reader.Value);
            int imageCount = images;
            int count = 0;
            int page = 0;

            while (images > 0)
            {
                reader.ReadToFollowing("post");
                reader.MoveToAttribute(XML_ELEMENT_IMAGE_URL);
                WebClient myWebClient = new WebClient();
                string file = reader.Value.Substring(reader.Value.LastIndexOf('/') + 1);
                string path = txtPath.Text + "\\" + file;
                myWebClient.DownloadFile(reader.Value, path);
                images--;
                count++;

                if (count == 100)
                {
                    count = 0;
                    page++;

                    search = "http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags=" + tag + "&pid=" + page;
                    request = (HttpWebRequest)
                    WebRequest.Create(search);
                    response = (HttpWebResponse)
                    request.GetResponse();

                    ReceiveStream = response.GetResponseStream();
                    readStream = new StreamReader(ReceiveStream);
                    contents = readStream.ReadToEnd();

                    reader = null;
                    reader = XmlReader.Create(new StringReader(contents));
                    reader.ReadToFollowing("posts");
                }
            }

            MessageBox.Show("Finished downloading " + imageCount + " images.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
