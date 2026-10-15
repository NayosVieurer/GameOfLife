#pragma once
#include "BaseCommand.h"
#include <cstdint>
class StartCommand : BaseCommand
{
public:


	StartCommand(const uint8_t* data);

	

	void Execute(GameOfLife* gol) override;

private:

	int rows;
	int cols;
	int cellChances;
};


