#include <iostream>
#include <chrono>
#include <thread>
#include "GameOfLife.h"
#include "CommunicationLayer.h"

using namespace std;

const int TARGET_FPS = 60;
const auto FRAME_DURATION = std::chrono::milliseconds(1000 / TARGET_FPS);

int main() 
{
	GameOfLife* GoL = new GameOfLife();
	GoL->InitializeRandomState();

	CommunicationLayer* commLayer = new CommunicationLayer(GoL->GetGridSize());
	commLayer->ConnectToPipe();

	while(true)
	{         
		auto start = std::chrono::high_resolution_clock::now();

		commLayer->SendData(GoL->GetCurrentState().data(), GoL->GetGridSize());
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