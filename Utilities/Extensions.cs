﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public static class Extensions
    {
        public static void Stop(this System.Threading.Timer timer)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void Start(this System.Threading.Timer timer, int intervalms)
        {
            timer.Change(intervalms, intervalms);
        }

        public static void LogInfoFormat(this Control ctl, string format, params object[] args)
        {
        }

        public static void LogErrorFormat(this Control ctl, string format, params object[] args)
        {
        }

        public static void LogInfo(this Control ctl, object ex)
        {
        }

        public static void LogError(this Control ctl, object ex)
        {
        }

        public static void ShowUserControl(this UserControl ctl)
        {
            Form frm = new Form();
            int header = frm.Height - frm.ClientRectangle.Height;
            frm.Text = ctl.Text;
            frm.Size = ctl.Size;
            // add the header height
            frm.Height += header;
            frm.Tag = ctl;
            ctl.Dock = DockStyle.Fill;
            frm.Controls.Add(ctl);
            frm.Load += Frm_Load;
            frm.Closing += Frm_Closing;
            frm.Show();
        }

        private static void Frm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((Form)sender).Tag is MissionPlanner.Controls.IDeactivate)
            {
                ((MissionPlanner.Controls.IDeactivate)((Form)sender).Tag).Deactivate();
            }
        }

        private static void Frm_Load(object sender, EventArgs e)
        {
            if (((Form)sender).Tag is MissionPlanner.Controls.IActivate)
            {
                ((MissionPlanner.Controls.IActivate)((Form)sender).Tag).Activate();
            }
        }

        static ConcurrentDictionary<Action, long> reentryDictionary = new ConcurrentDictionary<Action, long>();

        public static void ProtectReentry(Action action)
        {
            long m_InFunction = reentryDictionary.ContainsKey(action) ? reentryDictionary[action] : 0;

            if (Interlocked.CompareExchange(ref m_InFunction, 1, 0) == 0)
            {
                // We're not in the function
                try
                {
                    action();
                }
                finally
                {
                    long temp;
                    reentryDictionary.TryRemove(action, out temp);
                }
            }
            else
            {
                // We're already in the function
            }
        }
    }
}
