﻿// Copyright Muck van Weerdenburg 2011.
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
        public static IEnumerable<CHDKPTPDevice> ListDevices(bool only_supported = true)
        {
            return CHDKPTPUtil.FindDevices(only_supported);
        }

        private CHDKPTPSession _session;

        public Session(CHDKPTPDevice device)
        {
            _session = new CHDKPTPSession(device);
        }

        #region IDisposable Support

        private bool disposedValue = false;

        public void Dispose()
        {
            if (!disposedValue)
            {
                if (_session.IsOpen)
                    Disconnect();

                _session.Dispose();

                disposedValue = true;
            }
        }

        #endregion

        public void Connect()
        {
            _session.Device.Open();
            try
            {
                _session.OpenSession();
            }
            catch (Exception e)
            {
                _session.Device.Close();
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
                _session.Device.Close();
            }
        }

        // returns return data (if any) unless get_error is true
        private object GetScriptMessage(int script_id, bool return_string_as_byte_array, bool get_error = false)
        {
            while (true)
            {
                _session.CHDK_ReadScriptMsg(out CHDK_ScriptMsgType type, out int subtype, out int script_id2, out byte[] data);

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
            if (!_session.IsOpen)
                Connect();

            _session.CHDK_ExecuteScript(script, CHDK_ScriptLanguage.PTP_CHDK_SL_LUA, out int script_id, out CHDK_ScriptErrorType status);

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
                _session.CHDK_ScriptStatus(out CHDK_ScriptStatus flags);
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
            _session.CHDK_DownloadFile(filename, out byte[] buf);

            return buf;
        }

        public CHDKPTPDevice Device
        {
            get
            {
                return (CHDKPTPDevice)_session.Device;
            }
        }
    }
}
