using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaptureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static string PublicIP { get; private set; }

        internal static string IPV4 { get; private set; }

        internal static string MachineName { get; private set; }

        internal static string Username { get; private set; }

        internal static string UserPromptInput { get; set; }

        internal static string WebApiURL { get; private set; }




        public MainWindow()
        {
            InitializeComponent();
        }

        private void btngetinfo_Click(object sender, RoutedEventArgs e)
        {


            WebApiURL = GetWebApiURL();
            Input inp = new Input();
            inp.ShowDialog();


            PublicIP = GetPublicIP();
            IPV4 = GetLocalIPAddress();
            MachineName = GetMachineName();
            Username = GetUsername();

        }

        public static string GetPublicIP()
        {
            try
            {
                string externalip = new WebClient().DownloadString("http://icanhazip.com") ?? null;

                return externalip;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);

                return string.Empty;
            }
        }

        public static string GetUsername()
        {
            try
            {
                return Environment.UserName ?? string.Empty;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        public static string GetMachineName()
        {
            try
            {
                return Environment.MachineName ?? string.Empty;
            }
            catch (Exception ex)
            {

                return string.Empty;
            }
        }

        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;

            }

        }


        public static string GetWebApiURL()
        {
            try
            {
                return ConfigurationManager.AppSettings["apiurl"];

               
            }
            catch (Exception)
            {

                return string.Empty;
            }
        }
        // Perform the equivalent of posting a form with a filename and two files, in HTML:
        // <form action="{url}" method="post" enctype="multipart/form-data">
        //     <input type="text" name="filename" />
        //     <input type="file" name="file1" />
        //     <input type="file" name="file2" />
        // </form>
        private async Task<System.IO.Stream> UploadAsync(string url, string filename, Stream fileStream, byte[] fileBytes)
        {
            // Convert each of the three inputs into HttpContent objects

            HttpContent stringContent = new StringContent(filename);
            // examples of converting both Stream and byte [] to HttpContent objects
            // representing input type file
            HttpContent fileStreamContent = new StreamContent(fileStream);
            HttpContent bytesContent = new ByteArrayContent(fileBytes);

            // Submit the form using HttpClient and 
            // create form data as Multipart (enctype="multipart/form-data")

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                // Add the HttpContent objects to the form data

                // <input type="text" name="filename" />
                formData.Add(stringContent, "filename", "filename");
                // <input type="file" name="file1" />
                formData.Add(fileStreamContent, "file1", "file1");
                // <input type="file" name="file2" />
                formData.Add(bytesContent, "file2", "file2");

                // Invoke the request to the server

                // equivalent to pressing the submit button on
                // a form with attributes (action="{url}" method="post")
                var response = await client.PostAsync(url, formData);

                // ensure the request was a success
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                return await response.Content.ReadAsStreamAsync();
            }
        }
    }
}
