using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MemoryUtils
{
    public static class MethodBus {
        public static List<Func<bool>> Methods { get; set; } = new List<Func<bool>>();

        public static void SubscribeMethod(Func<bool> Method) {
            Methods.Add(Method);
        }
    }
}
