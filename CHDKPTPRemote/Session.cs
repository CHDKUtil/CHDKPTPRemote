// Copyright Muck van Weerdenburg 2011.
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using LibUsbDotNet; // XXX needed for call to CHDKPTPUtil.FindDevices
using CHDKPTP;

namespace CHDKPTPRemote
{
    public class Session
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
                    switch ((CHDK_ScriptDataType)subtype)
                    {
                        case CHDK_ScriptDataType.PTP_CHDK_TYPE_BOOLEAN:
                            return (data[0] | data[1] | data[2] | data[3]) != 0;
                        case CHDK_ScriptDataType.PTP_CHDK_TYPE_INTEGER:
                            return data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
                        case CHDK_ScriptDataType.PTP_CHDK_TYPE_STRING:
                            if (return_string_as_byte_array)
                                return data;
                            else
                                return (new ASCIIEncoding()).GetString(data);
                        default:
                            throw new Exception("script returned unsupported data type: " + type.ToString());
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


        private Bitmap GetLiveImage(bool set_active_area, ref Rectangle active_area, Bitmap old_img)
        {
            if (old_img != null)
            {
                if (old_img.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    throw new Exception("cannot reuse Bitmap: PixelFormat is not Format24bppRgb");
                }
                if (old_img.Width != vp_max_width || old_img.Height != vp_max_height)
                {
                    throw new Exception("cannot reuse Bitmap; dimensions are incorrect");
                }
            }

            GetImageInfoFromData(chdk_live_buffer);

            byte[] img = chdk_live_buffer;
            int image_start = vp_buf_start;
            int image_end = vp_buf_start + vp_buf_size;

            // convert uyvyyy to rgbrgbrgbrgb
            byte[] pixels = new byte[vp_max_width * vp_max_height * 3];
            for (int img_idx = image_start, pxls_idx = 0; img_idx < image_end; img_idx += ((vp_buffer_width - vp_max_width) * 6) / 4)
            {
                for (int x = 0; x < vp_max_width; x++, img_idx += 6, pxls_idx += 12)
                {
                    sbyte u = (sbyte)img[img_idx];
                    sbyte v = (sbyte)img[img_idx + 2];
                    add_pixel(pixels, pxls_idx, img[img_idx + 1], u, v);
                    add_pixel(pixels, pxls_idx + 3, img[img_idx + 3], u, v);
                    add_pixel(pixels, pxls_idx + 6, img[img_idx + 4], u, v);
                    add_pixel(pixels, pxls_idx + 9, img[img_idx + 5], u, v);
                }
            }

            // copy pixels to bitmap

            if (old_img == null)
                old_img = new Bitmap(vp_max_width, vp_max_height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Imaging.BitmapData bmpd = old_img.LockBits(new Rectangle(0, 0, vp_max_width, vp_max_height), System.Drawing.Imaging.ImageLockMode.WriteOnly, old_img.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpd.Scan0, pixels.Length);
            old_img.UnlockBits(bmpd);
            return old_img;
        }

        public Bitmap GetLiveImage( Bitmap old_img = null)
        {
            Rectangle tmp = new Rectangle();
            return GetLiveImage(false, ref tmp, old_img);
        }

        public Bitmap GetLiveImage(out Rectangle active_area, Bitmap old_img = null)
        {
            active_area = new Rectangle();
            return GetLiveImage(true, ref active_area, old_img);
        }


        public Bitmap GetLiveOverlay(Bitmap old_img = null)
        {
            if (old_img != null)
            {
                if (old_img.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    throw new Exception("cannot reuse Bitmap: PixelFormat is not Format32bppArgb");
                }
                if (old_img.Width != bm_max_width || old_img.Height != bm_max_height)
                {
                    throw new Exception("cannot reuse Bitmap; dimensions are incorrect");
                }
            }

            GetImageInfoFromData(chdk_live_buffer);

            byte[] img = chdk_live_buffer;
            int image_start = bm_buf_start;
            int image_end = chdk_live_buffer.Length;


            // TODO: make static and only fill when palette actually changed
            int[] palette = new int[256];
            byte[] alpha = { 128, 171, 214, 255 };
            int pal_start = palette_data_start;
            int pal_end = palette_data_start + palette_size;

            switch (palette_type)
            {
                case 1: // 16 AYUV values
                case 2: // 16 AYUV values with A in 0-3 range
                    for (int i = 0; i < 256; i++)
                    {
                        int off_a = 4 * (i & 0xf);
                        int off_b = 4 * (i >> 4);

                        int v = (sbyte)img[pal_start + off_a] + (sbyte)img[pal_start + off_b];
                        if (v > 127) v = 127;
                        else if (v < -128) v = -128;

                        int u = (sbyte)img[pal_start + off_a + 1] + (sbyte)img[pal_start + off_b + 1];
                        if (u > 127) u = 127;
                        else if (u < -128) u = -128;

                        int y = img[pal_start + off_a + 2] + img[pal_start + off_b + 2];
                        if (y > 255) y = 255;

                        int a = (img[pal_start + off_a + 3] + img[pal_start + off_b + 3]) / 2;
                        if (palette_type == 2) a = alpha[a & 3];

                        palette[i] = ayuv2argb((byte)a, (byte)y, (sbyte)u, (sbyte)v);
                    }
                    break;
                case 3: // 256 AYUV values with A in 0-3 range
                    palette[0] = ayuv2argb(0, img[pal_start + 2], (sbyte)img[pal_start + 1], (sbyte)img[pal_start + 0]);     // Force color 0 to be fully transparent
                    for (int i = 4; i < palette_size; i += 4)
                    {
                        palette[i >> 2] = ayuv2argb(alpha[img[pal_start + i + 3] & 3], img[pal_start + i + 2], (sbyte)img[pal_start + i + 1], (sbyte)img[pal_start + i + 0]);
                    }
                    break;
                default:
                    // Create a 'gray scale' palette;
                    palette[0] = 0;     // color 0 is fully transparent
                    for (int i = 1; i < 256; i++) palette[i] = ayuv2argb(255, (byte)i, 0, 0);
                    break;
            }


            // convert index to argb
            int[] pixels = new int[bm_max_width * bm_max_height];
            for (int img_idx = image_start, pxls_idx = 0; img_idx < image_end; img_idx += bm_buffer_width - bm_max_width)
            {
                for (int x = 0; x < bm_max_width; x++, img_idx++, pxls_idx++)
                {
                    pixels[pxls_idx] = palette[img[img_idx]];
                }
            }

            // copy pixels to bitmap
            if (old_img == null)
                old_img = new Bitmap(bm_max_width, bm_max_height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bmpd = old_img.LockBits(new Rectangle(0, 0, bm_max_width, bm_max_height), System.Drawing.Imaging.ImageLockMode.WriteOnly, old_img.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpd.Scan0, pixels.Length);
            old_img.UnlockBits(bmpd);

            return old_img;
        }

    }
}
