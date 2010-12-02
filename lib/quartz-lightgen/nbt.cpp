#include "nbt.h"

int NBT::ReadString(BYTE *input, string *output)
{
	WORD strLen;

	Read(input, &strLen);

	if (output != NULL)
	{
		*output = "";
		for (int i = 0; i < strLen; i++)
			*output += input[i + 2];
	}

	return strLen + 2;
}

int NBT::ReadArray(BYTE *input, DWORD &length)
{
	Read(input, &length);

	return length + 4;
}

int NBT::ReadList(BYTE *input, string baseName, DWORD &itemCount)
{
	int length = 0;
	char ibuff[6];
	BYTE type;

	length += Read(input, &type);
	length += Read(input + length, &itemCount);

	if (itemCount <= 0)
	{
		return length;
	}

	for (DWORD i = 0; i < itemCount; i++)
	{
		NBTValueDescriptor val;
		sprintf(ibuff, "%i", i);
		val.byte = input + length;
		val.Type = type;

		switch(type)
		{
		case Tag_Byte:
			{
				length ++;

				m_NBT[baseName + ibuff] = val;
			}
			break;

		case Tag_Short:
			{
				length += 2;

				m_NBT[baseName + ibuff] = val;
			}
			break;

		case Tag_Int:
			{
				length += 4;

				m_NBT[baseName + ibuff] = val;
			}
			break;

		case Tag_Long:
			{
				length += 8;

				m_NBT[baseName + ibuff] = val;
			}
			break;

		case Tag_Float:
			{
				length += 4;

				m_NBT[baseName + ibuff] = val;
			}
			break;

		case Tag_Double:
			{
				length += 8;

				m_NBT[baseName + ibuff] = val;
			}
			break;

		case Tag_Array:
			{
				length += ReadArray(input + length, val.Length);
				val.byte += 4;
				m_NBT[baseName + ibuff] = val;
			}
			break;

		case Tag_List:
			{
				length += ReadList(input + length, baseName + ibuff + ".", val.Length);

				m_NBT[baseName + ibuff + ".ListTag"] = val;
			}
			break;

		case Tag_Compound:
			{
				length += ReadCompound(input + length, baseName + ibuff + ".");

				m_NBT[baseName + ibuff + ".CompoundTag"] = val;
			}
			break;

		default:
			;
		}
	}

	return length;
}

int NBT::ReadCompound(BYTE *input, string baseName, bool firstMember/* =false */)
{
	BYTE tagType;
	string tagName;
	DWORD length = 0;

	do
	{
		length += Read(input + length, &tagType);

		NBTValueDescriptor value;
		value.Type = tagType;
		value.byte = input + length;

		if (tagType == 0)
			break;

		value.Name.string = (char *)input + length + 2;
		length += ReadString(input + length, &tagName);
		value.Name.length = tagName.length();
		value.byte = input + length;

		switch (tagType)
		{
		case Tag_Byte:
			length++;
			m_NBT[baseName + tagName] = value;
			break;
		case Tag_Short:
			length += 2;
			m_NBT[baseName + tagName] = value;
			break;
		case Tag_Int:
			length += 4;
			m_NBT[baseName + tagName] = value;
			break;
		case Tag_Long:
			length += 8;
			m_NBT[baseName + tagName] = value;
			break;
		case Tag_Float:
			length += 4;
			m_NBT[baseName + tagName] = value;
			break;
		case Tag_Double:
			length += 8;
			m_NBT[baseName + tagName] = value;
			break;
		case Tag_Array:
			{
				length += ReadArray(input + length, value.Length);

				if (value.Length == 0)
				{
					//LOG("arrayLength == 0 (%s)\n", (baseName + tagName).c_str());
				}

				value.byte += 4;
				m_NBT[baseName + tagName] = value;
			}
			break;
		case Tag_String:
			value.Length = ReadString(input + length, NULL) - 2;
			value.byte += 2;
			length += value.Length + 2;
			m_NBT[baseName + tagName] = value;
			break;
		case Tag_List:
			length += ReadList(input + length, baseName + tagName + ".", value.Length);
			value.byte -= (value.Name.length + 3);
			m_NBT[baseName + "ListTag"] = value;
			break;
		case Tag_Compound:
			length += ReadCompound(input + length, baseName + tagName + (firstMember ? "" : "."));
			value.byte -= (value.Name.length + 3);
			m_NBT[baseName + "CompoundTag"] = value;
			break;
		}

		if (firstMember)
			break;

	} while (tagType != Tag_End);


	NBTValueDescriptor value;
	value.Type = Tag_End;
	value.byte = input + length - 1;
	value.Name.length = 0;
	m_NBT[baseName + "CompoundEnd"] = value;

	return length;
}

bool NBT::LoadFromMemory(BYTE *data, int length)
{
	z_stream strm;
	DWORD uncompressedLen = 0;

	uncompressedLen = *(DWORD *)(data + length - 4);

	//LOG("NBT Unc len = %i\n", uncompressedLen);

	m_NBTBinary = new BYTE[uncompressedLen];
	memset(m_NBTBinary, 0, uncompressedLen);
	memset(&strm, 0, sizeof(z_stream));

	strm.next_in = data;
	strm.next_out = m_NBTBinary;
	strm.avail_in = length;
	strm.avail_out = uncompressedLen;

	inflateInit2(&strm, MAX_WBITS + 16);
	int z = inflate(&strm, Z_SYNC_FLUSH);
	inflateEnd(&strm);

	if (z < 0)
	{
		printf("Failed to read gz entity: %s\n", strm.msg);
		return false;
	}

	m_memory = ReadCompound(m_NBTBinary, "", true);

	return true;
}

