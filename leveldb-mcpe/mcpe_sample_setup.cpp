
int main() {

	class NullLogger : public leveldb::Logger {
	public:
		void Logv(const char*, va_list) override {
		}
	};

	leveldb::Options options;
	
	//create a bloom filter to quickly tell if a key is in the database or not
	options.filter_policy = leveldb::NewBloomFilterPolicy(10);

	//create a 40 mb cache (we use this on ~1gb devices)
	options.block_cache = leveldb::NewLRUCache(40 * 1024 * 1024);

	//create a 4mb write buffer, to improve compression and touch the disk less
	options.write_buffer_size = 4 * 1024 * 1024;

	//disable internal logging. The default logger will still print out things to a file
	options.info_log = new NullLogger();

	//use the new raw-zip compressor to write (and read)
	options.compressors[0] = new leveldb::ZlibCompressorRaw(-1);
	
	//also setup the old, slower compressor for backwards compatibility. This will only be used to read old compressed blocks.
	options.compressors[1] = new leveldb::ZlibCompressor();

	
	//create a reusable memory space for decompression so it allocates less
	leveldb::ReadOptions readOptions;
	readOptions.decompress_allocator = new leveldb::DecompressAllocator();


	//... init leveldb with Options and read with ReadOptions
}