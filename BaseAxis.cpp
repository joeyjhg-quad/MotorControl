#include "pch.h"
#include "BaseAxis.h"

#define DRIVE_FIN_TIMEOUT				(10000)
#define INTERRUPT_THREAD_PRIORITY		THREAD_PRIORITY_TIME_CRITICAL

void BaseAxis::setPoint(PNT_DATA_EX PntData, int ptnnum)
{
	//data = PntData;
	ans = sscSetPointDataEx(board_id, channel, axisNumber, ptnnum, &PntData);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscSetPointDataEx failure. axnum=%d, sscGetLastError=0x%08X\n"), 1, sscGetLastError());
		return;
	}
}

void BaseAxis::getPoint(PNT_DATA_EX& PntData, int ptnnum)
{
	ans = sscCheckPointDataEx(board_id, channel, axisNumber, ptnnum, &PntData);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscCheckPointDataEx failure. axnum=%d, sscGetLastError=0x%08X\n"), 1, sscGetLastError());
		return;
	}
}

void BaseAxis::servoOn()
{
	ans = sscSetCommandBitSignalEx(board_id, channel, axisNumber, SSC_CMDBIT_AX_SON, SSC_BIT_ON);

}

void BaseAxis::servoOff()
{
	ans = sscSetCommandBitSignalEx(board_id, channel, axisNumber, SSC_CMDBIT_AX_SON, SSC_BIT_OFF);
}

void BaseAxis::jogMove_Plus() {
	ans = sscCheckPointDataEx(board_id, channel, axisNumber, 0, &data);
	ans = sscJogStart(board_id, channel, axisNumber, data.speed, data.actime, data.dctime, SSC_DIR_PLUS);
	Sleep(50);
	if (ans != SSC_OK)
	{
		printf("sscJogStart failure. sscGetLastError=0x%08X\n", sscGetLastError());
		checkAlarm();
		return;
	}
	else
	{
		checkAlarm();
		printf("sscJogStart success\n");

	}

 }

void BaseAxis::jogMove_Minus()
{
	ans = sscCheckPointDataEx(board_id, channel, axisNumber, 0, &data);
	ans = sscJogStart(board_id, channel, axisNumber, data.speed, data.actime, data.dctime, SSC_DIR_MINUS);
	Sleep(50);
	if (ans != SSC_OK)
	{
		printf("sscJogStart failure. sscGetLastError=0x%08X\n", sscGetLastError());
		checkAlarm();
		return;
	}
	else
	{
		checkAlarm();
		printf("sscJogStart success\n");

	}
}
//void BaseAxis::jogMove(int boardNum, int speed, int direction) {
//    //test
//    ans = sscJogStart(boardNum, 1, axisNumber, 1000, 3000, 3000, SSC_DIR_PLUS);
//}
void BaseAxis::jogStop()
{
	ans = sscJogStop(board_id, channel, axisNumber);
}

void BaseAxis::customMove(long distance)
{
	ans = sscCheckPointDataEx(board_id, channel, axisNumber, 0, &data);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscCheckPointDataEx failure.  sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
	ans = sscIncStart(board_id, channel, axisNumber, distance, data.speed, data.actime, data.dctime);
	Sleep(50);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscIncStart failure.  sscGetLastError=0x%08X\n"), sscGetLastError());
		checkAlarm();
		//return;
	}
	else
		checkAlarm();


}

void BaseAxis::positionMove(int point_s, int point_e)
{
	ans = sscLinearStart(board_id, channel, axisNumber,1, point_s, point_e);
	if (ans != SSC_OK)
	{
		TRACE(_T("positionMove failure.  sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
}

void BaseAxis::checkAlarm()
{
	unsigned short code = 0;
	unsigned short detail_code = 0;
	ans = sscGetAlarm(board_id, channel, axisNumber, SSC_ALARM_OPERATION, &code, &detail_code);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscCheckPointDataEx failure.  sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
		// 알람 코드와 상세 코드가 반환되면 출력
	}
	else
	{
		TRACE(_T("sscGetAlarm success.  code=%u\n", code));
	}

	if (code == 0)
		return;
	else
	{
		switch (code)
		{
		case 178:	// survo off
			TRACE(_T("서보 ON으로 바꿔주십시오.\n"));
			ans = sscResetAlarm(board_id, channel, axisNumber, SSC_ALARM_OPERATION);
			break;
		default:
			break;
		}
	}
}
