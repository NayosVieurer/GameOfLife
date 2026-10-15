#include "StartCommand.h"
#include "GameOfLife.h"
#include <cstring>

StartCommand::StartCommand(const uint8_t* data)
{
	std::memcpy(&rows, data, sizeof(int));
	std::memcpy(&cols, data + sizeof(int), sizeof(int));
	std::memcpy(&cellChances, data + sizeof(int) * 2, sizeof(int));
}

void StartCommand::Execute(GameOfLife* gol)
{
	gol->StartSimulation(rows, cols, cellChances);
}