bool NBT::LoadFromMemoryUncompressed(BYTE *data, int length, bool prependCompound)
{
	if (m_NBTBinary)
		delete [] m_NBTBinary;

	m_NBTBinary = new BYTE[length];

	if (prependCompound)
	{
		m_NBTBinary[0] = Tag_Compound;
		m_NBTBinary[1] = 0;
		m_NBTBinary[2] = 0;
		memcpy(m_NBTBinary + 3, data, length - 3);
	} else
		memcpy(m_NBTBinary, data, length);

	m_memory = ReadCompound(m_NBTBinary, "", true);

	return true;
}

int NBT::CompressToMemory(BYTE *output, int outputLength)
{
	z_stream strm;
	DWORD compressedLen = 0;

	compressedLen = compressBound(m_memory);

	if (outputLength < compressedLen)
	{
		//ERRLOG("Not enough memory allocated for NBT object compression\n");
		return 0;
	}

	memset(&strm, 0, sizeof(z_stream));
	memset(output, 0, outputLength);

	strm.next_in = m_NBTBinary;
	strm.next_out = output;
	strm.avail_in = m_memory;
	strm.avail_out = outputLength;

	int z = 0;
	deflateInit2(&strm, Z_DEFAULT_COMPRESSION, Z_DEFLATED, MAX_WBITS + 16, 8, Z_DEFAULT_STRATEGY);
	z = deflate(&strm, Z_FINISH);
	deflateEnd(&strm);

	if (z < 0)
	{
		printf("Failed to write gz entity: %i %s\n", z, strm.msg);
		return 0;
	}

	return strm.total_out;
}

bool NBT::LoadFile(const string &fileName)
{
	gzFile mapFile;
	FILE *mapFile_;
	DWORD uncompressedLen = 0;

	// get file length
	mapFile_ = fopen(fileName.c_str(), "rb");
	if (mapFile_ == NULL)
	{
		printf("Couldn't open file %s...\n", fileName.c_str());
		return false;
	}
	fseek(mapFile_, -4, SEEK_END);
	if (fread(&uncompressedLen, 4, 1, mapFile_) != 1)
	{
		printf("Error in NBT: couldn't read NBT file size for %s\n", fileName.c_str());
	}
	fclose(mapFile_);

	if (uncompressedLen == 0)
	{
		return false;
	}

	if (m_NBTBinary)
		delete [] m_NBTBinary;

	mapFile = gzopen(fileName.c_str(), "rb");
	m_NBTBinary = new BYTE[uncompressedLen];
	memset(m_NBTBinary, 0, uncompressedLen);
	uncompressedLen = gzread(mapFile, (char *)m_NBTBinary, uncompressedLen);
	gzclose(mapFile);

	m_memory = ReadCompound(m_NBTBinary, "", true);

	return true;
}

bool NBT::WriteFile(const string &fileName)
{
	gzFile mapFile;

	mapFile = gzopen(fileName.c_str(), "wb");
	if (mapFile == NULL)
	{
		printf("Couldn't open file %s...\n", fileName.c_str());
		return false;
	}

	gzwrite(mapFile, m_NBTBinary, m_memory);
	gzclose(mapFile);

	return true;
}

void NBT::Dump()
{
	char buf[255] = {0};

	for (map<string, NBTValueDescriptor>::iterator i = m_NBT.begin(); i != m_NBT.end(); i++)
	{
		switch (i->second.Type)
		{
		case Tag_Byte:
			printf("%s = %i\n", i->first.c_str(), *i->second.byte);
			break;
		case Tag_Short:
			printf("%s = %i\n", i->first.c_str(), *i->second.word);
			break;
		case Tag_Int:
			printf("%s = %i\n", i->first.c_str(), *i->second.dword);
			break;
		case Tag_Long:
			printf("%s = %lli\n", i->first.c_str(), *i->second.qword);
			break;
		case Tag_String:
			strncpy(buf, i->second.str, i->second.Length);
			printf("%s = %s\n", i->first.c_str(), buf);
			break;
		case Tag_Float:
			printf("%s = %4.2f\n", i->first.c_str(), *i->second.fl);
			break;
		case Tag_Double:
			printf("%s = %4.2f\n", i->first.c_str(), *i->second.db);
			break;
		default:
			printf("%s\n", i->first.c_str());
		}

	}

	printf("mem = %i\n", m_memory);
}

BYTE *NBT::GetCompoundBinaryPointer(const string &tagName)
{
	if (m_NBT.count(tagName + ".CompoundTag") != 0)
		return m_NBT[tagName + ".CompoundTag"].byte;

	return NULL;
}

BYTE *NBT::GetTagDataPointer(const string &tagName)
{
	if (m_NBT.count(tagName) != 0)
		return m_NBT[tagName].arr;

	return NULL;
}
