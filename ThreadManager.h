#pragma once
#include "MotorControlManager.h"
#include <thread>
#include <atomic>

class ThreadManager {
private:
    MotorControlManager motorControlManager;
    std::thread m_positionThread;
    std::thread m_driveThread;
    std::atomic<bool> m_running_position;
    std::atomic<bool> m_running_drive;
    CWnd* m_pDlgWnd = nullptr; // UI 핸들

public:
    ThreadManager();
    ~ThreadManager();

    void setDialogWnd(CWnd* pDlgWnd); // UI 핸들 설정
    void startPositionThread();
    void stopPositionThread();

    void startDriveThread();
    void stopDriveThread();

    void uiUpdate(); // UI 업데이트 호출
    //long getCurrentPosition(); // MotorControlManager에서 위치 조회
};
