#pragma once
class BaseAxis {
protected:
    int axisNumber;  // 축 번호 (1, 2, 3 등)
    int board_id = 0;
    int channel = 1;
    int ans = 0;

    PNT_DATA_EX data;
public:
    BaseAxis(int axisNum) : axisNumber(axisNum) {} // 축 번호 설정
    virtual ~BaseAxis() {}

    void setPoint(PNT_DATA_EX PntData, int ptnnum = 0);
    void getPoint(PNT_DATA_EX& PntData, int ptnnum = 0);
    void servoOn();

    void servoOff();
    // 조그 이동 함수: 보드 번호, 속도, 방향을 받아서 API 호출
    void jogMove_Plus();
    void jogMove_Minus();
    //void jogMove(int boardNum, int speed, int direction);

    void jogStop();

    void customMove(long distance);

    void positionMove(int point_s = 0, int point_e = 1);

    void checkAlarm();
    // 가상 초기화 함수 (필요시 오버라이드)
    //virtual void initializeMotor() = 0;
};

