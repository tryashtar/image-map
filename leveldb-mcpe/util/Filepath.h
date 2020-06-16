#pragma once

#if defined(_MSC_VER)
#include <codecvt>
#include <string>
#include <fstream>
#endif

namespace port {

#if defined(_MSC_VER)
	// std::strings won't work for windows as all their STL/Win32 APIs assume char* are pure ASCII
	// "luckily", there is an unofficial parameter for iostream that takes a wide character Unicode string
	typedef std::wstring filepath;
	typedef wchar_t filepath_char;
#define _FILE_STR(str) L ## str
#else
	typedef std::string filepath;
	typedef char filepath_char;
#define _FILE_STR(str) str
#endif


	inline filepath toFilePath(const std::string &string) {
#if defined(_MSC_VER)
		std::wstring_convert<std::codecvt_utf8<wchar_t>, wchar_t> converter;
		return std::move(converter.from_bytes(string));
#else
		return std::move(string);
#endif
	}

#if defined(_MSC_VER)
	inline FILE* fopen_mb(const filepath_char* filename, const filepath_char* mode) {
		FILE* file = nullptr;

		errno_t error = _wfopen_s(&file, filename, mode);
		_set_errno(error);

		return file;
	}

	// this function will silently allocate memory on windows to convert char* to wchar_t*
	inline FILE* fopen_mb(const char* const filename, const filepath_char* mode) {
		filepath path = toFilePath(filename);

		return port::fopen_mb(path.c_str(), mode);
	}
#else
	inline FILE* fopen_mb(const filepath_char* filename, const filepath_char* mode) {
		return ::fopen(filename, mode);
	}
#endif
}
