#pragma once

#include <sys/stat.h>
#include <list>
#include <map>
#include <set>
#include <stdlib.h>

#include "common.h"
#include "nbt.h"
#include "blockinfo.h"

#define Encode(x, z) ((x << 16) | (z & 0xffff))

class ChunkStore
{
public:
	ChunkStore(const string &mapDirectory);
	~ChunkStore();

	MapChunk *GetChunkFromBlockPosition(int x, int z);
	// below are in map chunk coordinates
	list<MapChunk *> GetSurroundingChunks(int x, int z, int chunkRadius);
	MapChunk *GetMapChunk(int x, int z);

	list<MapChunk *> GetExpiredChunks();
	void RemoveExpiredChunks(list<MapChunk *> *chunks);
	void SaveModifiedChunks();
	void Cleanup();

	void GenerateChunkLighting(MapChunk *chunk);
private:
	bool SaveMapChunk(MapChunk *chunk);
	bool LoadMapChunk(int x, int z);
	string GetChunkFileName(int x, int z);
	string base36(int val);

	//pthread_mutex_t m_loadLock;
	bool m_saving;
	map<DWORD, MapChunk *> m_loadedChunks;
	set<DWORD> m_unloadableChunks;
	string m_mapDirectory;

	void BlockLightMapStep(int x, int y, int z, int light);
	void LightMapStep(int x, int y, int z, int light);
	void SetBlockLight(int x, int y, int z, BYTE blockLight, BYTE skyLight, bool block, bool sky);
	bool GetBlock(int x, int y, int z, BYTE &blockType, BYTE &metaData, BYTE &blockLight, BYTE &skyLight);
	BYTE GetHighestBlockAtPoint(int x, int z);
	BYTE GetHighestSurroundingBlock(int x, int z);
};
