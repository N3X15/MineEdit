cmake_minimum_required (VERSION 2.6)

# prefer static
SET(CMAKE_FIND_LIBRARY_PREFIXES "lib")
SET (CMAKE_FIND_LIBRARY_SUFFIXES .a)

set(ZLIB_FIND_QUIETLY ON)
set(ZLIB_FIND_REQUIRED ON)

find_package(ZLIB)

project (lightgen)

set(LIGHTGEN_SOURCE_FILES
	blockinfo.cpp  
	chunkstore.cpp  
	common.cpp  
	lightgen.cpp  
	main.cpp  
	nbt.cpp
	)

include_directories(
    ${ZLIB_INCLUDE_DIRS}
    )
add_executable(lightgen ${LIGHTGEN_SOURCE_FILES})

target_link_libraries(
    lightgen
    ${ZLIB_LIBRARIES}
    )