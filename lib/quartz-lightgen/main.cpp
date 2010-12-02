#include <stdio.h>
#include <string>
#include <fstream>
#include <vector>

#include "chunkstore.h"
#include "blockinfo.h"


void relightChunk(ChunkStore store, int x, int z);

int main(int argc, char *argv[])
{
	BlockInfo::Init();
	
	char usage[] = "usage: quartz_light mapdir chunkx chunky\n eg: quartz_light ~/world/ 0 0\n";

	if (argc == 3)
	{
		//ChunkStore store(argv[1]);
		ifstream ifs( argv[2] );

		int numchunks;
		std::string temp;
		while( getline( ifs, temp ) )
		{
			int x,z;
			sscanf(temp.c_str(),"%d,%d",&x,&z);
			ChunkStore store(argv[1]); // I have a sneaking suspicion that loading this out of scope causes lag.
			relightChunk(store,x,z);
			store.SaveModifiedChunks();
		}
		//store.SaveModifiedChunks();
		return 0;
	}
	else if (argc == 4)
	{
		ChunkStore store(argv[1]);
		relightChunk(store, atoi(argv[2]), atoi(argv[3]));
		return 0;
	}
	printf("%s\n\n Only %d arguments were supplied.\n",usage,argc);
	for(int i = 0; i < argc; i++)
		printf("[%d] %s\n",i,argv[i]);
		
	getchar();
	return -1;
}

void relightChunk(ChunkStore store, int x, int z)
{
	MapChunk *chunk;
	if (!(chunk = store.GetMapChunk(x, z)))
		return;
	printf("\nRegenerating light for chunk (%d,%d)...\n",x,z);
	chunk->needsLighting = true;
	store.GenerateChunkLighting(chunk);	
}