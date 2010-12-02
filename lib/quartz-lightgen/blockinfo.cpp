#include "blockinfo.h"

BlockReplaceStat BlockInfo::BlockReplaceInfo[256];
BlockLightStat BlockInfo::BlockLightInfo[256];

void BlockInfo::Init()
{
	memset(BlockReplaceInfo, 0, sizeof(BlockReplaceStat) * 256);
	for (int i = 0; i < 256; i++)
	{
		BlockLightInfo[i].emit = 0;
		BlockLightInfo[i].stop = -16;
	}

	/*BlockReplaceInfo[bt_Air].replaceable = 1;
	BlockReplaceInfo[bt_Water].replaceable = 1;
	BlockReplaceInfo[bt_StillWater].replaceable = 1;
	BlockReplaceInfo[bt_Lava].replaceable = 1;
	BlockReplaceInfo[bt_StillLava].replaceable = 1;*/
	BlockReplaceInfo[bt_Air].replaceable = 1;
	BlockReplaceInfo[bt_Air].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_Sapling].replaceable = 1;
	BlockReplaceInfo[bt_Sapling].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_YellowFlower].replaceable = 1;
	BlockReplaceInfo[bt_YellowFlower].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_RedRose].replaceable = 1;
	BlockReplaceInfo[bt_RedRose].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_BrownMushroom].replaceable = 1;
	BlockReplaceInfo[bt_BrownMushroom].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_RedMushroom].replaceable = 1;
	BlockReplaceInfo[bt_RedMushroom].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_Torch].replaceable = 1;
	BlockReplaceInfo[bt_Torch].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_Fire].replaceable = 1;
	BlockReplaceInfo[bt_Crops].replaceable = 1;
	BlockReplaceInfo[bt_SignPost].replaceable = 1;
	BlockReplaceInfo[bt_SignPost].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_Ladder].replaceable = 1;
	BlockReplaceInfo[bt_Ladder].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_MineTracks].replaceable = 1;
	BlockReplaceInfo[bt_MineTracks].spawnitemonreplace = 1;
	BlockReplaceInfo[bt_RedTorchOn].replaceable = 1;
	BlockReplaceInfo[bt_RedTorchOff].replaceable = 1;
	BlockReplaceInfo[bt_Snow].replaceable = 1;

	BlockLightInfo[bt_Air].stop = 0;
	BlockLightInfo[bt_Sapling].stop = 0;
	BlockLightInfo[bt_Water].stop = -3;
	BlockLightInfo[bt_StillWater].stop = -3;
	BlockLightInfo[bt_Leaves].stop = -3;
	BlockLightInfo[bt_Glass].stop = 0;
	BlockLightInfo[bt_YellowFlower].stop = 0;
	BlockLightInfo[bt_RedRose].stop = 0;
	BlockLightInfo[bt_BrownMushroom].stop = 0;
	BlockLightInfo[bt_RedMushroom].stop = 0;
	BlockLightInfo[bt_Torch].stop = 0;
	BlockLightInfo[bt_MobSpawner].stop = 0;
	BlockLightInfo[bt_RedstoneWire].stop = 0;
	BlockLightInfo[bt_WoodenDoor].stop = 0;
	BlockLightInfo[bt_Ladder].stop = 0;
	BlockLightInfo[bt_MineTracks].stop = 0;
	BlockLightInfo[bt_IronDoor].stop = 0;
	BlockLightInfo[bt_RedTorchOn].stop = 0;
	BlockLightInfo[bt_RedTorchOff].stop = 0;
	BlockLightInfo[bt_Snow].stop = 0;
	BlockLightInfo[bt_Ice].stop = 0;
	BlockLightInfo[bt_Fence].stop = 0;
	BlockLightInfo[bt_Portal].stop = 0;
	BlockLightInfo[bt_JackOLantern].stop = 0;
	BlockLightInfo[bt_SignPost].stop  = 0;
	BlockLightInfo[bt_WallSign].stop = 0;

	BlockLightInfo[bt_Lava].emit = 15;
	BlockLightInfo[bt_StillLava].emit = 15;
	BlockLightInfo[bt_BrownMushroom].emit = 1;
	BlockLightInfo[bt_Torch].emit = 14;
	BlockLightInfo[bt_Fire].emit = 15;
	BlockLightInfo[bt_BurningFurnace].emit = 14;
	BlockLightInfo[bt_GlowingRedstoneOre].emit = 9;
	BlockLightInfo[bt_RedTorchOn].emit = 7;
	BlockLightInfo[bt_Lightstone].emit = 15;
	BlockLightInfo[bt_Portal].emit = 11;
	BlockLightInfo[bt_JackOLantern].emit = 15;
}
