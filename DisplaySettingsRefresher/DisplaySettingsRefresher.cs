using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;

using Rainmeter;

namespace DisplaySettingsRefresher
{
    class Measure
    {
        public static implicit operator Measure(IntPtr data)
        {
            return (Measure)GCHandle.FromIntPtr(data).Target;
        }
        public IntPtr buffer = IntPtr.Zero;
    }

    public class Plugin
    {
        private static EventHandler _displayChangedHandler;
        private static string _command;
        private static void DisplayChanged(API api)
        {
            api.Execute(_command);
        }
        
        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
            
            API api = rm;
            
            _command = api.ReadString("Command", "[!RefreshApp]");
            
            _displayChangedHandler = delegate
            {
                DisplayChanged(api);
            };
            
            SystemEvents.DisplaySettingsChanged += _displayChangedHandler;
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            SystemEvents.DisplaySettingsChanged -= _displayChangedHandler;
            
            Measure measure = data;
            
            if (measure.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(measure.buffer);
            }
            
            GCHandle.FromIntPtr(data).Free();
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
        }

        [DllExport]
        public static double Update(IntPtr data) => 0.0;
    }
}

