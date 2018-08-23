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
            ALTITUDE_UNSAFE,                // if elevation is unsafe
            GROUND_COLLISION_DETECTED,      // if ground collision could happen
            LANDING_CROSING_UNSAFE_AREA,    // if last mission waypoint is crossing the forbidden area
            WRONG_MISSION_SEQUENCE,         // if takeoff declared after landing
            DISTANCE_UNSAFE,                // distance between last mission wp and landing point is unsafe
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
            double bearing = land_point.GetBearing(wp_point);
            return land_point.newpos(bearing, m_hwp_radius);
        }

        private PointLatLngAlt get_last_nav_coords(PointLatLngAlt lta_point, PointLatLngAlt land_point)
        {
            double bearing = land_point.GetBearing(lta_point);
            return land_point.newpos(bearing, m_hwp_radius * 0.4f);
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
                if (defined_mission[i].getCommand() == (ushort)MAVLink.MAV_CMD.LAND) return i;
            }
            return -1;
        }
        #endregion

        #region Public Methods

        // Method stores the original Mission before checking
        public void doStoreOriginalMission(MyDataGridView Commands)
        {
            m_takeoff_id = -1;
            m_land_id = -1;

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

                // check also takeoff and landing points:
                if (cmd_id == (int)MAVLink.MAV_CMD.TAKEOFF) m_takeoff_id = defined_mission.Count();
                if (cmd_id == (int)MAVLink.MAV_CMD.LAND) m_land_id = defined_mission.Count();

                defined_mission.Add(new MissionItem(cmd_id, p1, p2, p3, p4, x, y, z));

                //actually we ignore all after landing point
                if (cmd_id == (int)MAVLink.MAV_CMD.LAND) break;
            }
        }

        // Method checks if current mission sequence is correct
        public MissionCheckerResult doCheckMissionSequence()
        {
            if (m_takeoff_id < 0) return MissionCheckerResult.NO_TAKEOFF_POINT;
            
            // if landing point is defined and landing is before takeoff:
            if (m_land_id > 0 && m_takeoff_id > m_land_id) return MissionCheckerResult.WRONG_MISSION_SEQUENCE;

            // if landing point is not defined, use takeoff point:
            if (m_land_id < 0) { m_land_id = m_takeoff_id; }

            double unsafeAngleOffset = defined_mission[m_land_id].P3;
            if (unsafeAngleOffset > 180) { DO_DISABLE_HWP = true; } // Set the flag to true, that means that after all tests mission will be modified

            //Otherwise return OK
            return MissionCheckerResult.OK;
        }

        // Method checks if the landing direction not crosses the forbiden area near landing
        public MissionCheckerResult doCheckLandingDirection()
        {
            int last_wp = getLastMissionWP();


            // check direction between last mission waypoint and landing(takeoff) point including forbidden area sector.
            if (last_wp > 0 && m_land_id >= 0)
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

        // Method checks the altitude using SRTM, but very slow... todo parallel for()?
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


        public int getDoDisableHWPCommandID()
        {
            for (int i = defined_mission.Count() - 1; i > 0; i--)
            {
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.MAV_CMD_DO_DISABLE_HWP) return i;
            }
            return -1;
        }

        public void doRestoreModifiedMission()
        {
            int landingPointIndex = -1;
            landingPointIndex = getLandWP();
            if (landingPointIndex < 0) return;

            //find and delete DO_DISABLE_HWP command
            for (int i = defined_mission.Count() - 1; i > landingPointIndex; i--)
            {
                if (defined_mission[i].getCommand() == (int)MAVLink.MAV_CMD.MAV_CMD_DO_DISABLE_HWP) defined_mission.RemoveAt(i);
            }

            //check for landing sequence
            if(defined_mission[landingPointIndex-1].getCommand() == (int)MAVLink.MAV_CMD.WAYPOINT && defined_mission[landingPointIndex - 2].getCommand() == (int)MAVLink.MAV_CMD.LOITER_TO_ALT)
            {
                defined_mission.RemoveRange(landingPointIndex - 2, 2);
            }
            
            return;
        }

        // Method modifies the defined mission according to disable HWP feature.
        public MissionCheckerResult doDisableHWP()
        {
            doRestoreModifiedMission();

            int LAST_MWP = getLastMissionWP();
            int LAND_CMD_ID = getLandWP();

            if (LAND_CMD_ID < 0) { LAND_CMD_ID = m_takeoff_id; } // use takeoff point

            bool distUnsafe = false;
            //if distance between last mwp and landing point is less than hwp radius
            double distance = defined_mission[LAND_CMD_ID].getCoords().GetDistance(defined_mission[LAST_MWP].getCoords());
            if (distance <= m_hwp_radius) distUnsafe = true; 

            //modify this additional points
            m_land_point = defined_mission[LAND_CMD_ID].getCoords();
            
            //insert LOITER_TO_ALTITUDE
            PointLatLngAlt LTA = distUnsafe ? defined_mission[LAST_MWP].getCoords() : get_loiter_coords(defined_mission[LAST_MWP].getCoords(), m_land_point);
            PointLatLngAlt LWP = get_last_nav_coords(LTA, m_land_point);

            // insert WPs before land command
            defined_mission.Insert(LAND_CMD_ID, new MissionItem((int)MAVLink.MAV_CMD.WAYPOINT, 0, 0, 0, 0, LWP.Lat, LWP.Lng, m_land_point.Alt));
            defined_mission.Insert(LAND_CMD_ID, new MissionItem((int)MAVLink.MAV_CMD.LOITER_TO_ALT, 0, m_hwp_lradius, 0, 1, LTA.Lat, LTA.Lng, m_land_point.Alt));
            defined_mission.Add(new MissionItem((int)MAVLink.MAV_CMD.MAV_CMD_DO_DISABLE_HWP, 0, 0, 0, 0, 0, 0, 0));

            // if LTA radius and LWP are crossing, return false.
            //if (LTA.GetDistance(LWP) < m_hwp_lradius + m_hwp_wpradius) return MissionCheckerResult.DISTANCE_UNSAFE;
            return MissionCheckerResult.OK;
        }

        //check only waypoints
        public MissionCheckerResult doCheckElevation()
        {
            for (int a = 0; a < defined_mission.Count - 1; a++)
            {
                // lets find a pair of waypoints to check the altitude
                PointLatLngAlt p1;
                if (defined_mission[a].getCommand() != (ushort)MAVLink.MAV_CMD.WAYPOINT) continue; else p1 = defined_mission[a].getCoords();

                //ok first wp is found, find next
                PointLatLngAlt p2 = null;
                for (int i = a + 1; i < defined_mission.Count; i++)
                {
                    if (defined_mission[i].getCommand() != (ushort)MAVLink.MAV_CMD.WAYPOINT) continue; else { p2 = defined_mission[i].getCoords(); a = i; break; }
                }

                if (p2 == null) { return MissionCheckerResult.OK; } //we finished check

                // calculate altitude difference in meters
                double alt_p1 = p1.Alt + srtm.getAltitude(p1.Lat, p1.Lng).alt;
                double alt_p2 = p2.Alt + srtm.getAltitude(p2.Lat, p2.Lng).alt;
                double alt_diff = Math.Abs(alt_p2 - alt_p1); // altitude difference in meters

                double distance = p1.GetDistance(p2); // distance in meters

                double pitchAngle = Math.Atan(alt_diff / distance) / (Math.PI / 180); // angle in deg

                if(pitchAngle > 20 || pitchAngle < -25)
                {
                    //this situation is dangerous! Alert!

                    string alertText = "Dangerous Altitude is detected on WP " + a + ". Pitch angle is critical." + pitchAngle.ToString();
                    MessageBox.Show(alertText, "Mission Checker warning", MessageBoxButtons.OK);

                    return MissionCheckerResult.GROUND_COLLISION_DETECTED;
                }
            }
            return MissionCheckerResult.OK;
        }
        

        #endregion



    }
}
