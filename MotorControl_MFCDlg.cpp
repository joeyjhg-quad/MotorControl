
// MotorControl_MFCDlg.cpp: 구현 파일
//

#include "pch.h"
#include "framework.h"
#include "MotorControl_MFC.h"
#include "MotorControl_MFCDlg.h"
#include "afxdialogex.h"
//#include <mc2xxstd.h>
//#include "mc2xxstd.h"

#ifdef _DEBUG

#define new DEBUG_NEW
#define CHG_CTRL_MODE_AUTO				(0)

#endif


// CMotorControlMFCDlg 대화 상자



CMotorControlMFCDlg::CMotorControlMFCDlg(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_MOTORCONTROL_MFC_DIALOG, pParent),
	m_btnJogXPlus(1, 0),  // X+ 축: 0, 방향: +
	m_btnJogXMinus(1, 1),// X- 축: 0, 방향: -
	m_btnJogYPlus(2, 0),  // Y+ 축: 1, 방향: +
	m_btnJogYMinus(2, 1),// Y- 축: 1, 방향: -
	m_btnJogZPlus(3, 0),  // Z+ 축: 2, 방향: +
	m_btnJogZMinus(3, 1) // Z- 축: 2, 방향: -
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CMotorControlMFCDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TEXT_STATE_CONNECT, m_ConnectState);
	DDX_Control(pDX, IDC_TEXT_STATE_SERVO1, m_Servo1State);
	DDX_Control(pDX, IDC_EDIT_POSITION_STATE_X, m_PositionStateX);
	DDX_Control(pDX, IDC_EDIT_POSITION_STATE_Y, m_PositionStateY);
	DDX_Control(pDX, IDC_EDIT_POSITION_STATE_Z, m_PositionStateZ);
	DDX_Control(pDX, IDC_EDIT_CUSTOM_VAR, m_CustomVar);
	DDX_Control(pDX, IDC_EDIT_POSITION_MOVE_X, m_PositionMoveX);
	DDX_Control(pDX, IDC_EDIT_POSITION_MOVE_Y, m_PositionMoveY);
	DDX_Control(pDX, IDC_EDIT_POSITION_MOVE_Z, m_PositionMoveZ);
	DDX_Control(pDX, IDC_EDIT6, m_Speed_Now);
	DDX_Control(pDX, IDC_EDIT7, m_Tca_Now);
	DDX_Control(pDX, IDC_EDIT8, m_Tcd_Now);
	DDX_Control(pDX, IDC_EDIT9, m_Speed_Target);
	DDX_Control(pDX, IDC_EDIT10, m_Tca_Target);
	DDX_Control(pDX, IDC_EDIT11, m_Tcd_Target);
	DDX_Control(pDX, IDC_EDIT_DISTANCE, m_Distance);
	DDX_Control(pDX, IDC_TEXT_STATE_DRIVE_1, m_DriveFin_1);
	DDX_Control(pDX, IDC_TEXT_STATE_DRIVE_2, m_DriveFin_2);
	DDX_Control(pDX, IDC_TEXT_STATE_DRIVE_3, m_DriveFin_3);
	DDX_Control(pDX, IDC_BUTTON12, m_Btn_EmergencyStop);
}

