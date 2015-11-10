using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dron.Helpers
{
    interface IControl
    {
        int Roll { get; }
        int Pitch { get; }
        int Gaz { get; }
        int Yaw { get; }
        void Refresh();
    }
}
