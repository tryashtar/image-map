#ifdef ZOPFLI

#include "leveldb/zopfli_compressor.h"
#include "leveldb/zlib_compressor.h"

#include <zlib.h>
#include <zopfli/zopfli.h>
#include <algorithm>
namespace leveldb {

	void ZopfliCompressor::compressImpl(const char* input, size_t length, ::std::string& output) const
	{
		//extend the buffer to the worst case
		auto originalSize = output.size();

		ZopfliOptions options;
		ZopfliInitOptions(&options);

		size_t outsize = 0;
		auto buffer = (char*)nullptr;
		ZopfliCompress(&options, ZOPFLI_FORMAT_ZLIB, (uint8_t*)input, length, (uint8_t**)&buffer, &outsize);

		assert(outsize > 0);

		output.append(buffer, outsize);
		free(buffer);
	}

	bool ZopfliCompressor::decompress(const char* input, size_t length, ::std::string &output) const {
		return ZlibCompressor::inflate(input, length, output) == Z_OK;
	}
		
}

#endif