BEGIN_MESSAGE_MAP(CMotorControlMFCDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_Connect, &CMotorControlMFCDlg::OnBnClickedButtonConnect)
	//ON_BN_CLICKED(IDC_BUTTON_JOG_X_PLUS, &CMotorControlMFCDlg::OnBnClickedButtonJogXPlus)
	ON_WM_LBUTTONUP()
	ON_MESSAGE(WM_WORKER_THREAD_MESSAGE_POSITIONSTATE, &CMotorControlMFCDlg::OnUpdatePosition)
	ON_MESSAGE(WM_WORKER_THREAD_MESSAGE_DRIVEFIN, &CMotorControlMFCDlg::OnUpdateDirveFin)
	ON_BN_CLICKED(IDC_BUTTON_ON_SERVO1, &CMotorControlMFCDlg::OnBnClickedButtonOnServo1)
	ON_BN_CLICKED(IDC_BUTTON_OFF_SERVO1, &CMotorControlMFCDlg::OnBnClickedButtonOffServo1)
	ON_BN_CLICKED(IDC_BUTTON_CUSTOM_X_PLUS, &CMotorControlMFCDlg::OnBnClickedButtonCustomXPlus)
	ON_BN_CLICKED(IDC_BUTTON_CUSTOM_X_MINUS, &CMotorControlMFCDlg::OnBnClickedButtonCustomXMinus)
	ON_BN_CLICKED(IDC_BUTTON_SPEED_SETUP, &CMotorControlMFCDlg::OnBnClickedButtonSpeedSetup)
	ON_BN_CLICKED(IDC_BUTTON_ON_SERVO2, &CMotorControlMFCDlg::OnBnClickedButtonOnServo2)
	ON_BN_CLICKED(IDC_BUTTON_OFF_SERVO2, &CMotorControlMFCDlg::OnBnClickedButtonOffServo2)
	ON_BN_CLICKED(IDC_BUTTON_CUSTOM_Y_PLUS, &CMotorControlMFCDlg::OnBnClickedButtonCustomYPlus)
	ON_BN_CLICKED(IDC_BUTTON_CUSTOM_Y_MINUS, &CMotorControlMFCDlg::OnBnClickedButtonCustomYMinus)
	ON_BN_CLICKED(IDC_BUTTON_POSITION_MOVE, &CMotorControlMFCDlg::OnBnClickedButtonPositionMove)
	ON_BN_CLICKED(IDC_BUTTON_DISTANCE_10, &CMotorControlMFCDlg::OnBnClickedDistanceSetup)
	ON_BN_CLICKED(IDC_BUTTON_DISTANCE_1, &CMotorControlMFCDlg::OnBnClickedDistanceSetup)
	ON_BN_CLICKED(IDC_BUTTON_DISTANCE_01, &CMotorControlMFCDlg::OnBnClickedDistanceSetup)
	ON_BN_CLICKED(IDC_BUTTON_DISTANCE_001, &CMotorControlMFCDlg::OnBnClickedDistanceSetup)
	ON_BN_CLICKED(IDC_BUTTON_DISTANCE_0001, &CMotorControlMFCDlg::OnBnClickedDistanceSetup)
	ON_BN_CLICKED(IDC_BUTTON_DISCONNECT, &CMotorControlMFCDlg::OnBnClickedButtonDisconnect)
	ON_BN_CLICKED(IDC_BUTTON_EMERGENCYSTOP, &CMotorControlMFCDlg::OnBnClickedButtonEmergencystop)
END_MESSAGE_MAP()


// CMotorControlMFCDlg 메시지 처리기

BOOL CMotorControlMFCDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 이 대화 상자의 아이콘을 설정합니다.  응용 프로그램의 주 창이 대화 상자가 아닐 경우에는
	//  프레임워크가 이 작업을 자동으로 수행합니다.
	SetIcon(m_hIcon, TRUE);			// 큰 아이콘을 설정합니다.
	SetIcon(m_hIcon, FALSE);		// 작은 아이콘을 설정합니다.

	// TODO: 여기에 추가 초기화 작업을 추가합니다.
	m_btnJogXPlus.SubclassDlgItem(IDC_BUTTON_JOG_X_PLUS, this);
	m_btnJogXMinus.SubclassDlgItem(IDC_BUTTON_JOG_X_MINUS, this);
	m_btnJogYPlus.SubclassDlgItem(IDC_BUTTON_JOG_Y_PLUS, this);
	m_btnJogYMinus.SubclassDlgItem(IDC_BUTTON_JOG_Y_MINUS, this);
	m_btnJogZPlus.SubclassDlgItem(IDC_BUTTON_JOG_Z_PLUS, this);
	m_btnJogZMinus.SubclassDlgItem(IDC_BUTTON_JOG_Z_MINUS, this);
	m_CustomVar.SetWindowText(_T("10"));
	m_Distance.SetWindowText(_T("1"));
	m_DriveFin_1.SetWindowText(_T("운전 상태"));
	m_DriveFin_2.SetWindowText(_T("운전 상태"));
	m_DriveFin_3.SetWindowText(_T("운전 상태"));

	m_Btn_EmergencyStop.EnableWindowsTheming(FALSE);
	m_Btn_EmergencyStop.SetFaceColor(RGB(200, 0, 0), true);

	PntData.position = 0;							/* 						*/
	PntData.speed = 1000;								/* 					*/
	PntData.actime = 3000;								/* 						*/
	PntData.dctime = 3000;								/* 						*/
	PntData.dwell = 0;								/* 0ms							*/
	PntData.subcmd = SSC_SUBCMD_POS_ABS				/* Absolute Position			*/
		| SSC_SUBCMD_STOP_SMZ;			/* Smoothing Stop				*/
	PntData.s_curve = 100;								/* 100% 						*/

	Update_Speed();



	return TRUE;  // 포커스를 컨트롤에 설정하지 않으면 TRUE를 반환합니다.
}

