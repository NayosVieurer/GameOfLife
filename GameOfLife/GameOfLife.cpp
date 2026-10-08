#include "GameOfLife.h"
#include <random>
#include <iostream>

GameOfLife::GameOfLife()
{
	currentState.resize(ROWS * COLS, false);
	nextState.resize(ROWS * COLS, false);
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
		currentState[i] = (dis(gen) < 25) ? 255 : 0; 
	}
}

void GameOfLife::Update()
{
	std::fill(nextState.begin(), nextState.end(), 0);

	for(int i = 0; i < ROWS; ++i)
	{
		for(int j = 0; j < COLS; ++j)
		{
			int aliveNeighbors = GetLiveNeighbors(i, j);
			if(currentState[i * COLS + j] == 255)
			{
				nextState[i * COLS + j] = aliveNeighbors == 2 || aliveNeighbors == 3 ? 255 : 0;
			}
			else
			{
				nextState[i * COLS + j] = aliveNeighbors == 3 ? 255 : 0;
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
			if (ni >= 0 && ni < ROWS && nj >= 0 && nj < COLS)
			{
				if (currentState[ni * COLS + nj] == 255)
				liveNeighbors++;
			}
		}
	}
	return liveNeighbors;
}
