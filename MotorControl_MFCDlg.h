
// MotorControl_MFCDlg.h: 헤더 파일
//

#pragma once
#include "MotorControlManager.h"
#include "ButtonClass.h"
#include "ThreadManager.h"

// CMotorControlMFCDlg 대화 상자
class CMotorControlMFCDlg : public CDialogEx
{
// 생성입니다.
public:
	CMotorControlMFCDlg(CWnd* pParent = nullptr);	// 표준 생성자입니다.
// 대화 상자 데이터입니다.
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_MOTORCONTROL_MFC_DIALOG };
#endif

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 지원입니다.


// 구현입니다.
protected:
	HICON m_hIcon;

	// 생성된 메시지 맵 함수
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg LRESULT OnUpdatePosition(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnUpdateDirveFin(WPARAM wParam, LPARAM lParam);
	DECLARE_MESSAGE_MAP()
public:
	PNT_DATA_EX		PntData;
	//afx_msg void OnBnClickedButton1();
	MotorControlManager motorControlManager;
	ThreadManager threadManager;
	CStatic m_ConnectState;
	CStatic m_Servo1State;
	afx_msg void OnBnClickedButtonConnect();
	CEdit m_PositionStateX;
	CEdit m_PositionStateY;
	CEdit m_PositionStateZ;
	CEdit m_CustomVar;
	CEdit m_PositionMoveX;
	CEdit m_PositionMoveY;
	CEdit m_PositionMoveZ;
	void Update_Speed();
	//afx_msg void OnBnClickedButtonJogXPlus();
	afx_msg void OnBnClickedButtonOnServo1();
	afx_msg void OnBnClickedButtonOffServo1();
	ButtonClass m_btnJogXPlus;
	ButtonClass m_btnJogXMinus;
	ButtonClass m_btnJogYPlus;
	ButtonClass m_btnJogYMinus;
	ButtonClass m_btnJogZPlus;
	ButtonClass m_btnJogZMinus;
	afx_msg void OnBnClickedButtonCustomXPlus();
	afx_msg void OnBnClickedButtonCustomXMinus();
	afx_msg void OnBnClickedButtonSpeedSetup();
	afx_msg void OnBnClickedDistanceSetup();

	CEdit m_Speed_Now;
	CEdit m_Tca_Now;
	CEdit m_Tcd_Now;
	CEdit m_Speed_Target;
	CEdit m_Tca_Target;
	CEdit m_Tcd_Target;
	afx_msg void OnBnClickedButtonOnServo2();
	afx_msg void OnBnClickedButtonOffServo2();
	afx_msg void OnBnClickedButtonCustomYPlus();
	afx_msg void OnBnClickedButtonCustomYMinus();
	afx_msg void OnBnClickedButtonPositionMove();
	CEdit m_Distance;
	afx_msg void OnBnClickedButtonDisconnect();
	CStatic m_DriveFin_1;
	CStatic m_DriveFin_2;
	CStatic m_DriveFin_3;
	CMFCButton m_Btn_EmergencyStop;
	afx_msg void OnBnClickedButtonEmergencystop();
};
