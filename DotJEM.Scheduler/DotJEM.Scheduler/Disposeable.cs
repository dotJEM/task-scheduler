using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;

namespace DotJEM.Scheduler
{
    public abstract class Disposeable : IDisposable
    {
        protected volatile bool Disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            Disposed = true;
        }

        ~Disposeable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public static class Correlator
    {
        private const string CORRELATION_KEY = "CORRELATION_KEY";
        private const string EMPTY = "00000000";

        public static void Set(Guid id)
        {
            CallContext.LogicalSetData(CORRELATION_KEY, Hash(id.ToByteArray(), 5));
        }

        public static string Get()
        {
            string ctx = (string)CallContext.LogicalGetData(CORRELATION_KEY);
            return ctx ?? EMPTY;
        }

        private static string Hash(byte[] bytes, int size)
        {
            using (SHA1 hasher = SHA1.Create())
            {
                byte[] hash = hasher.ComputeHash(bytes);
                return string.Join(string.Empty, hash.Take(size).Select(b => b.ToString("x2")));
            }
        }
    }
}