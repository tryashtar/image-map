using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LevelDBWrapper
{
    public static class Interop
    {
        public static readonly Encoding Encoding = Encoding.UTF8;

        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_open(IntPtr options, string name, out IntPtr error);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_options_create();
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_writeoptions_create();
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_writebatch_create();
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_writebatch_put(IntPtr batch, byte[] key, int keylen, byte[] val, int vallen);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_write(IntPtr db, IntPtr options, IntPtr batch, out IntPtr errptr);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_delete(IntPtr db, IntPtr options, byte[] key, IntPtr keylen, out IntPtr errptr);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_get(IntPtr db, IntPtr options, byte[] key, IntPtr keylen, out IntPtr vallen, out IntPtr errptr);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_put(IntPtr db, IntPtr options, byte[] key, IntPtr keylen, byte[] val, IntPtr vallen, out IntPtr errptr);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_readoptions_create();
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_free(IntPtr ptr);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_create_snapshot(IntPtr db);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_create_iterator(IntPtr db, IntPtr options);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_readoptions_set_snapshot(IntPtr options, IntPtr snapshot);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_release_snapshot(IntPtr db, IntPtr snapshot);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte leveldb_iter_valid(IntPtr iterator);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_seek_to_first(IntPtr iterator);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_seek_to_last(IntPtr iterator);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_seek(IntPtr iterator, byte[] key, int length);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_seek(IntPtr iterator, ref int key, int length);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_next(IntPtr iterator);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_prev(IntPtr iterator);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_iter_key(IntPtr iterator, out int length);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr leveldb_iter_value(IntPtr iterator, out int length);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_iter_destroy(IntPtr iterator);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_options_set_compression(IntPtr options, int n);
        [DllImport("leveldb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void leveldb_close(IntPtr db);
    }
}
