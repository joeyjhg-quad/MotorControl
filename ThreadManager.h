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
    CWnd* m_pDlgWnd = nullptr; // UI �ڵ�

public:
    ThreadManager();
    ~ThreadManager();

    void setDialogWnd(CWnd* pDlgWnd); // UI �ڵ� ����
    void startPositionThread();
    void stopPositionThread();

    void startDriveThread();
    void stopDriveThread();

    void uiUpdate(); // UI ������Ʈ ȣ��
    //long getCurrentPosition(); // MotorControlManager���� ��ġ ��ȸ
};