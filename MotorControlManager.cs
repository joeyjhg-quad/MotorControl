using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using mc2xxstd;
using static mc2xxstd.SscApi;

namespace MotorControl
{
    public class MotorControlManager
    {
        int board_id = 0;
        int channel = 1;
        int ans;
        Axis1 axis1;
        Axis2 axis2;
        Axis3 axis3;
        public MotorControlManager()
        {
            bool load_lib_flg = true;
            load_lib_flg = LoadLibraryDll();
            if (load_lib_flg == false)
            {
                Logger.Log("Api Load fail");
                Logger.Log("Api Load fail.\n");
                return;
            }
            axis1 = new Axis1();
            axis2 = new Axis2();
            axis3 = new Axis3();
        }
        public void GetCurrentPosition(int[] positions)
        {
            for (int i = 0; i < 3; i++)
            {
                ans = sscGetCurrentCmdPositionFast(board_id, channel, i + 1, out positions[i]);
                if (ans != SSC_OK)
                    positions[i] = -1;
            }
        }

        public void GetDriveFin(int[] finStatus)
        {
            for (int i = 0; i < 3; ++i)
            {
                int ans = sscGetDriveFinStatus(board_id, channel, i + 1, SSC_FIN_TYPE_SMZ, out finStatus[i]);
                if (ans != SSC_OK)
                {
                    Logger.Log($"sscGetDriveFinStatus failed for axis {i + 1}. sscGetLastError=0x{sscGetLastError():X}");
                    finStatus[i] = -1;
                }
            }
        }


