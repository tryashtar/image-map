#pragma once

#ifndef LEVELDB_DECOMPRESS_ALLOCATOR_H_
#define LEVELDB_DECOMPRESS_ALLOCATOR_H_

#include <mutex>
#include <vector>
#include <string>

namespace leveldb {
	class DLLX DecompressAllocator {
	public:
		virtual ~DecompressAllocator();

		virtual std::string get();
		virtual void release(std::string&& string);

		virtual void prune();

	protected:
		std::mutex mutex;
		std::vector<std::string> stack;
	};
}

#endif