// 대화 상자에 최소화 단추를 추가할 경우 아이콘을 그리려면
//  아래 코드가 필요합니다.  문서/뷰 모델을 사용하는 MFC 애플리케이션의 경우에는
//  프레임워크에서 이 작업을 자동으로 수행합니다.

void CMotorControlMFCDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 그리기를 위한 디바이스 컨텍스트입니다.

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 클라이언트 사각형에서 아이콘을 가운데에 맞춥니다.
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 아이콘을 그립니다.
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}

}

// 사용자가 최소화된 창을 끄는 동안에 커서가 표시되도록 시스템에서
//  이 함수를 호출합니다.
HCURSOR CMotorControlMFCDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


LRESULT CMotorControlMFCDlg::OnUpdatePosition(WPARAM wParam, LPARAM lParam) {
	long* positions = (long*)lParam; // 배열로 전달된 위치 값

	// X, Y, Z 값을 각각 문자열로 변환하여 UI에 출력
	CString strX, strY, strZ;
	strX.Format(_T("%.3f"), static_cast<double>(positions[0]) / 1000.0);
	strY.Format(_T("%.3f"), static_cast<double>(positions[1]) / 1000.0);
	strZ.Format(_T("%.3f"), static_cast<double>(positions[2]) / 1000.0);

	// 각 축의 위치를 해당 Edit Control에 표시
	SetDlgItemText(IDC_EDIT_POSITION_STATE_X, strX);
	SetDlgItemText(IDC_EDIT_POSITION_STATE_Y, strY);
	SetDlgItemText(IDC_EDIT_POSITION_STATE_Z, strZ);

	// 동적 할당된 메모리 해제
	delete[] positions;

	return 0;
}

LRESULT CMotorControlMFCDlg::OnUpdateDirveFin(WPARAM wParam, LPARAM lParam)
{
	int* status = reinterpret_cast<int*>(lParam);
	CString strX, strY, strZ;
	strX.Format(_T("%d"), status[0]); // X축 상태
	strY.Format(_T("%d"), status[1]); // Y축 상태
	strZ.Format(_T("%d"), status[2]); // Z축 상태

	// 상태에 따른 문자열 변환
	auto getStatusString = [](int value) -> CString {
		switch (value) {
		case -1: return _T("에러");
		case 1:  return _T("정지");
		case 2:  return _T("운전중");
		case 3:  return _T("연결안됨");
		default: return _T("알 수 없음");
		}
		};

	// 각 축 상태 문자열 설정
	strX = getStatusString(status[0]);
	strY = getStatusString(status[1]);
	strZ = getStatusString(status[2]);

	// 각 축의 위치를 해당 Edit Control에 표시
	SetDlgItemText(IDC_TEXT_STATE_DRIVE_1, strX);
	SetDlgItemText(IDC_TEXT_STATE_DRIVE_2, strY);
	SetDlgItemText(IDC_TEXT_STATE_DRIVE_3, strZ);
	delete[] status;
	return 0;
}

void CMotorControlMFCDlg::Update_Speed()
{
	CString str;
	m_Speed_Now.SetWindowText((str.Format(_T("%ld"), PntData.speed), str));
	m_Tca_Now.SetWindowText((str.Format(_T("%d"), PntData.actime), str));
	m_Tcd_Now.SetWindowText((str.Format(_T("%d"), PntData.dctime), str));
}

