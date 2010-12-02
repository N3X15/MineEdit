#include "chunkstore.h"

/**
 *
   Copyright (c) 2010, The Mineserver Project
   All rights reserved.

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
 * Neither the name of the The Mineserver Project nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
   ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
   WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
   DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
   DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
   LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
   ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 *
 * basis of lightgen code from:
 * 		https://github.com/fador/mineserver/blob/master/src/map.cpp#L459
 * 		thanks @fador
 */

void ChunkStore::SetBlockLight(int x, int y, int z, BYTE blockLight, BYTE skyLight, bool block, bool sky)
{
	int index, cbz, cbx;
	BYTE skyLight_, blockLight_;
	MapChunk *chunk;

	if (y < 0 || y > 127)
	{
		return;
	}

	chunk = GetChunkFromBlockPosition(x, z);
	if (!chunk)
		return;

	cbx = (x - ((x >> 4) * 16));
	cbz = (z - ((z >> 4) * 16));

	index = y + (cbz * 128) + (cbx * 128 * 16);
	skyLight_ = chunk->skyLight[index >> 1];
	blockLight_ = chunk->blockLight[index >> 1];

	if (y % 2)
	{
		if (sky)
		{
			skyLight_ &= 0x0f;
			skyLight_ |= skyLight << 4;
		}
		if (block)
		{
			blockLight_ &= 0xf;
			blockLight_ |= blockLight << 4;
		}
	} else
	{
		if (sky)
		{
			skyLight_ &= 0xf0;
			skyLight_ |= skyLight;
		}
		if (block)
		{
			blockLight_ &= 0xf0;
			blockLight_ |= blockLight;
		}
	}

	if (sky)
	{
		chunk->skyLight[index >> 1] = skyLight_;
	}
	if (block)
	{
		chunk->blockLight[index >> 1] = blockLight_;
	}

	chunk->modified = true;
}

void ChunkStore::GenerateChunkLighting(MapChunk *chunk)
{
	int index, wx, wz;
	BYTE *sky, *light, *blocks;
	BYTE blockType;

	if (!chunk->needsLighting)
		return;

	long long tickNow = tick();

	memset(chunk->blockLight, 0, 16 * 16 * 128 / 2);
	memset(chunk->skyLight, 0, 16 * 16 * 128 / 2);

	sky = chunk->skyLight;
	light = chunk->blockLight;
	blocks = chunk->blocks;

	// sky light
	for (int x = 0; x < 16; x++)
	{
		for (int z = 0; z < 16; z++)
		{
			for (int y = 127; y >= 0; y--)
			{
				wx = chunk->x * 16 + x;
				wz = chunk->z * 16 + z;
				index = y + (z * 128) + (x * 128 * 16);
				blockType = blocks[index];

				SetBlockLight(wz, y, wx, 0, 15, false, true);

				if (BlockInfo::GetLightInfo(blockType).stop != 0)
					break;
			}
		}
	}

	printf("Chunk lighting gen (sky) in %llu ms\n", tick() - tickNow);

	tickNow = tick();

	// light spread
	for (int x = 0; x < 16; x++)
	{
		for (int z = 0; z < 16; z++)
		{
			wx = chunk->x * 16 + x;
			wz = chunk->z * 16 + z;
			for (int y = GetHighestSurroundingBlock(wx, wz); y >= 0; y--)
			{
				index = y + (z * 128) + (x * 128 * 16);
				blockType = blocks[index];

				// stops light going down
				if (BlockInfo::GetLightInfo(blockType).stop == -16)
				{
					SetBlockLight(wx, y, wz, 0, 0, false, true);
					break;
				} else
				{
					SetBlockLight(wx, y, wz, 0, 15 + BlockInfo::GetLightInfo(blockType).stop, false, true);
					LightMapStep(wx, y, wz, 15 + BlockInfo::GetLightInfo(blockType).stop);
				}
			}
		}
	}

	printf("Chunk lighting gen (spread) in %llu ms\n", tick() - tickNow);

	tickNow = tick();

	// light emitters
	for (int x = 0; x < 16; x++)
	{
		for (int z = 0; z < 16; z++)
		{
			for (int y = chunk->heightMap[z * 16 + x] + 2; y >= 0; y--)
			{
				index = y + (z * 128) + (x * 128 * 16);
				blockType = blocks[index];
				if (BlockInfo::GetLightInfo(blockType).emit != 0)
				{
					wx = chunk->x * 16 + x;
					wz = chunk->z * 16 + z;
					BlockLightMapStep(wx, y, wz, BlockInfo::GetLightInfo(blockType).emit);
				}
			}
		}
	}

	chunk->needsLighting = false;
	printf("Chunk lighting gen (emit) in %llu ms\n", tick() - tickNow);
}


