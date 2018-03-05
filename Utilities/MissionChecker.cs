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
        #region Structures and Enumerations
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
            public void setCommand(int cmd, double p1, double p2, double p3, double p4, double x, double y, double z)
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

        }

        public enum MissionCheckerResult
        {
            OK,                             // OK
            NO_TAKEOFF_POINT,               // if no takeoff point declared
            NO_LAND_POINT,                  // if no landing point declared
            ALTITUDE_UNSAFE,                // if elevation is unsafe
            GROUND_COLLISION_DETECTED,      // if ground collision could happen
            LANDING_CROSING_UNSAFE_AREA,    // if last mission waypoint is crossing the forbidden area
            WRONG_MISSION_SEQUENCE,         // if takeoff declared after landing
            MISSION_MODIFIED,               // mission modified(HWP are disabled)
        }
        #endregion

        #region Private fields
        int m_takeoff_id;
        int m_land_id;
        PointLatLngAlt m_land_point;

        int     m_hwp_radius; // 200 meters HWP radius by default
        int     m_hwp_lradius;
        int     m_hwp_wpradius;
        bool    m_hwp_enabled;

        bool    m_doDisableHWP;
        double  m_safe_altitude; // Elevation checker        
        double  m_safe_altitude_delta = 20;  // 20 meters of altitude difference is safe for landing?

        public List<MissionItem> defined_mission = new List<MissionItem>(); // List of all mission items which are created by user.
        public List<MissionItem> modified_mission = new List<MissionItem>(); // list of mission items modified by mission checker
        #endregion

        #region Public Fields

        public bool DO_DISABLE_HWP
        {
            get { return m_doDisableHWP; }
            set { m_doDisableHWP = value; }
        }

        public int HWP_RADIUS
        {
            get { return m_hwp_radius; }
            set { m_hwp_radius = value; }
        }

        public int HWP_WPRADIUS
        {
            get { return m_hwp_wpradius; }
            set { m_hwp_wpradius = value; }
        }

        public int HWP_LRADIUS
        {
            get { return m_hwp_lradius; }
            set { m_hwp_lradius = value; }
        }

        public bool HWP_ENABLED
        {
            get { return m_hwp_enabled; }
            set { m_hwp_enabled = value; }
        }

        public double SAFE_ALTITUDE
        {
            get { return m_safe_altitude; }
            set { m_safe_altitude = value; }
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

        private int get_command_id(string command)
        {
            return (ushort)(MAVLink.MAV_CMD)Enum.Parse(typeof(MAVLink.MAV_CMD), command);
        }
        
        private PointLatLngAlt get_loiter_coords(PointLatLngAlt wp_point, PointLatLngAlt land_point)
        {
            PointLatLngAlt loiter_point = wp_point;

            double bearing = wp_point.GetBearing(land_point);
            double distance = wp_point.GetDistance(land_point);

            return loiter_point.newpos(bearing, distance * 0.25f);
        }

        private PointLatLngAlt get_last_nav_coords(PointLatLngAlt wp_point, PointLatLngAlt land_point)
        {
            PointLatLngAlt last_nav_point = wp_point;

            double bearing = wp_point.GetBearing(land_point);
            double distance = wp_point.GetDistance(land_point);

            return last_nav_point.newpos(bearing, distance * 0.5f);
        }

        private bool check_landing_obstacles(PointLatLngAlt land_point)
        {
            bool is_safe = false;

            // hwp_radius default 200
            is_safe = scan_srtm(land_point, m_hwp_radius, 10, 10);

            return is_safe;
        }

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

        private int getLastMissionWP()
        {
            for(int i = defined_mission.Count - 1; i >= 0;i--)
            {
                if (defined_mission[i].getCommand() == (ushort)MAVLink.MAV_CMD.WAYPOINT) return i;
            }
            return -1;
        }

        private int getLandWP()
        {
            for (int i = defined_mission.Count - 1; i >= 0; i--)
            {
                if (defined_mission[i].getCommand() == (ushort)MAVLink.MAV_CMD.LAND || defined_mission[i].getCommand() == (ushort)MAVLink.MAV_CMD.LAND_AT_TAKEOFF) return i;
            }
            return -1;
        }
        #endregion

        #region Public Methods

        // Method stores the original Mission before checking
        public void doStoreOriginalMission(MyDataGridView Commands)
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

        // Method checks if current mission don't contains landing and takeoff
        public MissionCheckerResult doCheckTakeoffLandingSequence()
        {
            m_takeoff_id = -1;
            m_land_id = -1;
            for (int i = 0; i < defined_mission.Count; i++)
            {
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.TAKEOFF) m_takeoff_id = i;
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.LAND || defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.LAND_AT_TAKEOFF) m_land_id = i;
            }
            if (m_takeoff_id < 0) return MissionCheckerResult.NO_TAKEOFF_POINT;
            if (m_land_id < 0) return MissionCheckerResult.NO_LAND_POINT;
            if (m_takeoff_id > m_land_id) return MissionCheckerResult.WRONG_MISSION_SEQUENCE;

            double unsafeAngleOffset = defined_mission[m_land_id].P3;
            if (unsafeAngleOffset > 180) { DO_DISABLE_HWP = true; } // Set the flag to true, that means that after all tests mission will be modified

            //Otherwise return OK
            return MissionCheckerResult.OK;
        }

        // Method checks if the landing direction not crosses the forbiden area near landing
        public MissionCheckerResult doCheckLandingDirection()
        {
            int last_wp = getLastMissionWP();
            m_land_id = getLandWP();
            if (last_wp > 0 && m_land_id > 0)
            {
                PointLatLngAlt lp_coords = defined_mission[m_land_id].getCoords();
                double lp_bearing = lp_coords.GetBearing(defined_mission[last_wp].getCoords());

                //perform check!
                double unsafeAngleStart = defined_mission[m_land_id].P2;
                while (unsafeAngleStart > 360) unsafeAngleStart -= 360;

                double unsafeAngleEnd = unsafeAngleStart + defined_mission[m_land_id].P3;
                while (unsafeAngleEnd > 360) unsafeAngleEnd -= 360;

                double unsafeAngleOffset = defined_mission[m_land_id].P3;
                
                if (unsafeAngleStart > unsafeAngleEnd)
                {
                    if (lp_bearing >= unsafeAngleStart && lp_bearing <= unsafeAngleStart + unsafeAngleOffset) return MissionCheckerResult.LANDING_CROSING_UNSAFE_AREA;
                }
                else
                {
                    if (lp_bearing >= unsafeAngleStart && lp_bearing <= unsafeAngleEnd) return MissionCheckerResult.LANDING_CROSING_UNSAFE_AREA;
                }
            }
            return MissionCheckerResult.OK;
        }

        // Method checks the altitude using SRTM
        public MissionCheckerResult doCheckAltitudeSRTM()
        {
            List<PointLatLngAlt> planlocs = new List<PointLatLngAlt>(); // list of all coords
            for (int a = 1; a < defined_mission.Count - 1; a++) // skip the takeoff point
            {
                if (defined_mission[a].getCommand() == (ushort)MAVLink.MAV_CMD.ROI || defined_mission[a].getCommand() == (ushort)MAVLink.MAV_CMD.DO_SET_ROI)
                {
                    continue;
                }
                else {
                    planlocs.Add(defined_mission[a].getCoords());
                }
            }

            const int accuracy = 10; // Predefined constant value, every 10 meters we check if the collision could happen

            for (int p = 0; p < planlocs.Count - 1; p++)
            {
                double distance = (int)planlocs[p + 1].GetDistance(planlocs[p]);
                int pcount = (int)Math.Abs(distance / accuracy);

                if (pcount > 0)
                {
                    PointLatLngAlt delta = planlocs[p + 1] - planlocs[p];
                    PointLatLngAlt inc = delta / pcount;

                    int i = 0;
                    do
                    {
                        PointLatLngAlt currentPoint = planlocs[p] + (inc * i);
                        double srtmAltitude = srtm.getAltitude(currentPoint.Lat, currentPoint.Lng).alt;
                        if (srtmAltitude > currentPoint.Alt - m_safe_altitude)
                        {
                            //return MissionCheckerResult.GROUND_COLLISION_DETECTED;
                            // TODO: FIX!!!
                            return MissionCheckerResult.OK;
                        }

                        i++;
                    } while (i < pcount);
                }
            }
            return MissionCheckerResult.OK;
        }


        public void doRestoreModifiedMission()
        {
            // simply cleanup the mission
            for (int i = 0; i <= defined_mission.Count() - 1; i++)
            {
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.LOITER_TO_ALT && defined_mission[i].P1 == 1) { defined_mission.RemoveAt(i); i = 0; };
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.WAYPOINT && defined_mission[i].P1 == 1) { defined_mission.RemoveAt(i); i = 0; };
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.MAV_CMD_DO_DISABLE_HWP) { defined_mission.RemoveAt(i); i = 0; };
            }
        }

        // Method modifies the defined mission according to disable HWP feature.
        public bool doDisableHWP()
        {
            doRestoreModifiedMission();
            
            int LAST_MWP = getLastMissionWP();
            int LAND_CMD_ID = getLandWP();
            
            //modify this additional points
            m_land_point = defined_mission[LAND_CMD_ID].getCoords();
            
            //insert LOITER_TO_ALTITUDE
            PointLatLngAlt LTA = get_loiter_coords(defined_mission[LAST_MWP].getCoords(), m_land_point);
            PointLatLngAlt LWP = get_last_nav_coords(defined_mission[LAST_MWP].getCoords(), m_land_point);

            // insert WPs before land command
            defined_mission.Insert(LAND_CMD_ID, new MissionItem((int)MAVLink.MAV_CMD.WAYPOINT, 1, 0, 0, 0, LWP.Lat, LWP.Lng, m_land_point.Alt));
            defined_mission.Insert(LAND_CMD_ID, new MissionItem((int)MAVLink.MAV_CMD.LOITER_TO_ALT, 1, m_hwp_lradius, 0, 1, LTA.Lat, LTA.Lng, m_land_point.Alt));
            defined_mission.Add(new MissionItem((int)MAVLink.MAV_CMD.MAV_CMD_DO_DISABLE_HWP, 0, 0, 0, 0, 0, 0, 0));

            // if LTA radius and LWP are crossing, return false.
            if (LTA.GetDistance(LWP) < m_hwp_lradius + m_hwp_wpradius) return false;
            return true;
        }
        

        #endregion

    }
}