void CMotorControlMFCDlg::OnBnClickedButtonConnect()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	//m_ConnectState.SetWindowText(_T("test"));
	//m_PositionStateX.SetWindowText(_T("123"));
	motorControlManager.open();
	motorControlManager.rebootAndStart();
	motorControlManager.setPoint(PntData);
	threadManager.setDialogWnd(this);
	threadManager.startPositionThread();
	threadManager.startDriveThread();
}

void CMotorControlMFCDlg::OnBnClickedButtonDisconnect()
{
	motorControlManager.close();
	threadManager.stopPositionThread();
	threadManager.stopDriveThread();
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
}

void CMotorControlMFCDlg::OnBnClickedButtonOnServo1()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	motorControlManager.servoOn(1);
}

void CMotorControlMFCDlg::OnBnClickedButtonOffServo1()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	motorControlManager.servoOff(1);
}

void CMotorControlMFCDlg::OnBnClickedButtonOnServo2()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	motorControlManager.servoOn(2);

}


void CMotorControlMFCDlg::OnBnClickedButtonOffServo2()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	motorControlManager.servoOff(2);
}
//void CMotorControlMFCDlg::OnBnClickedButtonJogXPlus()
//{
//	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
//	motorControlManager.jogMove_Plus(1);
//}

void CMotorControlMFCDlg::OnBnClickedButtonCustomXPlus()
{
	CString inputText;
	m_CustomVar.GetWindowText(inputText);
	long distance = _tstol(inputText);

	m_Distance.GetWindowText(inputText);
	double distanceScale = _tstof(inputText);
	distanceScale *= 1000;

	long scaledDistance = static_cast<long>(distance * distanceScale);
	motorControlManager.customMove(1, scaledDistance);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
}


void CMotorControlMFCDlg::OnBnClickedButtonCustomXMinus()
{
	CString inputText;
	m_CustomVar.GetWindowText(inputText);
	long distance = _tstol(inputText);
	distance *= -1;

	m_Distance.GetWindowText(inputText);
	double distanceScale = _tstof(inputText);
	distanceScale *= 1000;

	long scaledDistance = static_cast<long>(distance * distanceScale);
	motorControlManager.customMove(1, scaledDistance);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
}

void CMotorControlMFCDlg::OnBnClickedButtonCustomYPlus()
{
	CString inputText;
	m_CustomVar.GetWindowText(inputText);
	long distance = _tstol(inputText);

	m_Distance.GetWindowText(inputText);
	double distanceScale = _tstof(inputText);
	distanceScale *= 1000;

	long scaledDistance = static_cast<long>(distance * distanceScale);
	motorControlManager.customMove(2, scaledDistance);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
}


void CMotorControlMFCDlg::OnBnClickedButtonCustomYMinus()
{
	CString inputText;
	m_CustomVar.GetWindowText(inputText);
	long distance = _tstol(inputText);
	distance *= -1;

	m_Distance.GetWindowText(inputText);
	double distanceScale = _tstof(inputText);
	distanceScale *= 1000;

	long scaledDistance = static_cast<long>(distance * distanceScale);
	motorControlManager.customMove(2, scaledDistance);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
}




void CMotorControlMFCDlg::OnBnClickedButtonSpeedSetup()
{
	//PNT_DATA_EX		PntData[2] = { {0} };
	///* point data1 set */
	//PntData[0].position = 10000;							/* 10mm							*/
	//PntData[0].speed = 1000;								/* 20mm/s						*/
	//PntData[0].actime = 3000;								/* 100ms						*/
	//PntData[0].dctime = 3000;								/* 100ms						*/
	//PntData[0].dwell = 0;								/* 0ms							*/
	//PntData[0].subcmd	=	SSC_SUBCMD_POS_ABS				/* Absolute Position			*/
	//						|SSC_SUBCMD_STOP_SMZ;			/* Smoothing Stop				*/
	//PntData[0].s_curve = 100;								/* 100% 						*/

	//int ans = sscSetPointDataEx(0, 1, 1, 0, &PntData[0]);
	//if (ans != SSC_OK)
	//{
	//	TRACE(_T("sscSetPointDataEx failure. axnum=%d, sscGetLastError=0x%08X\n"), 1, sscGetLastError());
	//	return;
	//}

	//ans = sscCheckPointDataEx(0, 1, 1, 0, &PntData[1]);
	//if (ans != SSC_OK)
	//{
	//	TRACE(_T("sscJogStart failure. sscGetLastError=0x%08X\n"), sscGetLastError());
	//	return;
	//}
	//else
	//{
	//	//TRACE(_T("sscJogStart success\n"+PntData[0].position));
	//	TRACE(_T("sscJogStart success. Position: %d\n"), PntData[0].position);

	//}
	CString inputText;
	m_Speed_Target.GetWindowText(inputText);  // Edit Control에서 문자열 가져오기
	PntData.speed = _tstol(inputText);     // 문자열을 long으로 변환하여 할당

	m_Tca_Target.GetWindowText(inputText);
	PntData.actime = static_cast<short>(_tstoi(inputText));

	m_Tcd_Target.GetWindowText(inputText);
	PntData.dctime = static_cast<short>(_tstoi(inputText));

	motorControlManager.setPoint(PntData);
	Update_Speed();
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
}

