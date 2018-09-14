using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPython.Runtime;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class Loading : Form
    {
        static Loading Instance;

        static object locker = new object();

        public Loading()
        {
            InitializeComponent();
        }

        ~Loading()
        {
            Instance = null;
        }

        public new string Text 
        {
            get { return label1.Text; }
            set
            {
                try
                {
                    if (this.IsHandleCreated && !IsDisposed)
                    {
                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker) delegate
                            {
                                label1.Text = value;
                                this.Focus();
                                this.Refresh();
                            });
                        }
                        else
                        {
                            label1.Text = value;
                            this.Focus();
                            this.Refresh();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public new static void Close()
        {
            lock (locker)
            {
                if (Instance != null)
                {
                    if (!Instance.IsDisposed)
                    {
                        if (Instance.IsHandleCreated)
                        {

                            MainV2.instance.Invoke((MethodInvoker) delegate { ((Form) Instance).Close(); });

                            Instance = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a new dialog or use an existing one if its still valid
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static void ShowLoading(string Text, IWin32Window owner = null)
        {
            // ensure we only have one instance at a time
            lock (locker)
            {
                if (Instance != null && !Instance.IsDisposed)
                {
                    Instance.Text = Text;
                    return;
                }

                // create form on ui thread
                MainV2.instance.Invoke((MethodInvoker) delegate
                {
                    Loading frm = new Loading();
                    if(owner == null)
                        frm.TopMost = true;
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.Closing += Frm_Closing;

                    // set instance
                    Instance = frm;
                    // set text
                    Instance.label1.Text = Text;

                    ThemeManager.ApplyThemeTo(frm);
                    frm.Show(owner);
                    frm.Focus();
                });
            }
        }

        private static void Frm_Closing(object sender, CancelEventArgs e)
        {
            lock (locker)
            {
                Instance = null;
            }
        }
    }
}
