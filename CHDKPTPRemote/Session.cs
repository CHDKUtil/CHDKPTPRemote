// Copyright Muck van Weerdenburg 2011.
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

using CHDKPTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHDKPTPRemote
{
    public class Session : IDisposable
    {
        public static List<CHDKPTPDevice> ListDevices(bool only_supported = true)
        {
            return CHDKPTPUtil.FindDevices(only_supported);
        }

        private CHDKPTPSession _session;

        public Session(CHDKPTPDevice dev)
        {
            _session = new CHDKPTPSession(dev);
        }

        #region IDisposable Support

        private bool disposedValue = false;

        public void Dispose()
        {
            if (!disposedValue)
            {
                _session.Dispose();

                disposedValue = true;
            }
        }

        #endregion

        public void Connect()
        {
            _session.device.Open();
            try
            {
                _session.OpenSession();
            }
            catch (Exception e)
            {
                _session.device.Close();
                throw e;
            }
        }

        public void Disconnect()
        {
            try
            {
                _session.CloseSession();
            }
            catch (Exception)
            {
            }
            finally
            {
                _session.device.Close();
            }
        }

        // returns return data (if any) unless get_error is true
        private object GetScriptMessage(int script_id, bool return_string_as_byte_array, bool get_error = false)
        {
            CHDK_ScriptMsgType type;
            int subtype, script_id2;
            byte[] data;
            while (true)
            {
                _session.CHDK_ReadScriptMsg(out type, out subtype, out script_id2, out data);

                if (type == CHDK_ScriptMsgType.PTP_CHDK_S_MSGTYPE_NONE) // no more messages; no return value
                    return null;

                if (script_id2 != script_id) // ignore message from other scripts
                    continue;

                if (!get_error && type == CHDK_ScriptMsgType.PTP_CHDK_S_MSGTYPE_RET) // return info!
                {
                    CHDK_ScriptDataType chdkSubtype = (CHDK_ScriptDataType)subtype;
                    switch (chdkSubtype)
                    {
                        case CHDK_ScriptDataType.PTP_CHDK_TYPE_NIL:
                            return null;
                        case CHDK_ScriptDataType.PTP_CHDK_TYPE_BOOLEAN:
                            return (data[0] | data[1] | data[2] | data[3]) != 0;
                        case CHDK_ScriptDataType.PTP_CHDK_TYPE_INTEGER:
                            return data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
                        case CHDK_ScriptDataType.PTP_CHDK_TYPE_STRING:
                            if (return_string_as_byte_array)
                                return data;
                            else
                                return (new ASCIIEncoding()).GetString(data);
                        case CHDK_ScriptDataType.PTP_CHDK_TYPE_TABLE:
                            string str = Encoding.ASCII.GetString(data);
                            string[] split = str.Split('\n');
                            Dictionary<string, string> table = new Dictionary<string, string>();
                            foreach (var substr in split)
                            {
                                string[] kvp = substr.Split('\t');
                                if (kvp.Length != 2)
                                    break;
                                table.Add(kvp[0], kvp[1]);
                            }
                            return table;
                        default:
                            throw new Exception("script returned unsupported data type: " + chdkSubtype.ToString());
                    }
                }

                if (type == CHDK_ScriptMsgType.PTP_CHDK_S_MSGTYPE_ERR) // hmm.. error
                {
                    if (get_error)
                    {
                        return (new ASCIIEncoding()).GetString(data);
                    }
                    else
                    {
                        throw new Exception("error running script: " + (new ASCIIEncoding()).GetString(data));
                    }
                }

                // ignore other (user) messages
            }
        }

        // TODO: should be able to distinguish "real" exceptions and script errors
        public object ExecuteScript(string script, bool return_string_as_byte_array = false)
        {
            int script_id;
            CHDK_ScriptErrorType status;
            _session.CHDK_ExecuteScript(script, CHDK_ScriptLanguage.PTP_CHDK_SL_LUA, out script_id, out status);

            if (status == CHDK_ScriptErrorType.PTP_CHDK_S_ERRTYPE_COMPILE)
            {
                object msg = GetScriptMessage(script_id, false, true);
                if (msg.GetType() == typeof(string))
                {
                    throw new Exception("script compilation error: " + (string)msg);
                }
                else
                {
                    throw new Exception("script compilation error (unknown reason)");
                }
            }

            // wait for end
            while (true)
            {
                CHDK_ScriptStatus flags;
                _session.CHDK_ScriptStatus(out flags);
                if (!flags.HasFlag(CHDK_ScriptStatus.PTP_CHDK_SCRIPT_STATUS_RUN))
                {
                    break;
                }

                System.Threading.Thread.Sleep(100);
            }

            // get result
            return GetScriptMessage(script_id, return_string_as_byte_array);
        }

        public byte[] DownloadFile(string filename)
        {
            byte[] buf;

            _session.CHDK_DownloadFile(filename, out buf);

            return buf;
        }

        /* Add CHDK live view */
        private byte[] chdk_live_buffer;
        public void GetLiveData(int live_xfer_type)
        {
            _session.CHDK_GetLiveViewData(out chdk_live_buffer, live_xfer_type);
        }


        /* add support live view, code from Muck van Weerdenburg */
        private int lcd_aspect_ratio;
        private int palette_type;
        private int palette_data_start;
        private int palette_size;

        private int vp_fb_type;
        private int vp_max_width;
        private int vp_max_height;
        private int vp_buffer_width;
        private int vp_buf_start;
        private int vp_buf_size;

        private int bm_fb_type;
        private int bm_max_width;
        private int bm_max_height;
        private int bm_buffer_width;
        private int bm_buf_start;
        private int bm_buf_size;

        private int int_from_bytes(byte[] buf, int offset)
        {
            return buf[offset] | (buf[offset + 1] << 8) | (buf[offset + 2] << 16) | (buf[offset + 3] << 24);
        }

        private byte clip(int val)
        {
            if (val > 255)
                return 255;
            else if (val < 0)
                return 0;
            else
                return (byte)val;
        }

        private void add_pixel(byte[] buf, int pos, byte y, sbyte u, sbyte v)
        {
            buf[pos] = clip(((y << 12) + u * 7258 + 2048) >> 12); // b
            buf[pos + 1] = clip(((y << 12) - u * 1411 - v * 2925 + 2048) >> 12); // g
            buf[pos + 2] = clip(((y << 12) + v * 5743 + 2048) >> 12); // r
        }


        private int ayuv2argb(byte a, byte y, sbyte u, sbyte v)
        {
            byte r = clip(((y << 12) + v * 5743 + 2048) >> 12); // r
            byte g = clip(((y << 12) - u * 1411 - v * 2925 + 2048) >> 12); // g
            byte b = clip(((y << 12) + u * 7258 + 2048) >> 12); // b
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        private void GetImageInfoFromData(byte[] chdk_live_buffer)
        {
            lcd_aspect_ratio = int_from_bytes(chdk_live_buffer, 0x08);
            palette_type = int_from_bytes(chdk_live_buffer, 0x0c);
            palette_data_start = int_from_bytes(chdk_live_buffer, 0x10);
            int vp_desc_start = int_from_bytes(chdk_live_buffer, 0x14);
            int bm_desc_start = int_from_bytes(chdk_live_buffer, 0x18);


            // Viewport info
            vp_fb_type = int_from_bytes(chdk_live_buffer, vp_desc_start);
            vp_buf_start = int_from_bytes(chdk_live_buffer, vp_desc_start + (4 * 1));
            vp_buffer_width = int_from_bytes(chdk_live_buffer, vp_desc_start + (4 * 2));
            vp_max_width = int_from_bytes(chdk_live_buffer, vp_desc_start + (4 * 3));
            vp_max_height = int_from_bytes(chdk_live_buffer, vp_desc_start + (4 * 4));

            // Bitmap info
            bm_fb_type = int_from_bytes(chdk_live_buffer, bm_desc_start);
            bm_buf_start = int_from_bytes(chdk_live_buffer, bm_desc_start + (4 * 1));
            bm_buffer_width = int_from_bytes(chdk_live_buffer, bm_desc_start + (4 * 2));
            bm_max_width = int_from_bytes(chdk_live_buffer, bm_desc_start + (4 * 3));
            bm_max_height = int_from_bytes(chdk_live_buffer, bm_desc_start + (4 * 4));


            if (vp_buf_start > 0)
            {
                vp_buf_size = chdk_live_buffer.Length - vp_buf_start;
                // If have both Viewport and Bitmap info, using bm_buf_start at end of vp data. 
                if (bm_buf_start > 0x64)
                {
                    vp_buf_size = bm_buf_start - vp_buf_start;
                }

            }
            if (bm_buf_start > 0)
            {
                bm_buf_size = chdk_live_buffer.Length - bm_buf_start;
            }


            if (palette_data_start > 0)
            {
                palette_size = vp_buf_start - palette_data_start;
                if (vp_buf_start > 0 && bm_buf_start > 0)
                    palette_size = vp_buf_start - palette_data_start;
                else
                    palette_size = bm_buf_start - palette_data_start;

            }

        }
    }
}
