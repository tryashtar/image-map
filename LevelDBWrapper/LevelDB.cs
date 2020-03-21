using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LevelDBWrapper
{
    public abstract class LevelDBHandle : IDisposable
    {
        public IntPtr Handle { protected set; get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void FreeManagedObjects() { }

        protected virtual void FreeUnManagedObjects() { }

        bool _disposed = false;
        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    FreeManagedObjects();
                if (this.Handle != IntPtr.Zero)
                {
                    FreeUnManagedObjects();
                    this.Handle = IntPtr.Zero;
                }
                _disposed = true;
            }
        }

        ~LevelDBHandle()
        {
            Dispose(false);
        }
    }

    public class LevelDB : LevelDBHandle, IEnumerable<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<byte[], byte[]>>, IEnumerable<KeyValuePair<string, byte[]>>
    {
        public LevelDB(string path)
        {
            Console.WriteLine($"Opening {path}");
            IntPtr error;
            var options = new Options();
            Handle = Interop.leveldb_open(options.Handle, path, out error);
            Throw(error);
        }

        private static void Throw(IntPtr error)
        {
            if (error != IntPtr.Zero)
            {
                try
                {
                    var msg = Marshal.PtrToStringAnsi(error);
                    throw new Exception(msg);
                }
                finally
                {
                    Interop.leveldb_free(error);
                }
            }
        }

        public void Write(WriteBatch batch)
        {
            IntPtr error;
            var options = new WriteOptions();
            Interop.leveldb_write(this.Handle, options.Handle, batch.Handle, out error);
            Throw(error);
        }

        public void Delete(string key)
        {
            Delete(Interop.Encoding.GetBytes(key));
        }

        public void Delete(byte[] key)
        {
            IntPtr error;
            var options = new WriteOptions();
            Interop.leveldb_delete(this.Handle, options.Handle, key, (IntPtr)key.LongLength, out error);
            Throw(error);
        }

        public byte[] Get(string key)
        {
            return Get(Interop.Encoding.GetBytes(key));
        }

        public byte[] Get(byte[] key)
        {
            IntPtr error;
            IntPtr length;
            var options = new ReadOptions();
            var v = Interop.leveldb_get(this.Handle, options.Handle, key, (IntPtr)key.LongLength, out length, out error);
            Throw(error);

            if (v != IntPtr.Zero)
            {
                try
                {
                    var bytes = new byte[(long)length];
                    Marshal.Copy(v, bytes, 0, checked((int)length));
                    return bytes;
                }
                finally
                {
                    Interop.leveldb_free(v);
                }
            }
            return null;
        }

        public void Put(string key, string value)
        {
            this.Put(Interop.Encoding.GetBytes(key), Interop.Encoding.GetBytes(value));
        }


        public void Put(string key, byte[] value)
        {
            this.Put(Interop.Encoding.GetBytes(key), value);
        }

        public void Put(byte[] key, byte[] value)
        {
            IntPtr error;
            var options = new WriteOptions();
            Interop.leveldb_put(this.Handle, options.Handle, key, (IntPtr)key.LongLength, value, (IntPtr)value.LongLength, out error);
            Throw(error);
        }

        public void Close()
        {
            Dispose();
        }

        protected override void FreeUnManagedObjects()
        {
            if (this.Handle != default(IntPtr))
                Interop.leveldb_close(this.Handle);
        }

        public Snapshot CreateSnapshot()
        {
            return new Snapshot(Interop.leveldb_create_snapshot(this.Handle), this);
        }

        public Iterator CreateIterator(ReadOptions options)
        {
            return new Iterator(Interop.leveldb_create_iterator(this.Handle, options.Handle));
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            using (var sn = this.CreateSnapshot())
            using (var iterator = this.CreateIterator(new ReadOptions { Snapshot = sn }))
            {
                iterator.SeekToFirst();
                while (iterator.IsValid())
                {
                    yield return new KeyValuePair<string, string>(iterator.StringKey(), iterator.StringValue());
                    iterator.Next();
                }
            }
        }

        IEnumerator<KeyValuePair<string, byte[]>> IEnumerable<KeyValuePair<string, byte[]>>.GetEnumerator()
        {
            using (var sn = this.CreateSnapshot())
            using (var iterator = this.CreateIterator(new ReadOptions { Snapshot = sn }))
            {
                iterator.SeekToFirst();
                while (iterator.IsValid())
                {
                    yield return new KeyValuePair<string, byte[]>(iterator.StringKey(), iterator.Value());
                    iterator.Next();
                }
            }
        }

        public IEnumerator<KeyValuePair<byte[], byte[]>> GetEnumerator()
        {
            using (var sn = this.CreateSnapshot())
            using (var iterator = this.CreateIterator(new ReadOptions { Snapshot = sn }))
            {
                iterator.SeekToFirst();
                while (iterator.IsValid())
                {
                    yield return new KeyValuePair<byte[], byte[]>(iterator.Key(), iterator.Value());
                    iterator.Next();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class Options : LevelDBHandle
    {
        public Options()
        {
            Handle = Interop.leveldb_options_create();
            Interop.leveldb_options_set_compression(this.Handle, 4);
        }
    }

    public class WriteOptions : LevelDBHandle
    {
        public WriteOptions()
        {
            Handle = Interop.leveldb_writeoptions_create();
        }
    }

    public class ReadOptions : LevelDBHandle
    {
        public ReadOptions()
        {
            Handle = Interop.leveldb_readoptions_create();
        }

        public Snapshot Snapshot
        {
            set { Interop.leveldb_readoptions_set_snapshot(this.Handle, value.Handle); }
        }
    }

    public class WriteBatch : LevelDBHandle
    {
        public WriteBatch()
        {
            Handle = Interop.leveldb_writebatch_create();
        }

        public WriteBatch Put(string key, string value)
        {
            return Put(Interop.Encoding.GetBytes(key), Interop.Encoding.GetBytes(value));
        }

        public WriteBatch Put(string key, byte[] value)
        {
            return Put(Interop.Encoding.GetBytes(key), value);
        }

        public WriteBatch Put(byte[] key, byte[] value)
        {
            Interop.leveldb_writebatch_put(Handle, key, key.Length, value, value.Length);
            return this;
        }
    }

    public class Snapshot : LevelDBHandle
    {
        public WeakReference Parent;

        internal Snapshot(IntPtr Handle, LevelDB parent)
        {
            this.Handle = Handle;
            this.Parent = new WeakReference(parent);
        }

        internal Snapshot(IntPtr Handle)
        {
            this.Handle = Handle;
            Parent = new WeakReference(null);
        }

        protected override void FreeUnManagedObjects()
        {
            if (Parent.IsAlive)
            {
                var parent = Parent.Target as LevelDB;
                if (parent != null)
                    Interop.leveldb_release_snapshot(parent.Handle, this.Handle);
            }
        }
    }

    public class Iterator : LevelDBHandle
    {
        internal Iterator(IntPtr Handle)
        {
            this.Handle = Handle;
        }

        public bool IsValid()
        {
            return (int)Interop.leveldb_iter_valid(this.Handle) != 0;
        }

        public void SeekToFirst()
        {
            Interop.leveldb_iter_seek_to_first(this.Handle);
        }

        public void SeekToLast()
        {
            Interop.leveldb_iter_seek_to_last(this.Handle);
        }

        public void Seek(byte[] key)
        {
            Interop.leveldb_iter_seek(this.Handle, key, key.Length);
        }

        public void Seek(string key)
        {
            Seek(Interop.Encoding.GetBytes(key));
        }

        public void Next()
        {
            Interop.leveldb_iter_next(this.Handle);
        }

        public void Prev()
        {
            Interop.leveldb_iter_prev(this.Handle);
        }


        public string StringKey()
        {
            return Interop.Encoding.GetString(this.Key());
        }

        public byte[] Key()
        {
            int length;
            var key = Interop.leveldb_iter_key(this.Handle, out length);

            var bytes = new byte[length];
            Marshal.Copy(key, bytes, 0, length);
            return bytes;
        }

        public string StringValue()
        {
            return Interop.Encoding.GetString(this.Value());
        }

        public byte[] Value()
        {
            int length;
            var value = Interop.leveldb_iter_value(this.Handle, out length);

            var bytes = new byte[length];
            Marshal.Copy(value, bytes, 0, length);
            return bytes;
        }

        protected override void FreeUnManagedObjects()
        {
            Interop.leveldb_iter_destroy(this.Handle);
        }
    }
}
