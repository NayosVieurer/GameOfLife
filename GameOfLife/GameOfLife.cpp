#include "GameOfLife.h"
#include <random>
#include <iostream>

GameOfLife::GameOfLife(CommunicationLayer* comms)
{
	commLayer = comms;
}

std::vector<uint8_t> GameOfLife::GetCurrentState() const
{
	return currentState;
}

void GameOfLife::InitializeRandomState()
{
	std::random_device rd;
	std::mt19937 gen(rd());
	std::uniform_int_distribution<> dis(0, 99);

	for(size_t i = 0; i < GetGridSize(); ++i)
	{
		currentState[i] = (dis(gen) < cellChances) ? 255 : 0; 
	}
}

void GameOfLife::StartSimulation(int rows, int cols, int cellChances)
{
	isRunning = false;

	this->rows = rows;
	this->cols = cols;
	this->cellChances = cellChances;

	currentState.resize(rows * cols, false);
	nextState.resize(rows * cols, false);

	InitializeRandomState();

	std::thread startThread([this]() {


		while(!commLayer->IsStreamingConnected())
		{
			commLayer->ConnectToStreamingPipe(GetGridSize());
			std::this_thread::sleep_for(std::chrono::milliseconds(1000));

		}

		isRunning = true;
	});

	startThread.detach();
}

void GameOfLife::Update()
{
	if(!isRunning)
		return;

	commLayer->SendData(currentState.data(), GetGridSize());
	ComputeNextState();	
}

int GameOfLife::GetLiveNeighbors(int row, int col) const
{
	int liveNeighbors = 0;
	for (int x = -1; x <= 1; ++x)
	{
		for (int y = -1; y <= 1; ++y)
		{
			if (x == 0 && y == 0) continue;
			int ni = row + x;
			int nj = col + y;
			if (ni >= 0 && ni < rows && nj >= 0 && nj < cols)
			{
				if (currentState[ni * cols + nj] == 255)
				liveNeighbors++;
			}
		}
	}
	return liveNeighbors;
}

void GameOfLife::ComputeNextState()
{
	std::fill(nextState.begin(), nextState.end(), 0);

	for (int i = 0; i < rows; ++i)
	{
		for (int j = 0; j < cols; ++j)
		{
			int aliveNeighbors = GetLiveNeighbors(i, j);
			if (currentState[i * cols + j] == 255)
			{
				nextState[i * cols + j] = aliveNeighbors == 2 || aliveNeighbors == 3 ? 255 : 0;
			}
			else
			{
				nextState[i * cols + j] = aliveNeighbors == 3 ? 255 : 0;
			}
		}
	}

	int aliveCount = 0;
	for (auto cell : currentState) {
		if (cell == 255) aliveCount++;
	}
	std::cout << "Cellules vivantes au tick actuel : " << aliveCount << std::endl;


	currentState.swap(nextState);
}