void CMotorControlMFCDlg::OnBnClickedDistanceSetup()
{
	// 클릭된 버튼 ID 가져오기
	CWnd* pButton = GetFocus();
	if (pButton)
	{
		int buttonID = pButton->GetDlgCtrlID();

		// 버튼 ID에 따라 처리
		switch (buttonID)
		{
		case IDC_BUTTON_DISTANCE_10:
			SetDlgItemText(IDC_EDIT_DISTANCE, _T("10"));
			break;
		case IDC_BUTTON_DISTANCE_1:
			SetDlgItemText(IDC_EDIT_DISTANCE, _T("1"));
			break;
		case IDC_BUTTON_DISTANCE_01:
			SetDlgItemText(IDC_EDIT_DISTANCE, _T("0.1"));
			break;
		case IDC_BUTTON_DISTANCE_001:
			SetDlgItemText(IDC_EDIT_DISTANCE, _T("0.01"));
			break;
		case IDC_BUTTON_DISTANCE_0001:
			SetDlgItemText(IDC_EDIT_DISTANCE, _T("0.001"));
			break;
		default:
			break;
		}
	}
}








void CMotorControlMFCDlg::OnBnClickedButtonPositionMove()
{
	//short monnum[4] = { 0, 0, 0, 0 }; // 모니터 번호 설정
	//int ans = sscSetMonitor(0, 1, 0, monnum);
	//if (ans != SSC_OK)
	//{
	//	TRACE(_T("sscSetMonitor failure. sscGetLastError=0x%08X\n"), sscGetLastError());
	//	return;
	//}
	//else
	//{
	//	//TRACE(_T("sscJogStart success\n"+PntData[0].position));
	//	TRACE(_T("sscSetMonitor success.\n"));
	//}
	//short test2 = 0;
	//ans = sscGetIoStatusFast(0, 1, 1, &test2);
	//if (ans != SSC_OK)
	//{
	//	TRACE(_T("sscGetIoStatusFast failure. sscGetLastError=0x%08X\n"), sscGetLastError());
	//	return;
	//}
	//else
	//{
	//	//TRACE(_T("sscJogStart success\n"+PntData[0].position));
	//	TRACE(_T("sscGetIoStatusFast success. Value: %d\n"), test2);
	//}
	short c = 0;
	int ans = sscGetControlCycle(0, 1, &c);
	if (ans != SSC_OK)
	{
		TRACE(_T("sscGetControlCycle failure. sscGetLastError=0x%08X\n"), sscGetLastError());
		return;
	}
	else
	{
		//TRACE(_T("sscJogStart success\n"+PntData[0].position));
		TRACE(_T("sscGetControlCycle success.\n"));
	}

}

//void CMotorControlMFCDlg::OnBnClickedButtonPositionMove()
//{
//
//	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
//	CString* inputText = new CString[3];
//	m_PositionMoveX.GetWindowText(inputText[0]);
//	m_PositionMoveY.GetWindowText(inputText[1]);
//	m_PositionMoveZ.GetWindowText(inputText[2]);
//	motorControlManager.positionMove(inputText);
//
//
//}

void CMotorControlMFCDlg::OnBnClickedButtonEmergencystop()
{
	motorControlManager.emergencyStop();
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
}
