#pragma once
#include <vector>
#include <cstdint>
#include "CommunicationLayer.h"

class GameOfLife
{
public:

	GameOfLife(CommunicationLayer* comms);

	std::vector<uint8_t> GetCurrentState() const;

	void InitializeRandomState();

	void StartSimulation(int rows, int cols, int cellChances);

	void Update();
	size_t GetGridSize() const { return rows * cols; }

private:

	int GetLiveNeighbors(int row, int col) const;
	void ComputeNextState();

	CommunicationLayer* commLayer;

	std::vector<uint8_t> currentState;
	std::vector<uint8_t> nextState;
	int rows;
	int cols;
	int cellChances;

	bool isRunning = false;


};

