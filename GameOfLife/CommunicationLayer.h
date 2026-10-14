#pragma once
#include <windows.h>
#include <thread>
#include <map>
#include <functional>
#include "StartCommand.h"
#include <queue>
#include <mutex>

class GameOfLife;	

class CommunicationLayer
{
public:
	CommunicationLayer();
	~CommunicationLayer();

	void ConnectToPipe();

	void ConnectToStreamingPipe(size_t gridSize);

	void SendData(const void* data, size_t size);

	void Update(GameOfLife* gol);

	bool IsStreamingConnected() const { return isStreamingConnected; }

private:
	HANDLE streamingPipe;
	HANDLE commandsPipe;
	BOOL isStreamingConnected = false;
	BOOL isCommandsConnected = false;
	bool isConnecting = false;

	std::queue<BaseCommand*> commandQueue;
	std::thread commandsReceiverThread;

	void ReceiveCommands();

	std::mutex queueMutex;

	const std::map<uint8_t, std::function<BaseCommand*(const uint8_t*)>> commandHandlers = {
		{ 1, [](const uint8_t* data) { 
			StartCommand* cmd = new StartCommand(data);

			return (BaseCommand*)cmd;
		}},
		{ 2, [](const uint8_t* data) { return nullptr; } },
		// Add more command handlers as needed
	};
};

