using System;
using System.Collections.Generic;
using System.Linq;
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

//Custom
using System.Net.Http;
using System.Diagnostics; // For logging purposes, unneccesary for final build.
using Microsoft.Win32;
using System.IO.Compression;
using System.IO;

namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //User custom vars
        static string appName;
        static string exeName;
        static string versionURL;
        static string updateURL;

        //Code vars
        static RegistryKey key;
        static string path;

        //UI vars
        static MainWindow window;

        public MainWindow()
        {
            Hide();
            window = this;

            appName = "Doge";
            exeName = "doge.exe";
            versionURL = "https://raw.githubusercontent.com/ryanhlewis/updatelauncher/exampleapp/dogeversion.txt";
            updateURL = "https://github.com/ryanhlewis/updatelauncher/blob/exampleapp/doge.zip?raw=true";

            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = System.IO.Path.Combine(path, appName);
            Title = appName;

            Main();

            InitializeComponent();
          

            MouseLeftButtonDown += DragAnywhere;

        }



        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                //Grab version

                string responseBody = await client.GetStringAsync(versionURL);

                if (String.IsNullOrEmpty(responseBody) || responseBody.Length != 5)
                {
                    System.Windows.Application.Current.Shutdown();
                }

                Debug.WriteLine("Successfully got version: " + responseBody);

                //Ensure .exe exists in folder
                bool exeCheck = File.Exists(System.IO.Path.Combine(path, exeName));
                Debug.WriteLine(System.IO.Path.Combine(path, exeName) + "  " + exeCheck);

                //Compare version to version in registry

                key = Registry.CurrentUser.OpenSubKey("Software", true);

                key.CreateSubKey(appName);
                key = key.OpenSubKey(appName, true);

                //First time run check
                if (key.GetValue("version") == null)
                {
                    Debug.WriteLine("First time in Updater, setting version and downloading game");
                    key.SetValue("version", responseBody);
                    await getUpdate();
                    return;
                }

                Debug.WriteLine("Registry version: " + key.GetValue("version"));

                int regVersion = int.Parse(((string)(key.GetValue("version"))).Replace(".", "")); //ex. 0.0.0 -> 0
                int newVersion = int.Parse(responseBody.Replace(".", ""));                        // 1.2.3 -> 123

                if (regVersion == newVersion && exeCheck)
                {                                                       //Same version, quit to game.
                    Debug.WriteLine("Same version- going to game!");
                    goToGame();
                }
                else if(regVersion < newVersion || !exeCheck)
                {                                                          //Lower version, grab new one. On success, update registry.
                    Debug.WriteLine("Different versions- downloading new update");
                    
                    await getUpdate();
                    key.SetValue("version", responseBody);
                }


            }
            catch (Exception e)
            {
                Debug.WriteLine("\nException Caught!");
                Debug.WriteLine("Message :{0} ", e.Message);
                System.Windows.Application.Current.Shutdown();
            }
        }

        static async Task getUpdate()
        {

            window.Show();

            Debug.WriteLine("Getting new update");

            FileStream s = new FileStream(System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".zip", FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096);

            try
            {
              

                Debug.WriteLine(path);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                HttpResponseMessage response = await client.GetAsync(updateURL);
                await response.Content.CopyToAsync(s);

                s.Close();

                

                ZipFile.ExtractToDirectory(s.Name, path, true);

                Debug.WriteLine("Deleting temp file " + s.Name);

                File.Delete(s.Name);

                Debug.WriteLine("Extracted successfully to " + path);

           

                goToGame();

            }
            catch (Exception e)
            {
                Debug.WriteLine("\nException Caught!" + e);
                Debug.WriteLine("Message :{0} ", e.Message);
                File.Delete(s.Name);
                System.Windows.Application.Current.Shutdown();
            }
        }

        static private void goToGame()
        {
            
            System.Windows.Application.Current.Shutdown();
            Process.Start(path + "\\" + exeName);
        }

        private void DragAnywhere(object sender, MouseButtonEventArgs e)
        {
            DragMove(); // Move around window without clicking border.
        }

    }
}
