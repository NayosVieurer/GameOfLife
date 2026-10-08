#pragma once
#include <windows.h>
class CommunicationLayer
{
public:
	CommunicationLayer(size_t gridSize);
	~CommunicationLayer();

	void ConnectToPipe();

	void SendData(const void* data, size_t size);

private:
	HANDLE pipe;
	BOOL isConnected = false;
	bool isConnecting = false;
};

