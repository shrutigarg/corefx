// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression
{
    public class GZipStream : Stream
    {
        private DeflateStream _deflateStream;


        public GZipStream(Stream stream, CompressionMode mode)

            : this(stream, mode, false)
        {
        }


        public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
        {
            if (mode == CompressionMode.Decompress)
            {
                _deflateStream = new DeflateStream(stream, leaveOpen, new GZipDecoder());
            }
            else
            {
                _deflateStream = new DeflateStream(stream, mode, leaveOpen);
                _deflateStream.SetFileFormatWriter(new GZipFormatter());
            }
        }


        // Implies mode = Compress
        public GZipStream(Stream stream, CompressionLevel compressionLevel)

            : this(stream, compressionLevel, false)
        {
        }


        // Implies mode = Compress
        public GZipStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen)
        {
            _deflateStream = new DeflateStream(stream, compressionLevel, leaveOpen);
            _deflateStream.SetFileFormatWriter(new GZipFormatter());
        }

        public override bool CanRead
        {
            get
            {
                if (_deflateStream == null)
                {
                    return false;
                }

                return _deflateStream.CanRead;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (_deflateStream == null)
                {
                    return false;
                }

                return _deflateStream.CanWrite;
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (_deflateStream == null)
                {
                    return false;
                }

                return _deflateStream.CanSeek;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException(SR.NotSupported);
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException(SR.NotSupported);
            }

            set
            {
                throw new NotSupportedException(SR.NotSupported);
            }
        }

        public override void Flush()
        {
            CheckDeflateStream();
            _deflateStream.Flush();
            return;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.NotSupported);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.NotSupported);
        }

        public override int Read(byte[] array, int offset, int count)
        {
            CheckDeflateStream();
            return _deflateStream.Read(array, offset, count);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            CheckDeflateStream();
            _deflateStream.Write(array, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _deflateStream != null)
                {
                    _deflateStream.Dispose();
                }
                _deflateStream = null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public Stream BaseStream
        {
            get
            {
                if (_deflateStream != null)
                {
                    return _deflateStream.BaseStream;
                }
                else
                {
                    return null;
                }
            }
        }

        public override Task<int> ReadAsync(Byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            CheckDeflateStream();
            return _deflateStream.ReadAsync(array, offset, count, cancellationToken);
        }

        public override Task WriteAsync(Byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            CheckDeflateStream();
            return _deflateStream.WriteAsync(array, offset, count, cancellationToken);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            CheckDeflateStream();
            return _deflateStream.FlushAsync(cancellationToken);
        }

        private void CheckDeflateStream()
        {
            if (_deflateStream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }
        }
    }
}
