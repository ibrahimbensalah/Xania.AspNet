using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.AspNet.Core
{

    public class ConcatenatedStream : TextReader
    {
        private readonly IEnumerable<Stream> _streams;
        private readonly IEnumerator<StreamReader> _enumerator;

        public ConcatenatedStream(params Stream[] streams)
        {
            _streams = streams;
            _enumerator = _streams.Select(e => new StreamReader(e)).GetEnumerator();
            _enumerator.MoveNext();
        }

        public override int Read()
        {
            if (_enumerator.Current == null)
                return -1;

            var ch = _enumerator.Current.Read();
            if (ch != -1)
                return ch;

            if (!_enumerator.MoveNext())
                return -1;

            return '\n';
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var stream in _streams)
                {
                    stream.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
