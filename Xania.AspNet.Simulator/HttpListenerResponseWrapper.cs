using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Xania.AspNet.Simulator
{
    internal class HttpListenerResponseWrapper : HttpResponseBase, IDisposable
    {
        private readonly HttpListenerResponse _listenerResponse;
        private TextWriter _output;
        private readonly MemoryStream _outputStream;
        private bool _closed = false;
        private readonly HttpCookieCollection _cookies;

        public HttpListenerResponseWrapper(HttpListenerResponse listenerResponse)
        {
            _listenerResponse = listenerResponse;
            _outputStream = new MemoryStream();
            _output = new LoggingStreamWriter(new StreamWriter(_outputStream));
            _cookies = new HttpCookieCollection();
        }

        public override string ContentType { get; set; }

        public override Encoding ContentEncoding { get; set; }

        public override int StatusCode
        {
            get { return _listenerResponse.StatusCode; }
            set { _listenerResponse.StatusCode = value; }
        }

        public override HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }

        public override string Status
        {
            get 
            {
                return this.StatusCode.ToString(NumberFormatInfo.InvariantInfo) + " " + this.StatusDescription;
            }
            set
            {
                int i = value.IndexOf(' ');
                StatusCode = Int32.Parse(value.Substring(0, i), CultureInfo.InvariantCulture);
                StatusDescription = value.Substring(i + 1);
            }
        }

        public override string StatusDescription
        {
            get { return _listenerResponse.StatusDescription; }
            set { _listenerResponse.StatusDescription = value; }
        }

        public override TextWriter Output
        {
            get { return _output; }
            set { _output = value; }
        }

        public override Stream OutputStream
        {
            get { return _outputStream; }
        }

        public override NameValueCollection Headers
        {
            get { return _listenerResponse.Headers; }
        }

        public override string ApplyAppPathModifier(string virtualPath)
        {
            return virtualPath;
        }

        public override void Flush()
        {
        }

        public override void Write(char ch)
        {
            Output.Write(ch);
        }

        public override void Write(string s)
        {
            Output.Write(s);
        }

        public override void Close()
        {
            if (!_closed)
            {
                _closed = true;
                _output.Flush();

                var buffer = _outputStream.ToArray();
                _listenerResponse.ContentLength64 = buffer.Length;
                var output = _listenerResponse.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _outputStream.Dispose();
                _output.Dispose();
            }
        }

        ~HttpListenerResponseWrapper()
        {
            Dispose(false);
        }
    }

    internal class LoggingStreamWriter : TextWriter
    {
        private readonly StreamWriter _streamWriter;

        public LoggingStreamWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public override Encoding Encoding
        {
            get { return _streamWriter.Encoding; }
        }

        protected override void Dispose(bool disposing)
        {
            _streamWriter.Dispose();
        }

        public override void Close()
        {
            _streamWriter.Close();
        }

        public override void Write(bool value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(char value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(char[] buffer)
        {
            _streamWriter.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _streamWriter.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(double value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(float value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(int value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(long value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(object value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(string value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            _streamWriter.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            _streamWriter.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            _streamWriter.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            _streamWriter.Write(format, arg);
        }

        public override void Write(uint value)
        {
            _streamWriter.Write(value);
        }

        public override void Write(ulong value)
        {
            _streamWriter.Write(value);
        }

        public override Task WriteAsync(char value)
        {
            return _streamWriter.WriteAsync(value);
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            return _streamWriter.CreateObjRef(requestedType);
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return _streamWriter.WriteAsync(buffer, index, count);
        }

        public override bool Equals(object obj)
        {
            return _streamWriter.Equals(obj);
        }

        public override Task WriteAsync(string value)
        {
            return _streamWriter.WriteAsync(value);
        }

        public override void Flush()
        {
            _streamWriter.Flush();
        }

        public override Task FlushAsync()
        {
            return _streamWriter.FlushAsync();
        }

        public override IFormatProvider FormatProvider
        {
            get { return _streamWriter.FormatProvider; }
        }

        public override int GetHashCode()
        {
            return _streamWriter.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return _streamWriter.InitializeLifetimeService();
        }

        public override string NewLine
        {
            get { return _streamWriter.NewLine; }
            set { _streamWriter.NewLine = value; }
        }

        public override string ToString()
        {
            return _streamWriter.ToString();
        }

        public override void WriteLine()
        {
            _streamWriter.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            _streamWriter.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            _streamWriter.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(string value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            _streamWriter.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            _streamWriter.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            _streamWriter.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            _streamWriter.WriteLine(format, arg);
        }

        public override void WriteLine(uint value)
        {
            _streamWriter.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            _streamWriter.WriteLine(value);
        }

        public override Task WriteLineAsync()
        {
            return _streamWriter.WriteLineAsync();
        }

        public override Task WriteLineAsync(char value)
        {
            return _streamWriter.WriteLineAsync(value);
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return _streamWriter.WriteLineAsync(buffer, index, count);
        }

        public override Task WriteLineAsync(string value)
        {
            return _streamWriter.WriteLineAsync(value);
        }
    }
}