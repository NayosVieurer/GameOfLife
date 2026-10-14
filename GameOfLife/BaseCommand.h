#pragma once

class GameOfLife;

class BaseCommand
{
public:
	virtual void Execute(GameOfLife* gol) = 0;
};

