#ifdef ZSTD

#include "leveldb/zstd_compressor.h"

#include <zstd.h>

void leveldb::ZstdCompressor::compressImpl(const char* input, size_t length, ::std::string& output) const
{
	//extend the buffer to the worst case
	auto originalSize = output.size();
	auto capacity = ZSTD_compressBound(length);
	output.resize(originalSize + capacity);

	//and then compress into it
	auto sz = ZSTD_compress((void*)(output.data() + originalSize), capacity, input, length, compressionLevel);

	assert(!ZSTD_isError(sz));

	output.resize(sz + originalSize);
}


bool leveldb::ZstdCompressor::decompress(const char* input, size_t length, ::std::string &output) const
{
	//extend the buffer to contain the worst case. Worst case is that input length == output length
	auto originalSize = output.size();
	
	auto bufsize = length;

	while (true) 
	{
		bufsize *= 10; //assume that the compression is compressing worse than 10%. 
		//TODO use streams to decompress piece by piece, this is pretty wasteful for memory & re-decompressions
		output.resize(originalSize + bufsize);

		auto sz = ZSTD_decompress((void*)(output.data() + originalSize), bufsize, input, length);
		if (!ZSTD_isError(sz)) 
		{
			output.resize(sz + originalSize);
			break;
		}
	}

	return true;
}

#endif


