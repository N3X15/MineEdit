
#include "chunkstore.h"

#define CHUNKSTORE_LOADLOCK 	//pthread_mutex_lock(&m_loadLock)
#define CHUNKSTORE_LOADUNLOCK 	//pthread_mutex_unlock(&m_loadLock)

std::string itoa(int value, int base);

ChunkStore::ChunkStore(const string &mapDirectory)
:m_saving(false), m_mapDirectory(mapDirectory)
{
	//pthread_mutex_init(&m_loadLock, NULL);
}

ChunkStore::~ChunkStore()
{
	//pthread_mutex_destroy(&m_loadLock);
	Cleanup();
}

MapChunk *ChunkStore::GetChunkFromBlockPosition(int x, int z)
{
	return GetMapChunk(x >> 4, z >> 4);
}

list<MapChunk *> ChunkStore::GetSurroundingChunks(int x, int z, int radius)
{
	list<MapChunk *> ret;
	MapChunk *map;

	for (int cx = -radius; cx < radius; cx++)
		for (int cz = -radius; cz < radius; cz++)
		{
			map = GetMapChunk(x + cx, z + cz);
			if (map != NULL)
				ret.push_back(map);
		}

	return ret;
}

MapChunk *ChunkStore::GetMapChunk(int x, int z)
{
	DWORD mapKey = Encode(x, z);

	if (!m_loadedChunks.count(mapKey))
	{
		if (!LoadMapChunk(x, z))
		{
			return NULL;
		}
	}

	m_loadedChunks[mapKey]->lastVisited = tick();

	return m_loadedChunks[mapKey];
}

list<MapChunk *> ChunkStore::GetExpiredChunks()
{
	list<MapChunk *> ret;
	map<DWORD, MapChunk *>::iterator i;

	long long int nowTick = tick();

	for (i = m_loadedChunks.begin(); i != m_loadedChunks.end(); i++)
	{
		if (nowTick - i->second->lastVisited > 60000)
			ret.push_back(i->second);
	}

	if (ret.size())
	{
		//LOG("%i chunks expired\n", (int)ret.size());
	}

	return ret;
}

void ChunkStore::RemoveExpiredChunks(list<MapChunk *> *chunks)
{
	list<MapChunk *>::iterator i;
	//list<TileEntity *>::iterator e;

	for (i = chunks->begin(); i != chunks->end(); i++)
	{
		SaveMapChunk(*i);
		//for (e = (*i)->entities.begin(); e != (*i)->entities.end(); e++)
		//	delete *e;
		m_loadedChunks.erase(Encode((*i)->x, (*i)->z));
		delete (*i);
	}

}

void ChunkStore::SaveModifiedChunks()
{
	map<DWORD, MapChunk *>::iterator m;

	if (!m_saving)
	{

		for (m = m_loadedChunks.begin(); m != m_loadedChunks.end(); m++)
		{
			SaveMapChunk(m->second);
		}

	}
}

void ChunkStore::Cleanup()
{
	map<DWORD, MapChunk *>::iterator m;
	//list<TileEntity *>::iterator e;
	m_saving = true;

	while (m_loadedChunks.size())
	{
		m = m_loadedChunks.begin();
		SaveMapChunk(m->second);

		//for (e = m->second->entities.begin(); e != m->second->entities.end(); e++)
			//delete *e;

		delete m->second;
		m_loadedChunks.erase(m);
	}

	m_loadedChunks.clear();
	m_saving = false;
}


bool ChunkStore::SaveMapChunk(MapChunk *chunk)
{
	string outputFile = GetChunkFileName(chunk->x, chunk->z);

	if (chunk->modified == false)
		return true;

	//printf("Saving chunk %s...\n", outputFile.c_str());

	GenerateChunkLighting(chunk);
	//EntityLoader::SaveTileEntities(outputFile + ".qzt", &chunk->entities);
 	chunk->binary.WriteFile(outputFile + ".dat");
	chunk->modified = false;

	return true;
}

