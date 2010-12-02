#pragma once

#include <map>
#include <string.h>
#include "common.h"
#include "blocktypes.h"

struct BlockReplaceStat
{
	BYTE replaceable:4;
	BYTE spawnitemonreplace:4;
};

struct BlockLightStat
{
	char emit;
	char stop;
};

#define ISPICKAXE(tool) (tool == it_DiamondPickaxe || tool == it_WoodenPickaxe || tool == it_StonePickaxe || tool == it_GoldPickaxe || tool == it_IronPickaxe)
#define ISHOE(tool) (tool == it_DiamondHoe || tool == it_WoodenHoe || tool == it_StoneHoe || tool == it_GoldHoe || tool == it_IronHoe)

class BlockInfo
{
public:
	static void Init();
	static BlockReplaceStat GetDestructInfo(BYTE blockType) { return BlockReplaceInfo[blockType]; }
	static BlockLightStat GetLightInfo(BYTE blockType) { return BlockLightInfo[blockType]; }
private:
	static BlockReplaceStat BlockReplaceInfo[256];
	static BlockLightStat BlockLightInfo[256];
};
