using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextVisionVideoControlLibrary
{
    public struct klv_tag
    {
        public bool valid;
        public int id;
        public int len;
        public byte[] data;
    }

    public enum receiver_states
    {
        SyncOnIFrame,
        CatchFrames,
    }

    public enum recording_cmd
    {
        RecNoCmd,
        RecStart,
        RecStop,
    }

    public enum recording_status
    {
        RecordingIdle,
        RecordingEnabled,
        RecordingFailure,
    }

    public enum stream_status
    {
        StreamIdle,
        StreamAcquiring,
        StreamDetectionOk,
        StreamLost,
    }
}
