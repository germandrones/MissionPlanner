using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{

    /// <summary>
    /// A helper class used to check the user defined mission for mistakes or unsafe points.
    /// </summary>
    class MissionChecker
    {
        #region Internal Definitions
        // Structure of mission item like defined in mavcmd.xml
        public struct MissionItem
        {
            int command;
            public double P1, P2, P3, P4;
            public double X, Y, Z;

            public MissionItem(int cmd, double p1, double p2, double p3, double p4, double x, double y, double z)
            {
                command = cmd;
                P1 = p1;
                P2 = p2;
                P3 = p3;
                P4 = p4;
                X = x;
                Y = y;
                Z = z;
            }

            public int getCommand() { return command; }

            public PointLatLngAlt getCoords() { return new PointLatLngAlt(lat:X, lng:Y, alt:Z); }

        }

        public enum MissionCheckerResult
        {
            OK,
            ERROR,
            NO_TAKEOFF_POINT,
            NO_LAND_POINT,
            LANDING_UNSAFE,
            ALTITUDE_UNSAFE,
            GROUND_COLLISION_DETECTED,
        }

        int m_takeoff_id;
        int m_land_id;
        PointLatLngAlt m_land_point;

        double m_hwp_radius = 200; // 200 meters HWP radius by default
        bool   m_hwp_enabled;

        double m_loiter_radius;
        double m_exit_tangent;
        double m_safe_altitude_delta = 20;  // 20 meters of altitude difference is safe for landing?

        List<MissionItem> defined_mission = new List<MissionItem>(); // List of all mission items which are created by user.
        public List<MissionItem> modified_mission = new List<MissionItem>(); // list of mission items modified by mission checker
        #endregion

        #region Public Fields

        public double HWP_RADIUS
        {
            get { return m_hwp_radius; }
            set { m_hwp_radius = value; }
        }

        public bool HWP_ENABLED
        {
            get { return m_hwp_enabled; }
            set { m_hwp_enabled = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public MissionChecker()
        {
            defined_mission.Clear();
        }
        #endregion

        #region Internal Helpers

        // Method returns id of a command
        private int get_command_id(string command)
        {
            return (ushort)(MAVLink.MAV_CMD)Enum.Parse(typeof(MAVLink.MAV_CMD), command);
        }
        
        // Check if first waypoint is a takeoff point
        private void get_takeoff_land_ids(out int takeoff_id, out int land_id)
        {
            takeoff_id = -1;
            land_id = -1;
            for(int i = 0; i < defined_mission.Count; i++)
            {
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.TAKEOFF) takeoff_id = i;
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.LAND || defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.LAND_AT_TAKEOFF) land_id = i;
            }
        }

        private PointLatLngAlt get_loiter_coords(PointLatLngAlt wp_point, PointLatLngAlt land_point)
        {
            PointLatLngAlt loiter_point = wp_point;

            double bearing = wp_point.GetBearing(land_point);
            double distance = wp_point.GetDistance(land_point);

            return loiter_point.newpos(bearing, distance * 0.3f);
        }

        private PointLatLngAlt get_last_nav_coords(PointLatLngAlt wp_point, PointLatLngAlt land_point)
        {
            PointLatLngAlt last_nav_point = wp_point;

            double bearing = wp_point.GetBearing(land_point);
            double distance = wp_point.GetDistance(land_point);

            return last_nav_point.newpos(bearing, distance * 0.6f);
        }


        /// <summary>
        /// Method checks the landing for obstackles using SRTM Data
        /// </summary>
        private bool check_landing_obstacles(PointLatLngAlt land_point)
        {
            bool is_safe = false;

            // hwp_radius default 200
            is_safe = scan_srtm(land_point, m_hwp_radius, 10, 10);

            return is_safe;
        }

        // Scan SRTM Data
        private bool scan_srtm(PointLatLngAlt lp, double scan_radius, double scan_step, double scan_angle_step)
        {
            double min_alt = lp.Alt;
            double max_alt = lp.Alt;

            double scan_upper_value = (scan_radius * 2);

            for (int angle = 0; angle < 180; angle += (int)scan_angle_step)
            {
                PointLatLngAlt l_point = lp.newpos(angle + 180, scan_radius); // lowest point

                //get SRTM data between l_point and u_point with step = scan_step
                for(double step = 0; step <= scan_upper_value; step +=scan_step)
                {
                    PointLatLngAlt scan_point = l_point.newpos(angle, step);
                    double srtmAltitude = srtm.getAltitude(scan_point.Lat, scan_point.Lng).alt;

                    if (srtmAltitude > max_alt) max_alt = srtmAltitude;
                    if (srtmAltitude < min_alt) min_alt = srtmAltitude;
                }
            }
            // check if the min and max altitude are safe for landing
            if (max_alt - min_alt < m_safe_altitude_delta) return true; else return false;
        }

        

        #endregion

        
        private void StoreOriginalMission(MyDataGridView Commands)
        {
            defined_mission.Clear();

            foreach (DataGridViewRow Command in Commands.Rows)
            {
                int cmd_id = get_command_id(Command.Cells[0].Value.ToString());
                double p1 = double.Parse(Command.Cells[1].Value.ToString());
                double p2 = double.Parse(Command.Cells[2].Value.ToString());
                double p3 = double.Parse(Command.Cells[3].Value.ToString());
                double p4 = double.Parse(Command.Cells[4].Value.ToString());

                double x = double.Parse(Command.Cells[5].Value.ToString()); //
                double y = double.Parse(Command.Cells[6].Value.ToString()); //
                double z = double.Parse(Command.Cells[7].Value.ToString()); //

                defined_mission.Add(new MissionItem(cmd_id, p1, p2, p3, p4, x, y, z));
            }
        }


        /// <summary>
        /// Modifies the mission according the rules of HWP
        /// </summary>
        /// <returns>True or False if the mission was modified</returns>
        private void ModifyLandingProcedure()
        {
            modified_mission.Clear();

            // just copy user defined mission to modified mission list
            for (int i=0; i< defined_mission.Count(); i++)
            {
                if(i == m_land_id)
                {
                    //insert LOITER_TO_ALTITUDE
                    PointLatLngAlt LTA = get_loiter_coords(defined_mission[i - 1].getCoords(), m_land_point);
                    PointLatLngAlt LWP = get_last_nav_coords(defined_mission[i - 1].getCoords(), m_land_point);

                    modified_mission.Add(new MissionItem((int)MAVLink.MAV_CMD.LOITER_TO_ALT, 0, m_loiter_radius, 0, m_exit_tangent, LTA.Lat, LTA.Lng, LTA.Alt));

                    //insert last nav waypoint
                    modified_mission.Add(new MissionItem((int)MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, LWP.Lat, LWP.Lng, m_land_point.Alt));
                }
                modified_mission.Add(defined_mission[i]);
            }
            if(m_hwp_enabled)modified_mission.Add(new MissionItem((int)MAVLink.MAV_CMD.MAV_CMD_DO_DISABLE_HWP, 0, 0, 0, 0, 0, 0, 0));// disable HWP Points
        }

        private void GenerateSafeLandingPoints()
        {
            // check if the mission already contains the addition points
            bool isLTA = false;
            bool isWP = false;
            if (defined_mission[m_land_id - 1].getCommand() == (int)MAVLink.MAV_CMD.WAYPOINT) { isWP = true; }
            if (defined_mission[m_land_id - 2].getCommand() == (int)MAVLink.MAV_CMD.LOITER_TO_ALT) { isLTA = true; }

            if(isWP && isLTA)
            {
                //mission is already modified, remove this two points
                defined_mission.RemoveAt(m_land_id - 1);
                defined_mission.RemoveAt(m_land_id - 2);

                if(defined_mission[defined_mission.Count-1].getCommand() == (int)MAVLink.MAV_CMD.MAV_CMD_DO_DISABLE_HWP)
                {
                    defined_mission.RemoveAt(defined_mission.Count - 1);
                }
            }
            // refresh info
            get_takeoff_land_ids(out m_takeoff_id, out m_land_id);
            ModifyLandingProcedure();
        }



        #region Public Methods

        /// <summary>
        /// Checks the mission before uploading
        /// </summary>
        public MissionCheckerResult CheckMission(MyDataGridView Commands)
        {
            StoreOriginalMission(Commands);

            #region Takeoff and Landing
            get_takeoff_land_ids(out m_takeoff_id, out m_land_id);
            if (m_takeoff_id < 0) return MissionCheckerResult.NO_TAKEOFF_POINT;
            if (m_land_id < 0) return MissionCheckerResult.NO_LAND_POINT;
            #endregion

            #region collision detection using SRTM


            #endregion
            
            #region Landing point testing, must be always the last check
            m_land_point = defined_mission[m_land_id].getCoords();
            if (m_land_point == null) return MissionCheckerResult.ERROR;

            if(m_hwp_enabled)
            {
                // if HWP Enabled and Landing is Safe according to SRTM, we don't modify the mission.
                bool is_landing_safe = check_landing_obstacles(m_land_point);
                if(!is_landing_safe)
                {
                    GenerateSafeLandingPoints();
                    return MissionCheckerResult.LANDING_UNSAFE;
                }
            }
            else
            {
                // if HWP is Disabled, we always modify the mission by adding a LTA+WP
                GenerateSafeLandingPoints();
                return MissionCheckerResult.LANDING_UNSAFE;
            }
            #endregion

            return MissionCheckerResult.OK;
        }
        #endregion

    }
}
