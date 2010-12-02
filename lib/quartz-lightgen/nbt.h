#pragma once

#include "common.h"

#include <string>
#include <map>
#include <stdio.h>
#include <string.h>

#include <zlib.h>

class NBT
{
public:
    enum TagType
    {
		Tag_End = 0,
		Tag_Byte,
		Tag_Short,
		Tag_Int,
		Tag_Long,
		Tag_Float,
		Tag_Double,
		Tag_Array,
		Tag_String,
		Tag_List,
		Tag_Compound
    };

#pragma pack(1)
    struct NBTString
    {
    	WORD length;
    	char *string;
    };
#pragma pack()

    struct NBTValueDescriptor
    {
		BYTE Type;
		DWORD Length;

		NBTString Name;

		union
		{
			BYTE *byte;
			WORD *word;
			DWORD *dword;
			QWORD *qword;
			float *fl;
			double *db;
			BYTE *arr;
			char *str;
		};
    };

    int GetCompressedLength() { return compressBound(m_memory); }
    bool LoadFromMemory(BYTE *data, int length);
    int CompressToMemory(BYTE *output, int outputLength);
    bool LoadFile(const string &fileName);
    bool WriteFile(const string &fileName);
    int GetMemoryUsage() { return m_memory; }
    BYTE *GetDataPointer() { return m_NBTBinary; }

    BYTE *GetTagDataPointer(const string &tagName);

	template<typename T>
		bool GetValue(const string &tagName, T *value)
	{
		if (m_NBT.count(tagName) != 0)
		{
			for (DWORD i = 0; i < sizeof(T); i++)
				((BYTE *)value)[sizeof(T) - 1 - i] = m_NBT[tagName].byte[i];
			return true;
		}

		return false;
	}

	bool GetValue(const string &tagName, string *value)
	{
		if (m_NBT.count(tagName) != 0)
		{
			*value = "";
			for (DWORD i = 0; i < m_NBT[tagName].Length; i++)
				*value += m_NBT[tagName].str[i];
			return true;
		}

		return false;
	}

	template<typename T>
		bool SetValue(const string &tagName, T value)
	{
		if (m_NBT.count(tagName) != 0)
		{
			for (DWORD i = 0; i < sizeof(T); i++)
				m_NBT[tagName].byte[i] = ((BYTE *)&value)[sizeof(T) - 1 - i];
			return true;
		}

		return false;
	}

	void Dump();
	BYTE *GetCompoundBinaryPointer(const string &tagName);
	bool LoadFromMemoryUncompressed(BYTE *data, int length, bool prependCompound = true);

    NBT() :m_NBTBinary(0), m_memory(0) {};
    ~NBT() { delete [] m_NBTBinary; }

private:
    template<typename T>
    	int Read(BYTE *input, T *output)
    {
    	for (DWORD i = 0; i < sizeof(T); i++)
    		((BYTE *)output)[sizeof(T) - 1 - i] = input[i];
    	return sizeof(T);
    }

    int ReadString(BYTE *input, string *output = NULL);
    int ReadArray(BYTE *input, DWORD &length);
    int ReadList(BYTE *input, string baseName, DWORD &itemCount);
    int ReadCompound(BYTE *input, string baseName, bool firstMember=false);

    BYTE *m_NBTBinary;

    map<string, NBTValueDescriptor> m_NBT;

    int m_memory;
};


struct MapChunk
{
	BYTE *blocks;
	BYTE *data;
	BYTE *skyLight;
	BYTE *blockLight;
	BYTE *heightMap;

	long long lastVisited;
	bool modified;
	bool needsLighting;

	int x, z;

	NBT binary;

	//list<SpawnedItem> items;
	//list<TileEntity *> entities;
	//list<BlockProcessor *> processors;
};
