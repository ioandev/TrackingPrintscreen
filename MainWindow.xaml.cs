using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace TrackPrintScreen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon trayIcon;
        OptionsWindow optionsWindow = null;
        Screenshotter screenshotter = null;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = null;
        Options options;
        string configPath = "config.txt";
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Minimized;
            Hide();

            if (!System.IO.File.Exists(configPath))
            {
                System.IO.File.WriteAllText(configPath, System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath));
            }
            options = OptionsFiledatabase.Load(configPath);

            while (!System.IO.Directory.Exists(options.FolderPath) && revertToThisFolderDialog() == false) ;

            if (!System.IO.Directory.Exists(options.FolderPath))
            {
                System.IO.File.WriteAllText(configPath, System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath));
                options = OptionsFiledatabase.Load(configPath);
            }
            screenshotter = new Screenshotter(options);



            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon("icon.ico"), //Resources.AppIcon,
                Visible = true,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Shot", SpecialShot),
                    new MenuItem("Open Screenshot Folder", OpenScreenShotFolder),
                    new MenuItem("Options", OpenOptions),
                    new MenuItem("Exit", Exit)
                })
            };
            trayIcon.DoubleClick += OpenOptions;
            
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 2, 30);
            dispatcherTimer.Start();

            screenshotter.Shot(); // initial shot at the start of the application

            verifyDiskSpace();
    }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            if(optionsWindow != null)
            {
                optionsWindow.Close();
            }

            Environment.Exit(0);
        }

        void OpenOptions(object sender, EventArgs e)
        {
            if (optionsWindow != null) return;

            optionsWindow = new OptionsWindow(options);
            optionsWindow.Closed += OptionsWindowClosed;
            optionsWindow.Show();
        }

        void OptionsWindowClosed(object sender, EventArgs e)
        {
            options = optionsWindow.getOptions();
            OptionsFiledatabase.Save(configPath, options);
            optionsWindow = null;
            screenshotter.UpdateOptions(options);
        }

        void SpecialShot(object sender, EventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(400) };
            timer.Start();
            timer.Tick += (s, a) =>
            {
                timer.Stop();
                screenshotter.Shot(true);
            };
        }

        void OpenScreenShotFolder(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", options.FolderPath);
        }
        
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            screenshotter.Shot();
        }

        private void verifyDiskSpace()
        {
            FileInfo f = new FileInfo(options.FolderPath);
            string drive = System.IO.Path.GetPathRoot(f.FullName);

            var freespace = GetTotalFreeSpace(drive);

            if(freespace < 1024 * 1024 * 100)
            {
                System.Windows.Forms.MessageBox.Show("Tracking print screen space is lower than 100MB.");
            }
        }

        private long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }

        private bool revertToThisFolderDialog()
        {
            string sMessageBoxText = "Couldn't save the picture in the setting's folder. Want to revert to default?";
            string sCaption = "Screenshotter";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = System.Windows.MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                case MessageBoxResult.Cancel:
                    Environment.Exit(0);
                    break;
            }

            return false;
        }
    }
}