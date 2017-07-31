// Copyright Muck van Weerdenburg 2011.
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;

namespace PTP
{
    public class PTPDevice : IDisposable
    {
        public UsbDevice Device { get; }
        public bool IsOpen { get { return Device.IsOpen; } }
        public UsbEndpointReader Reader;
        public UsbEndpointWriter Writer;
        public byte ConfigurationID;
        public int InterfaceID;
        public ReadEndpointID ReaderEndpointID;
        public WriteEndpointID WriterEndpointID;
        public bool PTPSupported;
        public string Name { get; }

        public PTPDevice(UsbDevice device)
        {
            Device = device;
            PTPSupported = false;
            Name = Device.Info.ProductString; // TODO: try get better name
            Reader = null;
            Writer = null;
            ConfigurationID = 1;
            InterfaceID = 0;
            ReaderEndpointID = ReadEndpointID.Ep01;
            WriterEndpointID = WriteEndpointID.Ep02;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    ((IDisposable)Device).Dispose();

                if (IsOpen)
                    Close();

                disposedValue = true;
            }
        }

        ~PTPDevice()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public bool Open()
        {
            if (IsOpen)
                return false;

            if (!Device.Open())
                return false;

            IUsbDevice whole = Device as IUsbDevice;
            if (!ReferenceEquals(whole, null))
            {
                if (!whole.SetConfiguration(ConfigurationID) || !whole.ClaimInterface(InterfaceID))
                {
                    Device.Close();
                    throw new PTPException("could not set USB device configuration and interface to " + ConfigurationID + " and " + InterfaceID + ", respectively");
                }
            }

            Writer = Device.OpenEndpointWriter(WriterEndpointID);
            Reader = Device.OpenEndpointReader(ReaderEndpointID);

            return true;
        }

        public bool Close()
        {
            if (!IsOpen)
                return false;

            IUsbDevice whole = Device as IUsbDevice;
            if (!ReferenceEquals(whole, null))
            {
                whole.ReleaseInterface(InterfaceID);
            }

            return Device.Close();
        }

        public override string ToString()
        {
            return Device.Info.ProductString;
        }
    }
}