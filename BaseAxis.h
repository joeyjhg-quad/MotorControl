#pragma once
class BaseAxis {
protected:
    int axisNumber;  // �� ��ȣ (1, 2, 3 ��)
    int board_id = 0;
    int channel = 1;
    int ans = 0;

    PNT_DATA_EX data;
public:
    BaseAxis(int axisNum) : axisNumber(axisNum) {} // �� ��ȣ ����
    virtual ~BaseAxis() {}

    void setPoint(PNT_DATA_EX PntData, int ptnnum = 0);
    void getPoint(PNT_DATA_EX& PntData, int ptnnum = 0);
    void servoOn();

    void servoOff();
    // ���� �̵� �Լ�: ���� ��ȣ, �ӵ�, ������ �޾Ƽ� API ȣ��
    void jogMove_Plus();
    void jogMove_Minus();
    //void jogMove(int boardNum, int speed, int direction);

    void jogStop();

    void customMove(long distance);

    void positionMove(int point_s = 0, int point_e = 1);

    void checkAlarm();
    // ���� �ʱ�ȭ �Լ� (�ʿ�� �������̵�)
    //virtual void initializeMotor() = 0;
};
