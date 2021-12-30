using System;
using System.Runtime.InteropServices;

namespace QueryCiv3.Sav
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public unsafe struct PEER
    {
        private fixed byte HeaderText[4];
        public int Length;
        private fixed byte UnknownBuffer[8];
        public int MP1;
        public int MP2;
        private fixed byte UnknownBuffer2[8];
    }
}
