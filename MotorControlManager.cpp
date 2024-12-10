#include "pch.h"
#include "MotorControlManager.h"
#include "resource.h"

MotorControlManager::MotorControlManager()  {}

MotorControlManager::~MotorControlManager() {
	
}


void MotorControlManager::getCurrentPosition(long positions[3])
{
	for (int i = 0;i < 3;i++)
	{
		int ans = sscGetCurrentCmdPositionFast(board_id, channel, i+1, &positions[i]);
		if (ans != SSC_OK)
			positions[i] = -1;
	}
	
}

void MotorControlManager::getDriveFin(int* fin_status) {
	for (int i = 0; i < 3; ++i) {
		int ans = sscGetDriveFinStatus(board_id, channel, i+1, SSC_FIN_TYPE_SMZ, &fin_status[i]);
		if (ans != SSC_OK) {
			TRACE(_T("sscGetDriveFinStatus failed for axis %d. sscGetLastError=0x%08X\n"), i + 1, sscGetLastError());
			fin_status[i] = -1; // 에러 발생 시 기본값 설정
		}
	}
}



void MotorControlManager::open()
{
	ans = sscOpen(board_id);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscOpen failure. sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
	else
	{
		TRACE(_T("sscOpen success\n"));
	}
}


void MotorControlManager::rebootAndStart()
{
	ans = sscReboot(board_id, channel, SSC_DEFAULT_TIMEOUT);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscReboot failure. sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
	else
	{
		TRACE(_T("sscReboot success\n"));
	}

	ans = sscResetAllParameter(board_id, channel, SSC_DEFAULT_TIMEOUT);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscResetAllParameter failure. sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
	else
	{
		TRACE(_T("sscResetAllParameter success\n"));
	}

	ans = sscLoadAllParameterFromFlashROM(board_id, channel, SSC_DEFAULT_TIMEOUT);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscLoadAllParameterFromFlashROM failure. sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
	else
	{
		TRACE(_T("sscLoadAllParameterFromFlashROM success\n"));
	}

	ans = sscSystemStart(board_id, channel, SSC_DEFAULT_TIMEOUT);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscSystemStart failure. sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
	else
	{
		TRACE(_T("sscSystemStart success\n"));
	}
}


void MotorControlManager::setPoint(PNT_DATA_EX PntData)
{
	axis1.setPoint(PntData);
	axis2.setPoint(PntData);
	//axis3.setPoint(PntData);
}

void MotorControlManager::getPoint()
{
}

void MotorControlManager::servoOn(int axisNum)
{
	switch (axisNum)
	{
	case 1:
		axis1.servoOn();
		break;
	case 2:
		axis2.servoOn();
		break;
	}
}

void MotorControlManager::servoOff(int axisNum)
{
	switch (axisNum)
	{
	case 1:
		axis1.servoOff();
		break;
	case 2:
		axis2.servoOff();
		break;
	}
}

void MotorControlManager::jogMove_Plus(int axisNum)
{
	switch (axisNum)
	{
	case 1:
		axis1.jogMove_Plus();
		break;
	case 2:
		axis2.jogMove_Plus();
		break;
	}
}

void MotorControlManager::jogMove_Minus(int axisNum)
{
	switch (axisNum)
	{
	case 1:
		axis1.jogMove_Minus();
		break;
	case 2:
		axis2.jogMove_Minus();
		break;
	}
}

void MotorControlManager::jogMove(int axisNum, int direction)
{
	switch (axisNum)
	{
	case 1:
		if(direction == 1)
			axis1.jogMove_Minus();
		else if(direction == 0)
			axis1.jogMove_Plus();
		break;
	case 2:
		if (direction == 1)
			axis2.jogMove_Minus();
		else if (direction == 0)
			axis2.jogMove_Plus();
		break;
	}
}

void MotorControlManager::jogStop(int axisNum)
{
	switch (axisNum)
	{
	case 1:
		axis1.jogStop();
		break;
	case 2:
		axis2.jogStop();
		break;
	}
}

void MotorControlManager::customMove(int axisNum, long distance)
{
	switch (axisNum)
	{
	case 1:
		axis1.customMove(distance);
		break;
	case 2:
		axis2.customMove(distance);
		break;
	}
}

void MotorControlManager::positionMove(CString inputText[3])
{
	//PNT_DATA_EX PntData_Origin;
	//for (int i = 0;i < 3;i++)
	//{
	//	long position = _ttol(inputText[i]);
	//	switch (i)
	//	{
	//	case 0:
	//		axis1.getPoint(PntData_Origin);
	//		PntData_Origin.position = position;  // 변환된 position 사용
	//		PntData_Origin.subcmd = SSC_SUBCMD_POS_ABS | SSC_SUBCMD_STOP_SMZ;
	//		axis1.setPoint(PntData_Origin, 1);
	//		break;
	//	case 1:
	//		axis2.getPoint(PntData_Origin);
	//		PntData_Origin.position = position;  // 변환된 position 사용
	//		PntData_Origin.subcmd = SSC_SUBCMD_POS_ABS | SSC_SUBCMD_STOP_SMZ;
	//		axis2.setPoint(PntData_Origin, 1);
	//		break;
	//	/*
	//	case 2:
	//		axis3.getPoint(PntData_Origin);
	//		PntData_Origin.position = position;
	//		PntData_Origin.subcmd = SSC_SUBCMD_POS_INC | SSC_SUBCMD_STOP_SMZ;
	//		axis3.setPoint(PntData_Origin, 1);
	//		break;
	//	*/
	//	default:
	//		break;
	//	}
	//}
	long positions[3];
	long position = 0;
	long moveDistance = 0;
	for (int i = 0;i < 3;i++)
	{
		switch (i)
		{
		case 0:
			ans = sscGetCurrentCmdPositionFast(board_id, channel, i + 1, &positions[i]);
			if (ans != SSC_OK)
				positions[i] = -1;
			position = _ttol(inputText[i]);

			moveDistance = position - positions[i];
			axis1.customMove(moveDistance);
			break;
		case 1:
			ans = sscGetCurrentCmdPositionFast(board_id, channel, i + 1, &positions[i]);
			if (ans != SSC_OK)
				positions[i] = -1;
			position = _ttol(inputText[i]);

			moveDistance = position - positions[i];
			axis2.customMove(moveDistance);
			break;
		//case 2:
		//	int ans = sscGetCurrentCmdPositionFast(board_id, channel, i + 1, &positions[i]);
		//	if (ans != SSC_OK)
		//		positions[i] = -1;
		//	long position = _ttol(inputText[i]);

		//	long moveDistance = positions[i] - position;
		//	axis3.customMove(moveDistance);
		//	break;
		default:
			break;
		}

	}

	
}

void MotorControlManager::emergencyStop()
{
	for (int i = 0;i < 3;i++)
	{
		ans = sscDriveStop(board_id, channel, i + 1, 0);
		if (ans != SSC_OK)
		{
			TRACE(_T("sscDriveStop failure. sscGetLastError=0x%08X\n"), sscGetLastError());
		}
		else
		{
			TRACE(_T("sscDriveStop success\n"));
		}
	}
}

void MotorControlManager::close()
{
	ans = sscClose(board_id);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscClose failure. sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
	else
	{
		TRACE(_T("sscClose success\n"));
	}
}

