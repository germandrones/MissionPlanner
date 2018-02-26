using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerLand : GMapMarker
    {
        #region Private Fields
        const int picker_offset = 20;

        public int m_wprad;
        private double m_absolute_start_angle = 0;
        private double m_angle_offset = 0;

        private PointF m_scroller_1;
        private PointF m_scroller_2;
        private int m_scroller_radius = 10;

        private double DegToRad(double angle) { return (Math.PI / 180) * angle; }

        #endregion

        #region Public Fields
        public bool scroller1_selected;
        public bool scroller2_selected;
        public int wpno;

        public double StartAngle
        {
            get { return m_absolute_start_angle; }
            set { m_absolute_start_angle = value; }
        }

        public double AngleOffset
        {
            get { return m_angle_offset; }
            set { m_angle_offset = value; }
        }
        #endregion

        #region Constructor
        public GMapMarkerLand(PointLatLng p, double radius, double absolute_start_angle, double angle_offfset) : base(p)
        {
            m_wprad = (int)radius;
            m_absolute_start_angle = absolute_start_angle;
            m_angle_offset = angle_offfset;
        }
        #endregion

        #region Scroller Picker
        public void scrollerPicker(int x, int y)
        {
            if (Math.Abs(x - m_scroller_1.X) < picker_offset && Math.Abs(y - m_scroller_1.Y) < picker_offset) { scroller1_selected = true; } else { scroller1_selected = false; }
            if (Math.Abs(x - m_scroller_2.X) < picker_offset && Math.Abs(y - m_scroller_2.Y) < picker_offset) { scroller2_selected = true; } else { scroller2_selected = false; }
        }
        #endregion

        #region onRender only visualization
        public override void OnRender(Graphics g)
        {
            base.OnRender(g);

            if (m_wprad == 0 || Overlay.Control == null) return;

            double width = (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0), Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
            double height = (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0), Overlay.Control.FromLocalToLatLng(Overlay.Control.Height, 0)) * 1000.0);
            double m2pixelwidth = Overlay.Control.Width / width;
            double m2pixelheight = Overlay.Control.Height / height;

            GPoint loc = new GPoint((int)(LocalPosition.X - (m2pixelwidth * m_wprad * 2)), LocalPosition.Y);

            int x = LocalPosition.X - (int)(Math.Abs(loc.X - LocalPosition.X) / 2);
            int y = LocalPosition.Y - (int)(Math.Abs(loc.X - LocalPosition.X) / 2);

            int widtharc = (int)Math.Abs(loc.X - LocalPosition.X);
            int heightarc = (int)Math.Abs(loc.X - LocalPosition.X);

            double t_angle1 = m_absolute_start_angle - 90;
            double t_angle2 = m_angle_offset;

            int t_radius = (widtharc / 2);
            m_scroller_1.X = LocalPosition.X - m_scroller_radius + (int)(t_radius * Math.Cos(DegToRad(t_angle1)));
            m_scroller_1.Y = LocalPosition.Y - m_scroller_radius + (int)(t_radius * Math.Sin(DegToRad(t_angle1)));

            m_scroller_2.X = LocalPosition.X - m_scroller_radius + (int)(t_radius * Math.Cos(DegToRad(t_angle2 + t_angle1)));
            m_scroller_2.Y = LocalPosition.Y - m_scroller_radius + (int)(t_radius * Math.Sin(DegToRad(t_angle2 + t_angle1)));


            if (widtharc > 0 && widtharc < 200000000 && Overlay.Control.Zoom > 3)
            {
                // fill arc green / red color
                g.FillPie(new SolidBrush(Color.FromArgb(30, Color.Green)), x, y, widtharc, heightarc, 0, 360);
                g.FillPie(new SolidBrush(Color.FromArgb(70, Color.Red)), x, y, widtharc, heightarc, (int)(t_angle1), (int)(t_angle2));


                //draw two scrollers
                g.FillPie(new SolidBrush(Color.FromArgb(255, scroller1_selected ? Color.Blue : Color.Green )), m_scroller_1.X, m_scroller_1.Y, m_scroller_radius * 2, m_scroller_radius * 2, 0, 360);
                g.FillPie(new SolidBrush(Color.FromArgb(255, scroller2_selected ? Color.Blue : Color.Green )), m_scroller_2.X, m_scroller_2.Y, m_scroller_radius * 2, m_scroller_radius * 2, 0, 360);

            }
        }
        #endregion
    }
}