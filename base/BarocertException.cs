using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Barocert
{
    public class BarocertException : Exception
    {
        public BarocertException()
            : base()
        {
        }
        public BarocertException(long code, string Message)
            : base(Message)
        {
            this._code = code;
        }

        private long _code;

        public long code
        {
            get { return _code; }
        }
        public BarocertException(Linkhub.LinkhubException le)
            : base(le.Message, le)
        {
            this._code = le.code;
        }
    }

}
