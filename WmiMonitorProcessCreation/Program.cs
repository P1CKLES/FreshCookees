using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace GetWMI_Info
{

    public static class Page
    {
        public static string page = null;
    }


    public class EventWatcherAsync
    {


        [DllImport("user32", CharSet = CharSet.Auto)]
        static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);


        private void WmiEventHandler(object sender, EventArrivedEventArgs e)
        {
            string name = ((ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value)["Name"].ToString();
            string stopped = "iexplore.exe";

            if (name == stopped)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[*] Process stop Alert!      " + name);
                Console.WriteLine("[*] TargetInstance.Name :      " + ((ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value)["Name"]);
                Console.WriteLine("[*] TargetInstance.Handle :          " + ((ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value)["Handle"]);
                Console.ResetColor();
                Process[] proc = Process.GetProcessesByName("iexplore");
                if (proc.Length == 0)
                {
                    startIE();
                }
            }
        }

        public EventWatcherAsync()
        {
            try
            {
                string ComputerName = "localhost";
                string WmiQuery;
                ManagementEventWatcher Watcher;
                ManagementScope Scope;

                Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", ComputerName), null);
                Scope.Connect();
                
                WmiQuery = "Select * From __InstanceDeletionEvent Within 1 " + "Where TargetInstance ISA 'Win32_Process' ";

                Watcher = new ManagementEventWatcher(Scope, new EventQuery(WmiQuery));
                Watcher.EventArrived += new EventArrivedEventHandler(this.WmiEventHandler);
                Watcher.Start();
                while (true)
                {
                    Watcher.WaitForNextEvent();
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string title = @" 
                ______               __       ______            __                 
               / ____/_______  _____/ /_     / ____/___  ____  / /_____  ___  _____
              / /_  / ___/ _ \/ ___/ __ \   / /   / __ \/ __ \/ //_/ _ \/ _ \/ ___/
             / __/ / /  /  __(__  ) / / /  / /___/ /_/ / /_/ / ,< /  __/  __(__  ) 
            /_/   /_/   \___/____/_/ /_/   \____/\____/\____/_/|_|\___/\___/____/  
                                                                       " + "\n\n";



            string options = @"[*] IExplorer Cookie refresher for proxies that provide cookies!  \n[*] Example: AutoRefresh.exe 'myadomain.com/autorefresh.html'" + "\n\n";
            Console.Write(title);
            Console.ResetColor();

            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(options);
                Console.ResetColor();
                Environment.Exit(0);
            }
            else
            {
                Page.page = args[0];
            }

            Console.WriteLine("[*] Listening for iexplore process deletion!");
            EventWatcherAsync eventWatcher = new EventWatcherAsync();

        }

        public void startIE()
        {
            const int FormWidth = 800;
            const int FormHeight = 600;

            System.Drawing.Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            int posX = rect.Width - FormWidth;
            int posY = rect.Height - FormHeight;
            posX = Convert.ToInt32(posX / 2.0);
            posY = Convert.ToInt32(posY / 2.0);
            posX = Math.Max(posX, 0);
            posY = Math.Max(posY, 0);

            System.Type oType = System.Type.GetTypeFromProgID("InternetExplorer.Application");

            object o = System.Activator.CreateInstance(oType);
            o.GetType().InvokeMember("MenuBar", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { 0 });
            o.GetType().InvokeMember("ToolBar", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { 0 });
            o.GetType().InvokeMember("StatusBar", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { 0 });
            o.GetType().InvokeMember("AddressBar", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { 0 });
            o.GetType().InvokeMember("Visible", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { false });
            o.GetType().InvokeMember("Top", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { posY });
            o.GetType().InvokeMember("Left", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { posX });
            o.GetType().InvokeMember("Width", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { FormWidth });
            o.GetType().InvokeMember("Height", System.Reflection.BindingFlags.SetProperty, null, o, new object[] { FormHeight });
            o.GetType().InvokeMember("Navigate", System.Reflection.BindingFlags.InvokeMethod, null, o, new object[] { Page.page });

            try
            {
                object ohwnd = o.GetType().InvokeMember("hwnd", System.Reflection.BindingFlags.GetProperty, null, o, null);
                System.IntPtr IEHwnd = (System.IntPtr)ohwnd;
                NativeMethods.ShowWindow(IEHwnd, NativeMethods.WindowShowStyle.Hide);
            }
            catch (Exception ex)
            {
            }
        }



        public class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(
                System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(
                System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool ShowWindow(IntPtr hwnd, WindowShowStyle nCmdShow);


            /// <summary>Enumeration of the different ways of showing a window using
            /// ShowWindow</summary>
            public enum WindowShowStyle : int
            {
                /// <summary>Hides the window and activates another window.</summary>
                /// <remarks>See SW_HIDE</remarks>
                Hide = 0,
                /// <summary>Activates and displays a window. If the window is minimized
                /// or maximized, the system restores it to its original size and
                /// position. An application should specify this flag when displaying
                /// the window for the first time.</summary>
                /// <remarks>See SW_SHOWNORMAL</remarks>
                ShowNormal = 1,
                /// <summary>Activates the window and displays it as a minimized window.</summary>
                /// <remarks>See SW_SHOWMINIMIZED</remarks>
                ShowMinimized = 2,
                /// <summary>Activates the window and displays it as a maximized window.</summary>
                /// <remarks>See SW_SHOWMAXIMIZED</remarks>
                ShowMaximized = 3,
                /// <summary>Maximizes the specified window.</summary>
                /// <remarks>See SW_MAXIMIZE</remarks>
                Maximize = 3,
                /// <summary>Displays a window in its most recent size and position.
                /// This value is similar to "ShowNormal", except the window is not
                /// actived.</summary>
                /// <remarks>See SW_SHOWNOACTIVATE</remarks>
                ShowNormalNoActivate = 4,
                /// <summary>Activates the window and displays it in its current size
                /// and position.</summary>
                /// <remarks>See SW_SHOW</remarks>
                Show = 5,
                /// <summary>Minimizes the specified window and activates the next
                /// top-level window in the Z order.</summary>
                /// <remarks>See SW_MINIMIZE</remarks>
                Minimize = 6,
                /// <summary>Displays the window as a minimized window. This value is
                /// similar to "ShowMinimized", except the window is not activated.</summary>
                /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
                ShowMinNoActivate = 7,
                /// <summary>Displays the window in its current size and position. This
                /// value is similar to "Show", except the window is not activated.</summary>
                /// <remarks>See SW_SHOWNA</remarks>
                ShowNoActivate = 8,
                /// <summary>Activates and displays the window. If the window is
                /// minimized or maximized, the system restores it to its original size
                /// and position. An application should specify this flag when restoring
                /// a minimized window.</summary>
                /// <remarks>See SW_RESTORE</remarks>
                Restore = 9,
                /// <summary>Sets the show state based on the SW_ value specified in the
                /// STARTUPINFO structure passed to the CreateProcess function by the
                /// program that started the application.</summary>
                /// <remarks>See SW_SHOWDEFAULT</remarks>
                ShowDefault = 10,
                /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
                /// that owns the window is hung. This flag should only be used when
                /// minimizing windows from a different thread.</summary>
                /// <remarks>See SW_FORCEMINIMIZE</remarks>
                ForceMinimized = 11
            }

        }
    }
}
