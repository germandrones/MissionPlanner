using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Properties;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews
{
    public partial class Help : MyUserControl, IActivate
    {
        public Help()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            // We can show compiled chm file or, we can just parse the contents.
            //System.Windows.Forms.Help.ShowHelp(this, HelpFile);

            string HelpDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\UsrManual\\";
            string HelpFile = "UserManual\\GDMP.chm";
            webBrowser1.Navigate(new System.Uri(@"file://D:\dev\MissionPlanner\UserManual\index.html"));


            /*try
            {
                CHK_showconsole.Checked = Settings.Instance.GetBoolean("showconsole");
            }
            catch
            {
            }

            if (Program.WindowsStoreApp)
            {
                BUT_betaupdate.Visible = false;
                BUT_updatecheck.Visible = false;
            }*/
        }

        public void BUT_updatecheck_Click(object sender, EventArgs e)
        {
            try
            {
                Utilities.Update.CheckForUpdate(true);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }

        private void CHK_showconsole_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["showconsole"] = CHK_showconsole.Checked.ToString();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            //richTextBox1.Rtf = Resources.help_text;
            //ThemeManager.ApplyThemeTo(richTextBox1);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://firmware.ardupilot.org/Tools/MissionPlanner/upgrade/ChangeLog.txt");
        }

        private void PIC_wizard_Click(object sender, EventArgs e)
        {
            var cfg = new Wizard.Wizard();

            cfg.ShowDialog(this);
        }

        private void BUT_betaupdate_Click(object sender, EventArgs e)
        {
            try
            {
                Utilities.Update.dobeta = true;
                if (Control.ModifierKeys == Keys.Control)
                    Utilities.Update.domaster = true;
                Utilities.Update.DoUpdate();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }

        
    }
}