        public void Open()
        {
            int ans = sscOpen(board_id);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscOpen failure. sscGetLastError=0x{sscGetLastError():X}");
            }
            else
            {
                Logger.Log("sscOpen success");
            }
        }
        public void Close()
        {
            int ans = sscClose(board_id);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscClose failure. sscGetLastError=0x{sscGetLastError():X}");
            }
            else
            {
                Logger.Log("sscClose success");
            }
        }
        public void RebootAndStart()
        {
            int ans = sscReboot(board_id, channel, SSC_DEFAULT_TIMEOUT);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscReboot failure. sscGetLastError=0x{sscGetLastError():X}");
                return;
            }

            ans = sscResetAllParameter(board_id, channel, SSC_DEFAULT_TIMEOUT);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscResetAllParameter failure. sscGetLastError=0x{sscGetLastError():X}");
                return;
            }

            ans = sscLoadAllParameterFromFlashROM(board_id, channel, SSC_DEFAULT_TIMEOUT);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscLoadAllParameterFromFlashROM failure. sscGetLastError=0x{sscGetLastError():X}");
                return;
            }

            ans = sscSystemStart(board_id, channel, SSC_DEFAULT_TIMEOUT);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscSystemStart failure. sscGetLastError=0x{sscGetLastError():X}");
                return;
            }
            Logger.Log("RebootAndStart success");

        }

        public void SetPoint(PNT_DATA_EX pntData)
        {
            axis1.SetPoint(pntData);
            axis2.SetPoint(pntData);
            axis3.SetPoint(pntData);
        }

        public void ServoOn(int axisNum)
        {
            switch (axisNum)
            {
                case 1:
                    axis1.ServoOn();
                    break;
                case 2:
                    axis2.ServoOn();
                    break;
                case 3:
                    axis3.ServoOn();
                    break;
            }
        }

        public void ServoOff(int axisNum)
        {
            switch (axisNum)
            {
                case 1:
                    axis1.ServoOff();
                    break;
                case 2:
                    axis2.ServoOff();
                    break;
                case 3:
                    axis3.ServoOff();
                    break;
            }
        }

        public void JogMove(int axisNum, int direction)
        {
            switch (axisNum)
            {
                case 1:
                    if (direction == 1) axis1.JogMoveMinus();
                    else if (direction == 0) axis1.JogMovePlus();
                    break;
                case 2:
                    if (direction == 1) axis2.JogMoveMinus();
                    else if (direction == 0) axis2.JogMovePlus();
                    break;
                case 3:
                    if (direction == 1) axis3.JogMoveMinus();
                    else if (direction == 0) axis3.JogMovePlus();
                    break;
            }
        }

        public void JogStop(int axisNum)
        {
            switch (axisNum)
            {
                case 1:
                    axis1.JogStop();
                    break;
                case 2:
                    axis2.JogStop();
                    break;
                case 3:
                    axis3.JogStop();
                    break;
            }
        }

        public void CustomMove(int axisNum, int distance)
        {
            switch (axisNum)
            {
                case 1:
                    axis1.CustomMove(distance);
                    break;
                case 2:
                    axis2.CustomMove(distance);
                    break;
                case 3:
                    axis3.CustomMove(distance);
                    break;
            }
        }

        public void PositionMove(int[] input)
        {
            int[] positions = new int[3];
            for (int i = 0; i < 3; i++)
            {
                int ans = sscGetCurrentCmdPositionFast(board_id, channel, i + 1, out positions[i]);
                if (ans != SSC_OK) positions[i] = -1;

                int position = input[i];
                int moveDistance = position - positions[i];

                switch (i)
                {
                    case 0:
                        axis1.CustomMove(moveDistance);
                        break;
                    case 1:
                        axis2.CustomMove(moveDistance);
                        break;
                    case 2:
                        axis3.CustomMove(moveDistance);
                        break;
                }
            }
        }
        public void EmergencyStop()
        {
            for (int i = 0; i < 3; i++)
            {
                ans = sscDriveStop(board_id, channel, i + 1, 0);
                if (ans != SSC_OK)
                {
                    Logger.Log($"sscDriveStop failure. sscGetLastError=0x{sscGetLastError():X}");
                }
                else
                {
                    Logger.Log($"axis{i + 1} sscDriveStop success");
                }
            }
        }

        public void Sequence(int x, int y, int z)
        {
            Logger.Log("Sequence start");

            int currentPosition = 0;
            int moveDistance = 0;

            // 1️⃣ Z축을 15000으로 먼저 이동
            int initialZ = 15000;
            ans = sscGetCurrentCmdPositionFast(board_id, channel, 3, out currentPosition);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscGetCurrentCmdPositionFast failed for Z-axis. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
            moveDistance = initialZ - currentPosition;
            CustomMove(3, moveDistance); // Z축 이동
            WaitForAxis(3); // Z축 완료 대기

            // 2️⃣ Y축 이동
            ans = sscGetCurrentCmdPositionFast(board_id, channel, 2, out currentPosition);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscGetCurrentCmdPositionFast failed for Y-axis. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
            moveDistance = y - currentPosition;
            CustomMove(2, moveDistance);
            WaitForAxis(2); // Y축 완료 대기

            // 3️⃣ X축 이동
            ans = sscGetCurrentCmdPositionFast(board_id, channel, 1, out currentPosition);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscGetCurrentCmdPositionFast failed for X-axis. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
            moveDistance = x - currentPosition;
            CustomMove(1, moveDistance);
            WaitForAxis(1); // X축 완료 대기

            // 4️⃣ 마지막으로 Z축을 원하는 값으로 이동
            ans = sscGetCurrentCmdPositionFast(board_id, channel, 3, out currentPosition);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscGetCurrentCmdPositionFast failed for Z-axis. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
            moveDistance = z - currentPosition;
            CustomMove(3, moveDistance);
            WaitForAxis(3); // 최종 Z축 이동 완료 대기

            Logger.Log("Sequence end");
        }
        public void SequenceRelative(int dx, int dy, int dz)
        {
            Logger.Log("Sequence start (Relative)");

            int currentX = 0, currentY = 0, currentZ = 0;
            int moveDistance = 0;

            // 현재 위치 읽기
            ans = sscGetCurrentCmdPositionFast(board_id, channel, 1, out currentX);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscGetCurrentCmdPositionFast failed for X-axis. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
            ans = sscGetCurrentCmdPositionFast(board_id, channel, 2, out currentY);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscGetCurrentCmdPositionFast failed for Y-axis. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
            ans = sscGetCurrentCmdPositionFast(board_id, channel, 3, out currentZ);
            if (ans != SSC_OK)
            {
                Logger.Log($"sscGetCurrentCmdPositionFast failed for Z-axis. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }

            // 목표 위치 계산
            int targetX = currentX + dx;
            int targetY = currentY + dy;
            int targetZ = currentZ + dz;

            // 1️⃣ Z축을 15000으로 먼저 이동
            int initialZ = 15000;
            moveDistance = initialZ - currentZ;
            CustomMove(3, moveDistance); // Z축 이동
            WaitForAxis(3); // Z축 완료 대기

            // 2️⃣ Y축 이동
            moveDistance = targetY - currentY;
            CustomMove(2, moveDistance);
            WaitForAxis(2); // Y축 완료 대기

            // 3️⃣ X축 이동
            moveDistance = targetX - currentX;
            CustomMove(1, moveDistance);
            WaitForAxis(1); // X축 완료 대기

            // 4️⃣ 마지막으로 Z축을 목표 위치로 이동
            moveDistance = targetZ - initialZ;
            CustomMove(3, moveDistance);
            WaitForAxis(3); // 최종 Z축 이동 완료 대기

            Logger.Log($"Sequence end (New Position: X={targetX}, Y={targetY}, Z={targetZ})");
        }

        private void WaitForAxis(int axisNumber)
        {
            const int TIMEOUT_SEC = 10;
            const int CHECK_INTERVAL_MS = 100;

            for (int elapsedTime = 0; elapsedTime < TIMEOUT_SEC * 1000; elapsedTime += CHECK_INTERVAL_MS)
            {
                int finStatus = 0;
                ans = sscGetDriveFinStatus(board_id, channel, axisNumber, SSC_FIN_TYPE_SMZ, out finStatus);

                if (ans != SSC_OK)
                {
                    Logger.Log($"sscGetDriveFinStatus failed for axis {axisNumber}. sscGetLastError=0x{sscGetLastError():X}\n");
                    return;
                }

                if (finStatus == SSC_FIN_STS_STP) // 이동 완료 상태
                {
                    Logger.Log($"Axis {axisNumber}: Drive finished successfully.\n");
                    break;
                }
                else if (finStatus == SSC_FIN_STS_ALM_STP || finStatus == SSC_FIN_STS_ALM_MOV) // 알람 상태
                {
                    Logger.Log($"Axis {axisNumber}: Alarm detected. Status={finStatus}\n");
                    switch (axisNumber)
                    {
                        case 1:
                            axis1.CheckAlarm();
                            break;
                        case 2:
                            axis2.CheckAlarm();
                            break;
                        case 3:
                            axis3.CheckAlarm();
                            break;
                        default:
                            break;
                    }
                    return;
                }

                Thread.Sleep(CHECK_INTERVAL_MS);
            }
        }

        public void HomeReturn()
        {
            Logger.Log("HomeReturn start");
            // 3축 -> 1축 -> 2축 순서대로 원점 복귀
            for (int i = 0; i < 3; i++)
            {
                int axisNumber = (i + 2) % 3 + 1; // 3축 -> 1축 -> 2축 순서 계산
                ans = sscHomeReturnStart(board_id, channel, axisNumber);

                if (ans != SSC_OK)
                {
                    Logger.Log($"HomeReturn failure for axis {axisNumber}. sscGetLastError=0x{sscGetLastError():X}\n");
                    if (axisNumber == 3)
                    {
                        ans = sscResetAlarm(board_id, channel, axisNumber, SSC_ALARM_OPERATION);
                        CustomMove(axisNumber, -30000);
                        i -= 1;
                        Thread.Sleep(1000);
                        continue;
                    }
                    return;
                }
                else
                {
                    Logger.Log($"HomeReturn started successfully for axis {axisNumber}\n");
                }

                // 상태 확인: n초 동안 sscGetDriveFinStatus로 상태 확인
                const int TIMEOUT_SEC = 30; // 최대 대기 시간 (초)
                const int CHECK_INTERVAL_MS = 100; // 상태 확인 간격 (밀리초)
                bool isCompleted = false;

                for (int elapsedTime = 0; elapsedTime < TIMEOUT_SEC * 1000; elapsedTime += CHECK_INTERVAL_MS)
                {
                    int fin_status = 0;
                    ans = sscGetDriveFinStatus(board_id, channel, axisNumber, SSC_FIN_TYPE_SMZ, out fin_status);

                    if (ans != SSC_OK)
                    {
                        Logger.Log($"sscGetDriveFinStatus failed for axis {axisNumber}. sscGetLastError=0x{sscGetLastError():X}\n");
                        return; // 에러 발생 시 복귀 종료
                    }

                    // 운전 완료 상태 확인
                    if (fin_status == SSC_FIN_STS_STP) // 운전 완료 상태
                    {
                        Logger.Log($"Axis {axisNumber}: Drive finished successfully.\n");
                        isCompleted = true;
                        break;
                    }
                    else if (fin_status == SSC_FIN_STS_ALM_STP || fin_status == SSC_FIN_STS_ALM_MOV) // 알람 상태
                    {
                        Logger.Log($"Axis {axisNumber}: Alarm detected. Status={fin_status}\n");
                        switch (axisNumber)
                        {
                            case 1:
                                axis1.CheckAlarm();
                                break;
                            case 2:
                                axis2.CheckAlarm();
                                break;
                            case 3:
                                axis3.CheckAlarm();
                                break;
                            default:
                                break;
                        }
                        return; // 알람 발생 시 복귀 종료
                    }

                    // 상태 확인 간격 대기
                    Thread.Sleep(CHECK_INTERVAL_MS);
                }

                if (!isCompleted)
                {
                    Logger.Log($"Timeout while waiting for axis {axisNumber} to complete drive.\n");
                    return; // 타임아웃 시 복귀 종료
                }
            }

            Logger.Log("HomeReturn end");
        }

    }
}