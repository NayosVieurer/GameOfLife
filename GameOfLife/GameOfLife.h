#pragma once
#include <vector>
class GameOfLife
{
public:

	GameOfLife();

	std::vector<uint8_t> GetCurrentState() const;

	void InitializeRandomState();

	void Update();
	size_t GetGridSize() const { return ROWS * COLS; }

private:

	int GetLiveNeighbors(int row, int col) const;

	std::vector<uint8_t> currentState;
	std::vector<uint8_t> nextState;
	const int ROWS = 512;
	const int COLS = 512;

};

