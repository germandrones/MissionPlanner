using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MissionPlanner.ColibriControl
{
    public class ColibriMavlink
    {
        public float pitch_pos = 0, roll_pos = 0;
        public bool isHoldMode = false;

        #region Initial Definitions and constants
        /* Colibri Camera Mode Enum */
        public enum CameraMode
        {
            e_Rate = 0,
            e_PointToCordinate = 1,
            e_HoldCordinate = 2,
            e_Pilot = 3,
            e_Stow = 4,
            e_RateDriftOff = 6,
            e_DynamicDrift = 7,
            e_Park = 8,
            e_GyroCalibration = 10,
            e_Position = 12,
            e_GRR = 17
        };

        /* Constants */
        private const ushort X25_INIT_CRC = 0xFFFF;
        private const byte MAVLINK_START_OF_FRAME = 0xFE;
        private const int MAVLINK_HEADER_LENGTH = 8;
        private const byte MAVLINK_DRONE_ARMED_MASK = 0x80;
        private const byte MAVLINK_V2_EXT_MSGID = 0xF8;
        private const byte MAVLINK_V2_EXT_MSG_LEN = 0x3C;
        private const byte MAVLINK_V2_EXT_MSG_CRC = 8;
        private const byte MAVLINK_ATTITUDE_MSG_CRC = 39;
        private const byte MAVLINK_GBL_POS_INT_CRC = 104;
        private const byte MAVLINK_GPS_RAW_INT_CRC = 24;
        private const byte MAVLINK_SYS_STAT_CRC = 124;
        private const byte MAVLINK_HEART_BEAT_CRC = 50;
        private const byte MAVLINK_SYS_TIME_CRC = 137;

        /* Mavlink Packets Resources */
        /* Mavlink V2 Ext Packet - Camera Control */
        private Mutex mavlink_v2_ext_packet_mutex = new Mutex();
        private byte[] mavlink_v2_ext_packet =
        {
            0xFE, 0x3C, 0x00, 0xFF, 0xBE, 0xF8,                                      /* Mavlink Header */
            0xB0, 0x3B, 0x77, 0x04, 0x21, 0x00, 0x00, 0x00, 0x00, 0x0F,              /* Colirbi Packet bytes 0 - 9 */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x80, 0x80, 0x00, 0x12,              /* Colirbi Packet bytes 10 - 19 */
            0x00, 0x00, 0x00, 0x00,                                                  /* V2 Ext Type - Not Used By Transmitter */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  /* PTC Lat/Lon/Alt */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  /* Los/Gnd Cross - Not Used By Transmitter */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                          /* Position Pitch/Roll */
            0x00, 0x00, 0x00, 0x00,                                                  /* Los Z - Not Used By Transmitter */
            0x00, 0x00                                                               /* Mavlink Message CRC */
        };

        /* Mavlink Attitude Packet -    
         * The following values are used by the TRIP: Roll, Pitch, Yaw */
        private byte[] mavlink_attitude_packet =
        {
            0xFE, 0x1C, 0x00, 0xFF, 0xBE, 0x1E,                                      /* Mavlink Header */
            0x00, 0x00, 0x00, 0x00,                                                  /* Time Since Boot */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  /* Roll, Pitch, Yaw */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  /* Roll Speed, Pitch Speed, Yaw Speed */          
            0x00, 0x00                                                               /* Mavlink Message CRC */
        };

        /* Mavlink Global Position Packet - 
         * The following values are used by the TRIP: Latitude, Longitude, Altitude, Relative Altitude, Vx, Vy, Vz */
        private byte[] mavlink_global_pos_int_packet =
        {
            0xFE, 0x1C, 0x00, 0xFF, 0xBE, 0x21,                                      /* Mavlink Header */
            0x00, 0x00, 0x00, 0x00,                                                  /* Time Since Boot */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                          /* Latitude, Longitude */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                          /* Altitude, Relative Altitude */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                          /* Vx, Vy, Vz, Heading */   
            0x00, 0x00                                                               /* Mavlink Message CRC */
        };

        /* Mavlink GPS Raw int Packet - 
         * The following values are used by the TRIP: Satellites Visable */
        private byte[] mavlink_gps_raw_int_packet =
        {
            0xFE, 0x1E, 0x00, 0xFF, 0xBE, 0x18,                                      /* Mavlink Header */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                          /* TimeStamp */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  /* Latitude, Longitude, Altitude */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                          /* Eph, Epv, Vel, Cog */
            0x00,                                                                    /* Fix Type */   
            0x00,                                                                    /* Satellites Visable */
            0x00, 0x00                                                               /* Mavlink Message CRC */
        };

        /* Mavlink System Status Packet - 
         * The following values are used by the TRIP: Battery Voltage */
        private byte[] mavlink_sys_status_packet =
        {
            0xFE, 0x1F, 0x00, 0xFF, 0xBE, 0x01,                                      /* Mavlink Header */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  /* OCS Present, OCS Enabled, OCS Health */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                          /* Load, Battery Voltage, Battery Current, Drop Rate */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,              /* Error Counters */
            0x00,                                                                    /* Battery Remaining */   
            0x00, 0x00                                                               /* Mavlink Message CRC */
        };

        /* Mavlink Heart Beat Packet - 
        * The following values are used by the TRIP: Base Mode */
        private byte[] mavlink_heart_beat_packet =
        {
            0xFE, 0x09, 0x00, 0xFF, 0xBE, 0x00,                                      /* Mavlink Header */
            0x00, 0x00, 0x00, 0x00,                                                  /* Custom Mode */
            0x00, 0x00,                                                              /* Mav Type, AutoPilot Type */ 
            0x00, 0x00, 0x00,                                                        /* Base Mode, System Status Flag, Mavlink Version */
            0x00, 0x00                                                               /* Mavlink Message CRC */
        };

        /* Mavlink System Time Packet - 
        * The following values are used by the TRIP: TimeStamp */
        private byte[] mavlink_system_time_packet =
        {
            0xFE, 0x0C, 0x00, 0xFF, 0xBE, 0x02,                                      /* Mavlink Header */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                          /* TimeStamp */
            0x00, 0x00, 0x00, 0x00,                                                  /* Time Since Boot */
            0x00, 0x00                                                               /* Mavlink Message CRC */
        };
        /* Mavlink Rx Resources */
        private byte[] mavlink_rx_packet = new byte[MAVLINK_V2_EXT_MSG_LEN + MAVLINK_HEADER_LENGTH];
        private ushort mavlink_rx_index = 0;
        private ushort mavlink_rx_msg_id = 0;
        private ushort mavlink_rx_payload_len = 0;
        #endregion

        /****************************************************************************************************************************
        *                                                 CAMERA MOVEMENT FUNCTIONS
        ****************************************************************************************************************************/
        /****************************************************************************************************************************
        *                                                      MavlinkMoveCamUp()
        *                                                      
        * Description : Moves the camera upwards
        *               
        * Arguments   : none
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkMoveCamUp()
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message pitch value */
            mavlink_v2_ext_packet[20] &= 0xCF;
            mavlink_v2_ext_packet[20] = 0x30;
            mavlink_v2_ext_packet[21] = 0xFF;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkMoveCamDown()
        *                                                      
        * Description : Moves the camera downwards
        *               
        * Arguments   : none
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkMoveCamDown()
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message pitch value */
            mavlink_v2_ext_packet[20] &= 0xCF;
            mavlink_v2_ext_packet[21] = 0x00;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkMoveCamLeft()
        *                                                      
        * Description : Moves the camera leftwards
        *               
        * Arguments   : none
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkMoveCamLeft()
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message roll value */
            mavlink_v2_ext_packet[20] &= 0xF3;
            mavlink_v2_ext_packet[20] = 0x0C;
            mavlink_v2_ext_packet[22] = 0xFF;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkMoveCamRight()
        *                                                      
        * Description : Moves the camera rightwards
        *               
        * Arguments   : none
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkMoveCamRight()
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message roll value */
            mavlink_v2_ext_packet[20] &= 0xF3;
            mavlink_v2_ext_packet[22] = 0x00;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkCamStop()
        *                                                      
        * Description : Stops camera movement
        *               
        * Arguments   : none
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkCamStop()
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message movement values */
            mavlink_v2_ext_packet[20] &= 0xC3;
            mavlink_v2_ext_packet[21] = 0x80;
            mavlink_v2_ext_packet[22] = 0x80;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkCamZoomIn()
        *                                                      
        * Description : Camera Zoom In
        *               
        * Arguments   : none
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkCamZoomIn()
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message zoom value */
            mavlink_v2_ext_packet[20] |= 0x80;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkCamZoomOut()
        *                                                      
        * Description : Camera Zoom Out
        *               
        * Arguments   : none
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkCamZoomOut()
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message zoom value */
            mavlink_v2_ext_packet[20] |= 0x40;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkCamZoomStop()
        *                                                      
        * Description : Stop zooming
        *               
        * Arguments   : none
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkCamZoomStop()
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message zoom value */
            mavlink_v2_ext_packet[20] &= 0x3F;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkUpdateCameraMode()
        *                                                      
        * Description : update the colibri camera mode
        *               
        * Arguments   : CameraMode mode - new mode for the camera
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkUpdateCameraMode(CameraMode mode)
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message zoom value */
            mavlink_v2_ext_packet[9] &= 0xE0;
            mavlink_v2_ext_packet[9] |= (byte)mode;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkUpdatePosMode()
        *                                                      
        * Description : update the pitch & roll of the position camera mode
        *               
        * Arguments   : float pitch - pitch value
        *               float roll  - roll value
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkUpdatePosMode(float pitch, float roll)
        {
            byte[] roll_byteArray = BitConverter.GetBytes(pitch);
            byte[] pitch_byteArray = BitConverter.GetBytes(roll);

            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            roll_byteArray.CopyTo(mavlink_v2_ext_packet, 54);
            pitch_byteArray.CopyTo(mavlink_v2_ext_packet, 58);

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkUpdatePtcMode()
        *                                                      
        * Description : update the lat,lon & alt of the PTC camera mode
        *               
        * Arguments   : float lat - latitude value
        *               float lon - longitude value
        *               int alt   - altitude value    
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkUpdatePtcMode(float lat, float lon, int alt)
        {
            /* convert the mavlink format */
            int lat_int = (int)(lat * 10000000.0);
            int lon_int = (int)(lon * 10000000.0);
            int alt_int = (int)(alt * 1000.0);

            byte[] lat_byteArray = BitConverter.GetBytes(lat_int);
            byte[] lon_byteArray = BitConverter.GetBytes(lon_int);
            byte[] alt_byteArray = BitConverter.GetBytes(alt_int);

            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            lat_byteArray.CopyTo(mavlink_v2_ext_packet, 30);
            lon_byteArray.CopyTo(mavlink_v2_ext_packet, 34);
            alt_byteArray.CopyTo(mavlink_v2_ext_packet, 38);

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkUpdateRecEn()
        *                                                      
        * Description : Enable/Disable Video Recording
        *               
        * Arguments   : bool en - enable/disable
        *        
        * Returns     : none
        *
        ****************************************************************************************************************************/
        public void MavlinkUpdateRecEn(bool en)
        {
            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* update the colibri message zoom value */
            if (en)
                mavlink_v2_ext_packet[12] |= 0x10;
            else
                mavlink_v2_ext_packet[12] &= 0xEF;

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                 MAVLINK / COLIBRI PROTOCOL FUNCTIONS
        ****************************************************************************************************************************/
        /****************************************************************************************************************************
        *                                                      MavlinkGetV2ExtPacket()
        *                                                      
        * Description : Returns the Mavlink V2 Ext Packet that is ready for transmission, ( after CRC calculation )
        *               
        * Arguments   : ref byte[] tx_packet - will be filled with a packet that is ready for transmission
        *        
        * Returns     : Mavlink V2 Ext Packet
        *
        ****************************************************************************************************************************/
        public void MavlinkGetV2ExtPacket(ref byte[] tx_packet)
        {
            ushort mavlink_crc;

            /* lock the v2 ext packet */
            mavlink_v2_ext_packet_mutex.WaitOne();

            /* calculate the colibri message checksum */
            ColibCalcCSForMavMsg(mavlink_v2_ext_packet);

            /* calculate the mavlink message CRC */
            mavlink_crc = MavlinkCrcCalculate(mavlink_v2_ext_packet, mavlink_v2_ext_packet.Length - 2); /* All message bytes execpt the CRC bytes */
            mavlink_crc = MavlinkCrcAccumulate(MAVLINK_V2_EXT_MSG_CRC, mavlink_crc);

            /* put the calculated CRC in the mavlink message */
            mavlink_v2_ext_packet[mavlink_v2_ext_packet.Length - 2] = (byte)mavlink_crc;
            mavlink_v2_ext_packet[mavlink_v2_ext_packet.Length - 1] = (byte)(mavlink_crc >> 8);

            /* copy the ready packet to the tx_packet */
            tx_packet = mavlink_v2_ext_packet.ToArray();

            /* release the v2 ext packet */
            mavlink_v2_ext_packet_mutex.ReleaseMutex();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkGetRxPacket()
        *                                                      
        * Description : Returns the Mavlink V2 Ext Last Received Message
        *               
        * Arguments   : none
        *        
        * Returns     : Mavlink V2 Ext Rx Packet
        *
        ****************************************************************************************************************************/
        public byte[] MavlinkGetRxPacket()
        {
            return mavlink_rx_packet;
        }

        /****************************************************************************************************************************
        *                                                      MavlinkGetAttitudePacket()
        *                                                      
        * Description : Returns the Mavlink Attitude Packet that is ready for transmission, ( after CRC calculation )
        *               
        * Arguments   : ref byte[] tx_packet - will be filled with a packet that is ready for transmission
        *               float           roll - platform roll
        *               float           pitch - platform pitch
        *               float           yaw - platform yaw
        *        
        * Returns     : Mavlink Attitude Packet
        *
        ****************************************************************************************************************************/
        public void MavlinkGetAttitudePacket(ref byte[] tx_packet, float roll, float pitch, float yaw)
        {
            ushort mavlink_crc;

            byte[] roll_byteArray = BitConverter.GetBytes(roll);
            byte[] pitch_byteArray = BitConverter.GetBytes(pitch);
            byte[] yaw_byteArray = BitConverter.GetBytes(yaw);

            /* copy the values to the outgoing message */
            roll_byteArray.CopyTo(mavlink_attitude_packet, 10);
            pitch_byteArray.CopyTo(mavlink_attitude_packet, 14);
            yaw_byteArray.CopyTo(mavlink_attitude_packet, 18);

            /* calculate the mavlink message CRC */
            mavlink_crc = MavlinkCrcCalculate(mavlink_attitude_packet, mavlink_attitude_packet.Length - 2); /* All message bytes execpt the CRC bytes */
            mavlink_crc = MavlinkCrcAccumulate(MAVLINK_ATTITUDE_MSG_CRC, mavlink_crc);

            /* put the calculated CRC in the mavlink message */
            mavlink_attitude_packet[mavlink_attitude_packet.Length - 2] = (byte)mavlink_crc;
            mavlink_attitude_packet[mavlink_attitude_packet.Length - 1] = (byte)(mavlink_crc >> 8);

            /* copy the ready packet to the tx_packet */
            tx_packet = mavlink_attitude_packet.ToArray();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkGetGlobalPosIntPacket()
        *                                                      
        * Description : Returns the Mavlink Global Position Int Packet that is ready for transmission, ( after CRC calculation )
        *               
        * Arguments   : ref byte[] tx_packet - will be filled with a packet that is ready for transmission
        *               float            lat - platform latitude
        *               float            lon - platfrom longitude
        *               int alt          alt - platfrom altitude (above mean sea level)
        *               int          rel_alt - platfrom altitude (above ground level)
        *               float             vx - X ground speed
        *               float             vy - Y ground speed
        *               float             vz - Z ground speed
        *        
        * Returns     : Mavlink Global Pos Int Packet
        *
        ****************************************************************************************************************************/
        public void MavlinkGetGlobalPosIntPacket(ref byte[] tx_packet, float lat, float lon, int alt, int rel_alt, float vx, float vy, float vz)
        {
            ushort mavlink_crc;

            int lat_int = (int)(lat * 10000000.0);
            int lon_int = (int)(lon * 10000000.0);
            short vx_short = (short)(vx * 100.0);
            short vy_short = (short)(vy * 100.0);
            short vz_short = (short)(vz * 100.0);
            int alt_int = (int)(alt * 1000.0);
            int rel_alt_int = (int)(rel_alt * 1000.0);

            byte[] lat_byteArray = BitConverter.GetBytes(lat_int);
            byte[] lon_byteArray = BitConverter.GetBytes(lon_int);
            byte[] alt_byteArray = BitConverter.GetBytes(alt_int);
            byte[] rel_alt_byteArray = BitConverter.GetBytes(rel_alt_int);
            byte[] vx_byteArray = BitConverter.GetBytes(vx_short);
            byte[] vy_byteArray = BitConverter.GetBytes(vy_short);
            byte[] vz_byteArray = BitConverter.GetBytes(vz_short);

            /* copy the values the outgoing message */
            lat_byteArray.CopyTo(mavlink_global_pos_int_packet, 10);
            lon_byteArray.CopyTo(mavlink_global_pos_int_packet, 14);
            alt_byteArray.CopyTo(mavlink_global_pos_int_packet, 18);
            rel_alt_byteArray.CopyTo(mavlink_global_pos_int_packet, 22);
            vx_byteArray.CopyTo(mavlink_global_pos_int_packet, 26);
            vy_byteArray.CopyTo(mavlink_global_pos_int_packet, 28);
            vz_byteArray.CopyTo(mavlink_global_pos_int_packet, 30);

            /* calculate the mavlink message CRC */
            mavlink_crc = MavlinkCrcCalculate(mavlink_global_pos_int_packet, mavlink_global_pos_int_packet.Length - 2); /* All message bytes execpt the CRC bytes */
            mavlink_crc = MavlinkCrcAccumulate(MAVLINK_GBL_POS_INT_CRC, mavlink_crc);

            /* put the calculated CRC in the mavlink message */
            mavlink_global_pos_int_packet[mavlink_global_pos_int_packet.Length - 2] = (byte)mavlink_crc;
            mavlink_global_pos_int_packet[mavlink_global_pos_int_packet.Length - 1] = (byte)(mavlink_crc >> 8);

            /* copy the ready packet to the tx_packet */
            tx_packet = mavlink_global_pos_int_packet.ToArray();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkGetGPSRawIntPacket()
        *                                                      
        * Description : Returns the Mavlink GPS Raw Int Packet that is ready for transmission, ( after CRC calculation )
        *               
        * Arguments   : ref byte[] tx_packet - will be filled with a packet that is ready for transmission
        *               int        sat_count - number of visable satelites
        *        
        * Returns     : Mavlink GPS Raw Int Packet
        *
        ****************************************************************************************************************************/
        public void MavlinkGetGPSRawIntPacket(ref byte[] tx_packet, int sat_count)
        {
            ushort mavlink_crc;

            /* update the satelite count in the outgoing message */
            mavlink_gps_raw_int_packet[35] = (byte)sat_count;

            /* calculate the mavlink message CRC */
            mavlink_crc = MavlinkCrcCalculate(mavlink_gps_raw_int_packet, mavlink_gps_raw_int_packet.Length - 2); /* All message bytes execpt the CRC bytes */
            mavlink_crc = MavlinkCrcAccumulate(MAVLINK_GPS_RAW_INT_CRC, mavlink_crc);

            /* put the calculated CRC in the mavlink message */
            mavlink_gps_raw_int_packet[mavlink_gps_raw_int_packet.Length - 2] = (byte)mavlink_crc;
            mavlink_gps_raw_int_packet[mavlink_gps_raw_int_packet.Length - 1] = (byte)(mavlink_crc >> 8);

            /* copy the ready packet to the tx_packet */
            tx_packet = mavlink_gps_raw_int_packet.ToArray();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkGetSysStatusPacket()
        *                                                      
        * Description : Returns the Mavlink System Status Packet that is ready for transmission, ( after CRC calculation )
        *               
        * Arguments   : ref byte[] tx_packet - will be filled with a packet that is ready for transmission
        *               float   batt_voltage - battery voltage
        *        
        * Returns     : Mavlink System Status Packet
        *
        ****************************************************************************************************************************/
        public void MavlinkGetSysStatusPacket(ref byte[] tx_packet, float batt_voltage)
        {
            ushort mavlink_crc;

            short batt_voltage_short = (short)(batt_voltage * 1000.0);
            byte[] batt_voltage_byteArray = BitConverter.GetBytes(batt_voltage_short);

            /* update the battery voltage in the outgoing message */
            batt_voltage_byteArray.CopyTo(mavlink_sys_status_packet, 20);

            /* calculate the mavlink message CRC */
            mavlink_crc = MavlinkCrcCalculate(mavlink_sys_status_packet, mavlink_sys_status_packet.Length - 2); /* All message bytes execpt the CRC bytes */
            mavlink_crc = MavlinkCrcAccumulate(MAVLINK_SYS_STAT_CRC, mavlink_crc);

            /* put the calculated CRC in the mavlink message */
            mavlink_sys_status_packet[mavlink_sys_status_packet.Length - 2] = (byte)mavlink_crc;
            mavlink_sys_status_packet[mavlink_sys_status_packet.Length - 1] = (byte)(mavlink_crc >> 8);

            /* copy the ready packet to the tx_packet */
            tx_packet = mavlink_sys_status_packet.ToArray();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkGetHeartbeatPacket()
        *                                                      
        * Description : Returns the Mavlink Heartbeat Packet that is ready for transmission, ( after CRC calculation )
        *               
        * Arguments   : ref byte[] tx_packet - will be filled with a packet that is ready for transmission
        *               bool     drone_armed - is the platfrom armed ( motors are running, ready to takeoff )
        *        
        * Returns     : Mavlink Heart beat Packet
        *
        ****************************************************************************************************************************/
        public void MavlinkGetHeartbeatPacket(ref byte[] tx_packet, bool drone_armed)
        {
            ushort mavlink_crc;

            /* update the drone armed state */
            if (drone_armed)
                mavlink_heart_beat_packet[12] |= MAVLINK_DRONE_ARMED_MASK;
            else
                mavlink_heart_beat_packet[12] &= (MAVLINK_DRONE_ARMED_MASK ^ 0xFF);

            /* calculate the mavlink message CRC */
            mavlink_crc = MavlinkCrcCalculate(mavlink_heart_beat_packet, mavlink_heart_beat_packet.Length - 2); /* All message bytes execpt the CRC bytes */
            mavlink_crc = MavlinkCrcAccumulate(MAVLINK_HEART_BEAT_CRC, mavlink_crc);

            /* put the calculated CRC in the mavlink message */
            mavlink_heart_beat_packet[mavlink_heart_beat_packet.Length - 2] = (byte)mavlink_crc;
            mavlink_heart_beat_packet[mavlink_heart_beat_packet.Length - 1] = (byte)(mavlink_crc >> 8);

            /* copy the ready packet to the tx_packet */
            tx_packet = mavlink_heart_beat_packet.ToArray();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkGetSysTimePacket()
        *                                                      
        * Description : Returns the Mavlink System Time Packet that is ready for transmission, ( after CRC calculation )
        *               
        * Arguments   : ref byte[] tx_packet - will be filled with a packet that is ready for transmission
        *               int  epoch_timestamp - timestamp in epoch
        *        
        * Returns     : Mavlink System Time Packet
        *
        ****************************************************************************************************************************/
        public void MavlinkGetSysTimePacket(ref byte[] tx_packet, int epoch_timestamp)
        {
            ushort mavlink_crc;
            UInt64 epoch_timestamp_long = ((UInt64)epoch_timestamp * 1000000);
            byte[] epoch_byteArray = BitConverter.GetBytes(epoch_timestamp_long);

            /* copy the timestamp to the outgoing message */
            epoch_byteArray.CopyTo(mavlink_system_time_packet, 6);

            /* calculate the mavlink message CRC */
            mavlink_crc = MavlinkCrcCalculate(mavlink_system_time_packet, mavlink_system_time_packet.Length - 2); /* All message bytes execpt the CRC bytes */
            mavlink_crc = MavlinkCrcAccumulate(MAVLINK_SYS_TIME_CRC, mavlink_crc);

            /* put the calculated CRC in the mavlink message */
            mavlink_system_time_packet[mavlink_system_time_packet.Length - 2] = (byte)mavlink_crc;
            mavlink_system_time_packet[mavlink_system_time_packet.Length - 1] = (byte)(mavlink_crc >> 8);

            /* copy the ready packet to the tx_packet */
            tx_packet = mavlink_system_time_packet.ToArray();
        }

        /****************************************************************************************************************************
        *                                                      MavlinkRxParser()
        *                                                      
        * Description : Parses the incoming mavlink replys,
        *               
        * Arguments   : byte data_byte - new data byte
        *        
        * Returns     : true - CRC failed or Packet not ready 
        *               false - Ok
        *
        ****************************************************************************************************************************/
        public bool MavlinkRxParser(byte data_byte)
        {
            bool rc = true;

            /* parse the incoming byte */
            mavlink_rx_packet[mavlink_rx_index++] = data_byte;
            switch (mavlink_rx_index)
            {
                case 0:
                    mavlink_rx_index = 0;
                    break;

                case 1:
                    if (data_byte != MAVLINK_START_OF_FRAME)
                        mavlink_rx_index = 0;
                    break;

                case 2:
                    mavlink_rx_payload_len = data_byte;
                    if (mavlink_rx_payload_len != MAVLINK_V2_EXT_MSG_LEN)
                        mavlink_rx_index = 0;
                    break;

                case 3:
                case 4:
                case 5:
                    break;

                case 6:
                    mavlink_rx_msg_id = data_byte;
                    break;

                default:
                    {
                        if (mavlink_rx_index >= (mavlink_rx_payload_len + MAVLINK_HEADER_LENGTH))
                        {
                            ushort calculated_crc;
                            ushort packet_crc;

                            /* in this demo only V2 Ext Message is parsed */
                            if (mavlink_rx_msg_id != MAVLINK_V2_EXT_MSGID)
                            {
                                mavlink_rx_payload_len = 0;
                                mavlink_rx_index = 0;
                                break;
                            }

                            /* calculate the CRC from the packet bytes */
                            calculated_crc = MavlinkCrcCalculate(mavlink_rx_packet, mavlink_rx_payload_len + MAVLINK_HEADER_LENGTH - 2);
                            calculated_crc = MavlinkCrcAccumulate(MAVLINK_V2_EXT_MSG_CRC, calculated_crc);

                            /* read the CRC from the packet CRC bytes */
                            packet_crc = mavlink_rx_packet[mavlink_rx_payload_len + 7];
                            packet_crc <<= 8;
                            packet_crc |= mavlink_rx_packet[mavlink_rx_payload_len + 6];

                            /* validate that the checksum is Ok */
                            if (calculated_crc == packet_crc)
                            {
                                //Log2RichText(mav_rx_packet, rx_payload_len + MAVLINK_HEADER_LENGTH, RxRichTextBox);
                                rc = false;
                            }

                            /* clear the rx index & payload length for the next packet */
                            mavlink_rx_payload_len = 0;
                            mavlink_rx_index = 0;
                            break;
                        }
                    }
                    break;
            }
            return rc;
        }

        /****************************************************************************************************************************
        *                                            MavlinkCrcCalculate()
        *
        * Description : calculates CRC for Mavlink Protocol
        *
        * Arguments   : byte[] pBuffer - buffer pointer
        * 				int length - buffer length
        *
        * Returns     : CRC
        *
        ****************************************************************************************************************************/
        private ushort MavlinkCrcCalculate(byte[] pBuffer, int length)
        {
            ushort crcTmp;
            int i;

            if (length < 1)
            {
                return 0xffff;
            }

            crcTmp = X25_INIT_CRC;

            for (i = 1; i < length; i++) // skips header
            {
                crcTmp = MavlinkCrcAccumulate(pBuffer[i], crcTmp);
            }

            return (crcTmp);
        }

        /****************************************************************************************************************************
        *                                            MavlinkCrcAccumulate()
        *
        * Description : Adds additional byte to CRC calculate
        *
        * Arguments   : byte and previous CRC
        *
        * Returns     : new CRC
        *
        ****************************************************************************************************************************/
        private ushort MavlinkCrcAccumulate(byte b, ushort crc)
        {
            byte ch = (byte)(b ^ (byte)(crc & 0x00ff));
            ch = (byte)(ch ^ (ch << 4));

            return (ushort)((crc >> 8) ^ ((ushort)ch << 8) ^ ((ushort)ch << 3) ^ ((ushort)ch >> 4));
        }

        /****************************************************************************************************************************
        *                                                      ColibCalcCSForMavMsg()
        *
        * Description : Calculate the colibri checksum and place back in the mavlink V2 Ext packet
        *               The function assumes that the input packet is always a V2 Ext Mavlink Message
        *               So the colibri packet is in bytes:  6 to 25 
        *
        * Arguments   : byte[] mav_packet - buffer containing a the V2 Ext mavlink message
        *
        * Returns     : none
        *
        ****************************************************************************************************************************/
        private void ColibCalcCSForMavMsg(byte[] mav_packet)
        {
            int i;
            byte cs = 0;

            /* calculate the packet checksum */
            for (i = 6; i < 25; i++)
                cs += mav_packet[i];

            /* set the check sum */
            mav_packet[25] = cs;
        }
    }
}
