#include "pch.h"
#include "ThreadManager.h"

ThreadManager::ThreadManager() : m_running_position(false), m_running_drive(false) {}
ThreadManager::~ThreadManager() {
	stopPositionThread();
	stopDriveThread();
}

void ThreadManager::setDialogWnd(CWnd* pDlgWnd) {
	m_pDlgWnd = pDlgWnd;
}

void ThreadManager::startPositionThread() {
	m_running_position = true;
	m_positionThread = std::thread([this]() {
		while (m_running_position) {
			long* positions = new long[3]; // 동적 할당
			motorControlManager.getCurrentPosition(positions);
			if (m_pDlgWnd && ::IsWindow(m_pDlgWnd->GetSafeHwnd())) {
				::PostMessage(m_pDlgWnd->GetSafeHwnd(), WM_WORKER_THREAD_MESSAGE_POSITIONSTATE, 0, (LPARAM)positions);
				//TRACE(_T("message send Value: %d\n"), positions);
			}
			else {
				//TRACE(_T("thread fail. Value: %d\n"), positions);
			}

			Sleep(500);
		}
		});
}

void ThreadManager::stopPositionThread() {
	m_running_position = false;
	if (m_positionThread.joinable()) {
		m_positionThread.join();
	}
}

void ThreadManager::startDriveThread()
{
	m_running_drive = true;
	m_driveThread = std::thread([this]() {
		while (m_running_drive) {
			int* status = new int[3] { 0 };
			motorControlManager.getDriveFin(status);
			if (m_pDlgWnd && ::IsWindow(m_pDlgWnd->GetSafeHwnd())) {
				::PostMessage(m_pDlgWnd->GetSafeHwnd(), WM_WORKER_THREAD_MESSAGE_DRIVEFIN, 0, reinterpret_cast<LPARAM>(status));
				//TRACE(_T("message send Value: %d\n"), positions);
			}
			else {
				//TRACE(_T("thread fail. Value: %d\n"), positions);
			}
			Sleep(100);
		}
		});
}

void ThreadManager::stopDriveThread()
{
	m_running_drive = false;
	if (m_driveThread.joinable()) {
		m_driveThread.join();
	}
}

//long ThreadManager::getCurrentPosition() {
//    return motorControlManager.getCurrentPosition();
//}

void ThreadManager::uiUpdate() {
	// 필요 시 추가 UI 업데이트 로직 작성
}
