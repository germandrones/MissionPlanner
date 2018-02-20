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

        double m_hwp_radius;
        bool   m_hwp_enabled;

        double m_loiter_radius;
        double m_exit_tangent;

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

            //TODO: checking here...
            //is_safe = true;
            //...

            return is_safe;
        }

        #endregion






        #region Public Methods
        public void StoreOriginalMission(MyDataGridView Commands)
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
        public bool ModifyLandingProcedure()
        {
            //if(defined_mission[defined_mission.Count])
            if (defined_mission.Count >= 4)
            {
                List<MissionItem> temp = defined_mission.GetRange(defined_mission.Count - 4, 4);
                int mod = 0; // list of modifications in last 4 nav points
                foreach (var t in temp)
                {
                    if (t.getCommand() == (int)MAVLink.MAV_CMD.DISABLE_HWP) mod++;
                    if (t.getCommand() == (int)MAVLink.MAV_CMD.LOITER_TO_ALT) mod++;
                    if (t.getCommand() == (int)MAVLink.MAV_CMD.LAND || t.getCommand() == (int)MAVLink.MAV_CMD.LAND_AT_TAKEOFF) mod++;
                }
                if (mod >= 2) { return false; }// mission is already modified
            }
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
            if(m_hwp_enabled)modified_mission.Add(new MissionItem((int)MAVLink.MAV_CMD.DISABLE_HWP, 0, 0, 0, 0, 0, 0, 0));// disable HWP Points

            return true;
        }


        public MissionCheckerResult CheckMission(MyDataGridView Commands)
        {
            StoreOriginalMission(Commands);

            #region Takeoff and Landing
            get_takeoff_land_ids(out m_takeoff_id, out m_land_id);
            if (m_takeoff_id < 0) return MissionCheckerResult.NO_TAKEOFF_POINT;
            if (m_land_id < 0) return MissionCheckerResult.NO_LAND_POINT;
            #endregion

            #region Landing point testing
                m_land_point = defined_mission[m_land_id].getCoords();
                if (m_land_point == null) return MissionCheckerResult.ERROR;

                if (m_hwp_enabled) // check the landing area if any obstacles are present
                {
                    if (!check_landing_obstacles(m_land_point))
                    {
                        return MissionCheckerResult.LANDING_UNSAFE;
                    }
                }
                else
                {
                    return MissionCheckerResult.LANDING_UNSAFE;
                }
                #endregion

            return MissionCheckerResult.OK;
        }
        #endregion

    }
}