bool ChunkStore::LoadMapChunk(int x, int z)
{
	//static char numeral[3] = "";
	string inputFile = GetChunkFileName(x, z);

	if (m_unloadableChunks.find(Encode(x, z)) != m_unloadableChunks.end())
		return false;

	CHUNKSTORE_LOADLOCK;

	MapChunk *chunk = new MapChunk();

	if (!chunk->binary.LoadFile(inputFile + ".dat"))
	{
		m_unloadableChunks.insert(Encode(x, z));
		delete chunk;
		CHUNKSTORE_LOADUNLOCK;

		return false;
	}

	chunk->blocks = chunk->binary.GetTagDataPointer("_ROOT_Level.Blocks");
	chunk->data = chunk->binary.GetTagDataPointer("_ROOT_Level.Data");
	chunk->blockLight = chunk->binary.GetTagDataPointer("_ROOT_Level.BlockLight");
	chunk->skyLight = chunk->binary.GetTagDataPointer("_ROOT_Level.SkyLight");
	chunk->heightMap = chunk->binary.GetTagDataPointer("_ROOT_Level.HeightMap");

	if (chunk->blocks == NULL || chunk->data == NULL ||
		chunk->blockLight == NULL || chunk->skyLight == NULL ||
		chunk->heightMap == NULL)
	{
		//printf("Chunk %i, %i is invalid!\n", x, z);
		//chunk->binary.Dump();
		m_unloadableChunks.insert(Encode(x, z));
		delete chunk;

		CHUNKSTORE_LOADUNLOCK;
		return false;
	}

	chunk->needsLighting = false;
	chunk->modified = false;
	chunk->x = x;
	chunk->z = z;

	/*if (EntityLoader::EntityFileExists(inputFile + ".qzt"))
	{
		chunk->entities = EntityLoader::LoadTileEntities(inputFile + ".qzt");
	} else
	{
		// load tileentities from map binary
		BYTE *entity = NULL;
		TileEntity *tileEntity = NULL;
		int ex = 0, ey = 0, ez = 0;

		for (int i = 0; ;i++)
		{
			sprintf(numeral, "%i", i);
			entity = chunk->binary.GetCompoundBinaryPointer((string)"Level.TileEntities." + numeral);

			if (entity == NULL)
				break;

			chunk->binary.GetValue((string)"Level.TileEntities." + numeral + ".x", &ex);
			chunk->binary.GetValue((string)"Level.TileEntities." + numeral + ".y", &ey);
			chunk->binary.GetValue((string)"Level.TileEntities." + numeral + ".z", &ez);

			tileEntity = new TileEntity(ex, ey, ez);
			tileEntity->ExtractFromMapNBT(entity);
			chunk->entities.push_back(tileEntity);
		}
	}*/

	m_loadedChunks[Encode(x, z)] = chunk;

	CHUNKSTORE_LOADUNLOCK;
	return true;
}

string ChunkStore::GetChunkFileName(int x, int z)
{
	string returnFile;

	returnFile = m_mapDirectory + base36(x & 0x3F) + "/" + base36(z & 0x3F) + "/c." + base36(x) + "." + base36(z);

	return returnFile;
}

string ChunkStore::base36(int i)
{	
	return itoa(i,36);
}

/**
 * C++ version 0.4 std::string style "itoa":
 * Contributions from Stuart Lowe, Ray-Yuan Sheu,
 * Rodrigo de Salvo Braz, Luc Gallant, John Maloney
 * and Brian Hunt
 */
std::string itoa(int value, int base) 
{
	std::string buf;

	// check that the base if valid
	if (base < 2 || base > 36) return buf;

	enum { kMaxDigits = 35 };
	buf.reserve( kMaxDigits ); // Pre-allocate enough space.

	int quotient = value;

	// Translating number to string with base:
	do {
		buf += "0123456789abcdefghijklmnopqrstuvwxyz"[ std::abs( quotient % base ) ];
		quotient /= base;
	} while ( quotient );

	// Append the negative sign
	if ( value < 0) buf += '-';

	std::reverse( buf.begin(), buf.end() );
	return buf;
}

