#pragma once
#include "Axis1.h"
#include "Axis2.h"
class MotorControlManager
{
private:
	Axis1 axis1;
	Axis2 axis2;
	int board_id = 0;
	int channel = 1;
	int ans;
	CWnd* m_pDlgWnd = nullptr; // UI 핸들
public:
	MotorControlManager();  // 생성자 선언
	~MotorControlManager(); // 소멸자 선언

	void getCurrentPosition(long positions[3]);
	void getDriveFin(int* fin_status);

	void setDialogWnd(CWnd* pDlgWnd); // UI 핸들 설정
	void postMsg(CString Message);

	void open();
	void rebootAndStart();
	void setPoint(PNT_DATA_EX PntData);
	void getPoint();
	void servoOn(int axisNum);
	void servoOff(int axisNum);
	void jogMove_Plus(int axisNum);
	void jogMove_Minus(int axisNum);
	void jogMove(int axisNum, int direction);
	void jogStop(int axisNum);
	void customMove(int axisNum, long distance);
	void positionMove(CString inputText[3]);
	void homeReturn();
	void emergencyStop();
	void close();
};

