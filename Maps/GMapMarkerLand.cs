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
        public Pen Pen = new Pen(Brushes.White, 2);

        Color? initcolor = null;

        public GMapMarker InnerMarker;

        private int m_wprad = 200; //default
        private double m_unsafe_area_angle_1 = 0;
        private double m_unsafe_area_angle_2 = 0;

        private PointLatLng m_scroller1;
        private PointLatLng m_scroller2;

        public PointLatLng Scroller1
        {
            get { return m_scroller1; }
            set { m_scroller1 = value; }
        }

        public PointLatLng Scroller2
        {
            get { return m_scroller2; }
            set { m_scroller2 = value; }
        }

        public Color Color
        {
            get { return Pen.Color; }
            set
            {
                if (!initcolor.HasValue) initcolor = value;
                Pen.Color = value;
            }
        }


        public GMapMarkerLand(PointLatLng p, double radius, double unsafe_area_angle_1, double unsafe_area_angle_2) : base(p)
        {
            m_wprad = (int)radius;
            m_unsafe_area_angle_1 = unsafe_area_angle_1-90;
            m_unsafe_area_angle_2 = Math.Abs(m_unsafe_area_angle_1) + (unsafe_area_angle_2 - 90);

            m_unsafe_area_angle_1 = unsafe_area_angle_1 - 90;
            m_unsafe_area_angle_2 = unsafe_area_angle_2 - m_unsafe_area_angle_1 -90;

            Pen.DashStyle = DashStyle.Solid;
            Pen.Color = Color.Green;
        }

        private double DetToRad(double angle)
        {
            return (Math.PI / 180) * angle;
        }

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

            int t_radius = (widtharc / 2);
            int scroller_radius = 4;
            int scroller1_x = LocalPosition.X - scroller_radius + (int)(t_radius * Math.Cos(DetToRad(m_unsafe_area_angle_1)));
            int scroller1_y = LocalPosition.Y - scroller_radius + (int)(t_radius * Math.Sin(DetToRad(m_unsafe_area_angle_1)));
            int scroller2_x = LocalPosition.X - scroller_radius + (int)(t_radius * Math.Cos(DetToRad(m_unsafe_area_angle_2)));
            int scroller2_y = LocalPosition.Y - scroller_radius + (int)(t_radius * Math.Sin(DetToRad(m_unsafe_area_angle_2)));

            if (widtharc > 0 && widtharc < 200000000 && Overlay.Control.Zoom > 3)
            {
                // fill arc green color
                g.FillPie(new SolidBrush(Color.FromArgb(50, Color.Green)), x, y, widtharc, heightarc, 0, 360);

                // draw unsafe area arc
                g.FillPie(new SolidBrush(Color.FromArgb(50, Color.Red)), x, y, widtharc, heightarc, (int)(m_unsafe_area_angle_1), (int)(m_unsafe_area_angle_2));

                //draw two scrollers
                //g.FillPie(new SolidBrush(Color.FromArgb(250, Color.Red)), scroller1_x, scroller1_y, scroller_radius * 2, scroller_radius * 2, 0, 360);
                //g.FillPie(new SolidBrush(Color.FromArgb(250, Color.Red)), scroller2_x, scroller2_y, scroller_radius * 2, scroller_radius * 2, 0, 360);

            }
        }
    }
}