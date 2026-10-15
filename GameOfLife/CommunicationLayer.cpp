#include "CommunicationLayer.h"
#include <iostream>


CommunicationLayer::CommunicationLayer()
{


	commandsPipe = CreateNamedPipe(
		TEXT("\\\\.\\pipe\\GameOfLifeCommands"), 
		PIPE_ACCESS_INBOUND, 
		PIPE_TYPE_MESSAGE | PIPE_WAIT, 
		1, 
		1024,
		0, 
		NMPWAIT_USE_DEFAULT_WAIT, 
		NULL);
}

CommunicationLayer::~CommunicationLayer()
{
}

void CommunicationLayer::ConnectToPipe()
{
	isConnecting = true;

	std::thread connectThread([this]() {

		if (!isStreamingConnected)
		{
			isStreamingConnected = ConnectNamedPipe(streamingPipe, NULL) ? TRUE : (GetLastError() == ERROR_PIPE_CONNECTED);
		}

		if(!isCommandsConnected)
		{
			isCommandsConnected = ConnectNamedPipe(commandsPipe, NULL) ? TRUE : (GetLastError() == ERROR_PIPE_CONNECTED);

			if (isCommandsConnected)
			{
				commandsReceiverThread = std::thread(&CommunicationLayer::ReceiveCommands, this);
				commandsReceiverThread.detach();
			}
		}

		isConnecting = false;
	});

	connectThread.detach();
}

void CommunicationLayer::ConnectToStreamingPipe(size_t gridSize)
{
	streamingPipe = CreateNamedPipe(
		TEXT("\\\\.\\pipe\\GameOfLifeStream"),
		PIPE_ACCESS_OUTBOUND,
		PIPE_TYPE_MESSAGE | PIPE_WAIT,
		1,
		gridSize,
		0,
		NMPWAIT_USE_DEFAULT_WAIT,
		NULL);

	ConnectToPipe();

}

void CommunicationLayer::SendData(const void* data, size_t size)
{
	if (!isStreamingConnected)
		return;

	DWORD bytesWritten;
	BOOL success = WriteFile(streamingPipe, data, size, &bytesWritten, NULL);

	if (!success && isStreamingConnected)
	{
		isStreamingConnected = false;
		DisconnectNamedPipe(streamingPipe);
		streamingPipe = NULL;
	}
}

void CommunicationLayer::Update(GameOfLife* gol)
{
	std::queue<BaseCommand*> commandsToExecute;
	{
		std::lock_guard<std::mutex> lock(queueMutex);
		commandsToExecute = std::move(commandQueue);
	}

	while (!commandsToExecute.empty())
	{
		auto command = commandsToExecute.front();
		commandsToExecute.pop();

		command->Execute(gol);
	}
}

void CommunicationLayer::ReceiveCommands()
{
	uint8_t buffer[64];
	DWORD bytesRead = 0;
	while(true)
	{
		if (!isCommandsConnected)
			break;
		BOOL success = ReadFile(commandsPipe, buffer, sizeof(buffer), &bytesRead, NULL);
		if (!success || bytesRead == 0)
		{
			isCommandsConnected = false;
			DisconnectNamedPipe(commandsPipe);
			ConnectToPipe();
			break;
		}

		auto it = commandHandlers.find(buffer[0]);

		if(it != commandHandlers.end())
		{
			auto cmd = it->second(buffer + 2);

			if(cmd)
			{
				std::lock_guard<std::mutex> lock(queueMutex);	
				commandQueue.push(cmd);
			}
		}
		else
		{
			std::cout << "Unknown command received: " << (int)buffer[0] << std::endl;
		}
	}
}

