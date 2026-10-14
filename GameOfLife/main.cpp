#include <iostream>
#include <chrono>
#include <thread>
#include "GameOfLife.h"
#include "CommunicationLayer.h"

using namespace std;

const int TARGET_FPS = 60;
const std::chrono::milliseconds FRAME_DURATION = std::chrono::milliseconds(1000 / TARGET_FPS);

int main() 
{
	CommunicationLayer* commLayer = new CommunicationLayer();
	commLayer->ConnectToPipe();

	GameOfLife* GoL = new GameOfLife(commLayer);

	while (true)
	{
		auto start = std::chrono::high_resolution_clock::now();

		commLayer->Update(GoL);
		GoL->Update();

		auto end = std::chrono::high_resolution_clock::now();
		auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(end - start);

		if (elapsed < FRAME_DURATION)
		{
			std::this_thread::sleep_for(FRAME_DURATION - elapsed);
		}
	}


	return 0;
}	