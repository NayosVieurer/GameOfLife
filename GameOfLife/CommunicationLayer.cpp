#include "CommunicationLayer.h"
#include <thread>

CommunicationLayer::CommunicationLayer(size_t gridSize)
{
	pipe = CreateNamedPipe(
		L"\\\\.\\pipe\\GameOfLifePipe", 
		PIPE_ACCESS_OUTBOUND, 
		PIPE_TYPE_MESSAGE | PIPE_WAIT, 
		1, 
		gridSize,
		0, 
		NMPWAIT_USE_DEFAULT_WAIT, 
		NULL);
}

CommunicationLayer::~CommunicationLayer()
{
}

void CommunicationLayer::ConnectToPipe()
{
	if(isConnected || isConnecting)
		return;

	isConnecting = true;

	std::thread connectThread([this]() {
		BOOL success = ConnectNamedPipe(pipe, NULL);

		if(success || GetLastError() == ERROR_PIPE_CONNECTED)
		{
			isConnected = true;
		}
		else
		{
			isConnected = false;
		}

		isConnecting = false;
	});

	connectThread.detach();
}

void CommunicationLayer::SendData(const void* data, size_t size)
{
	if (!isConnected)
		return;

	DWORD bytesWritten;
	BOOL success = WriteFile(pipe, data, size, &bytesWritten, NULL);

	if (!success)
	{
		isConnected = false;
		DisconnectNamedPipe(pipe);
		ConnectToPipe();
	}
}