void ChunkStore::BlockLightMapStep(int x, int y, int z, int light)
{
	BYTE blockType, metaData, blockLight, skyLight;
	int x_, y_, z_;

	//DBG(L"BLMS: %i,%i,%i,%i,%p\n", x, y, z, light, chunk);

	if (light < 1)
		return;

	for (BYTE i = 0; i < 6; i++)
	{
		if (y == 127 && i == 2)
			i++;
		if (y == 0 && i == 3)
			i++;

		x_ = x;
		y_ = y;
		z_ = z;

		switch (i)
		{
		case 0: x_++; break;
		case 1: x_--; break;
		case 2: y_++; break;
		case 3: y_--; break;
		case 4: z_++; break;
		case 5: z_--; break;
		}

		if (GetBlock(x_, y_, z_, blockType, metaData, blockLight, skyLight))
		{
			if (blockLight < (light + BlockInfo::GetLightInfo(blockType).stop - 1))
			{
				SetBlockLight(x_, y_, z_, light + BlockInfo::GetLightInfo(blockType).stop - 1, 0, true, false);
				if (BlockInfo::GetLightInfo(blockType).stop != -16)
				{
					BlockLightMapStep(x_, y_, z_, light + BlockInfo::GetLightInfo(blockType).stop - 1);
				}
			}
		}
	}
}

void ChunkStore::LightMapStep(int x, int y, int z, int light)
{
	BYTE blockType, metaData, blockLight, skyLight;
	int x_, y_, z_;

	if (light < 1)
	{
		return;
	}

	// loop 1 time for each direction except up (positive y)
	for (BYTE i = 0; i < 5; i++)
	{
		if (y == 127 && i == 2)
			i++;
		if (y == 0 && i == 3)
			i++;

		x_ = x;
		y_ = y;
		z_ = z;

		// light spread direction
		switch (i)
		{
		case 0: x_++; break;
		case 1: x_--; break;
		case 2: y_--; break;
		case 3: z_++; break;
		case 4: z_--; break;
		}

		if (GetBlock(x_, y_, z_, blockType, metaData, blockLight, skyLight))
		{
			if (skyLight < (light + BlockInfo::GetLightInfo(blockType).stop - 1))
			{
				SetBlockLight(x_, y_, z_, 0, light + BlockInfo::GetLightInfo(blockType).stop - 1, false, true);
				if (BlockInfo::GetLightInfo(blockType).stop != -16)	// stop if this block lets no light through
					LightMapStep(x_, y_, z_, light + BlockInfo::GetLightInfo(blockType).stop - 1);
			}
		}
	}
}

bool ChunkStore::GetBlock(int x, int y, int z, BYTE &blockType, BYTE &metaData, BYTE &blockLight, BYTE &skyLight)
{
	int index;
	int cbx, cbz;
	MapChunk *chunk;

	if (y < 0 || y > 127)
	{
		return false;
	}

	chunk = GetChunkFromBlockPosition(x, z);
	if (!chunk)
		return false;

	cbx = (x - ((x >> 4) * 16));
	cbz = (z - ((z >> 4) * 16));

	index = y + (cbz * 128) + (cbx * 128 * 16);

	if (chunk->blockLight == NULL ||
		chunk->skyLight == NULL)
	{
		printf("Somehow blockLight/skyLight NULL for %i %i chunk\n", x >> 4, z >> 4);
		return false;
	}

	blockType = chunk->blocks[index];
	metaData = chunk->data[index >> 1];
	blockLight = chunk->blockLight[index >> 1];
	skyLight = chunk->skyLight[index >> 1];

	if (y % 2)
	{
		metaData &= 0xf0;
		metaData >>= 4;
		blockLight &= 0xf0;
		blockLight >>= 4;
		skyLight &= 0xf0;
		skyLight >>= 4;
	} else
	{
		metaData &= 0x0f;
		blockLight &= 0x0f;
		skyLight &= 0x0f;
	}

	return true;
}

BYTE ChunkStore::GetHighestSurroundingBlock(int x, int z)
{
	int _x, _z;
	BYTE heighest = 0;

	for (int i = 0; i < 5; i++)
	{
		_x = x; _z = z;
		switch (i)
		{
		case 1: _x--; break;
		case 2: _x++; break;
		case 3: _z--; break;
		case 4: _z++; break;
		}

		BYTE h = GetHighestBlockAtPoint(_x, _z);
		if (h > heighest)
			heighest = h;

	}

	return heighest;
}

BYTE ChunkStore::GetHighestBlockAtPoint(int x, int z)
{
	MapChunk *chunk = GetChunkFromBlockPosition(x, z);
	if (!chunk)
		return 0;

	int cbx = (x - ((x >> 4) * 16));
	int cbz = (z - ((z >> 4) * 16));

	return chunk->heightMap[cbz * 16 + cbx];
}
