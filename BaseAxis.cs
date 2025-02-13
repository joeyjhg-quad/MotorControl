using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
//using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using mc2xxstd;
using static mc2xxstd.SscApi;

namespace MotorControl
{
    public class BaseAxis
    {
        protected int axisNumber;
        int board_id = 0;
        int channel = 1;
        int ans = 0;
        PNT_DATA_EX data;
        public BaseAxis(int AxisNumber)
        {
            axisNumber = AxisNumber;
        }

        public void SetPoint(PNT_DATA_EX PntData, int ptnnum = 0)
        {
            ans = sscSetPointDataEx(board_id, channel, axisNumber, ptnnum, ref PntData);
            if (ans != SSC_OK)
            {
                Debug.WriteLine($"sscSetPointDataEx failure. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
            Logger.Log("SetPoint success");
        }
        public void GetPoint(PNT_DATA_EX PntData, int ptnnum = 0)
        {
            ans = sscCheckPointDataEx(board_id, channel, axisNumber, ptnnum, out PntData);
            if (ans != SSC_OK)
            {
                Debug.WriteLine($"sscSystemStart failure. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
        }
        public void ServoOn()
        {
            ans = sscSetCommandBitSignalEx(board_id, channel, axisNumber, SSC_CMDBIT_AX_SON, SSC_BIT_ON);
            if (ans != SSC_OK)
            {
                Debug.WriteLine($"ServoOn failure. sscGetLastError=0x{sscGetLastError():X}\n");
                Logger.Log($"Servo{axisNumber}On failure");
                return;
            }
                Logger.Log($"Servo{axisNumber}On success");
        }

        public void ServoOff()
        {
            ans = sscSetCommandBitSignalEx(board_id, channel, axisNumber, SSC_CMDBIT_AX_SON, SSC_BIT_OFF);
            if (ans != SSC_OK)
            {
                Debug.WriteLine($"ServoOff failure. sscGetLastError=0x{sscGetLastError():X}\n");
                Logger.Log($"Servo{axisNumber}Off failure");
                return;
            }
            Logger.Log($"Servo{axisNumber}Off success");
        }

        public void JogMovePlus()
        {
            ans = sscCheckPointDataEx(board_id, channel, axisNumber, 0, out data);
            ans = sscJogStart(board_id, channel, axisNumber, (int)data.speed, (short)data.actime, (short)data.dctime, SSC_DIR_PLUS);
            Thread.Sleep(50);
            if (ans != SSC_OK)
            {
                Debug.WriteLine($"sscJogStart failure. sscGetLastError=0x{sscGetLastError():X}\n");
                Logger.Log($"axis{axisNumber} JogMove fail");
                CheckAlarm();
                return;
            }
            else
            {
                Logger.Log($"axis{axisNumber} JogMove success");
                //CheckAlarm();
                Debug.WriteLine("sscJogStart success.\n");
            }
        }

        public void JogMoveMinus()
        {
            ans = sscCheckPointDataEx(board_id, channel, axisNumber, 0, out data);
            ans = sscJogStart(board_id, channel, axisNumber, (int)data.speed, (short)data.actime, (short)data.dctime, SSC_DIR_MINUS);
            Thread.Sleep(50);
            if (ans != SSC_OK)
            {
                Debug.WriteLine($"sscJogStart failure. sscGetLastError=0x{sscGetLastError():X}\n");
                Logger.Log($"axis{axisNumber} JogMove fail");
                CheckAlarm();
                return;
            }
            else
            {
                Logger.Log($"axis{axisNumber} JogMove success");
                //CheckAlarm();
                Debug.WriteLine("sscJogStart success.\n");
            }
        }

        public void JogStop()
        {
            ans = sscJogStop(board_id, channel, axisNumber);
            if (ans != SSC_OK)
            {
                Debug.WriteLine($"sscJogStop failure. sscGetLastError=0x{sscGetLastError():X}\n");
                Logger.Log($"axis{axisNumber} JogStop fail");
                CheckAlarm();
                return;
            }
            else
            {
                Logger.Log($"axis{axisNumber} JogStop success");
                //CheckAlarm();
                Debug.WriteLine("sscJogStop success.\n");
            }
        }

        public void CustomMove(int distance)
        {
            ans = sscCheckPointDataEx(board_id, channel, axisNumber, 0, out data);
            if (ans != SSC_OK)
            {
                Logger.Log($"axis{axisNumber} sscCheckPointDataEx fail");
                Debug.WriteLine($"sscCheckPointDataEx failure. sscGetLastError=0x{sscGetLastError():X}\n");
                return;
            }
            ans = sscIncStart(board_id, channel, axisNumber, distance, (int)data.speed, (short)data.actime, (short)data.dctime);
            Thread.Sleep(50);
            if (ans != SSC_OK)
            {
                Logger.Log($"axis{axisNumber} CustomMove fail");
                Debug.WriteLine($"sscIncStart failure. sscGetLastError=0x{sscGetLastError():X}\n");
                CheckAlarm();
                return;
            }
            Logger.Log($"axis{axisNumber} CustomMove success");

        }

        //public void PositionMove(int point_s, int point_e)
        //{
        //    ans = sscLinearStart(board_id, channel, axisNumber, 1, point_s, point_e);
        //    if (ans != SSC_OK)
        //    {
        //        Logger.Log($"axis{axisNumber} CustomMove fail");
        //        Debug.WriteLine($"positionMove failure. sscGetLastError=0x{sscGetLastError():X}\n");
        //        return;
        //    }
        //}

        public void CheckAlarm()
        {
            ushort code = 0;
            ushort detail_code = 0;
            ans = sscGetAlarm(board_id, channel, axisNumber, SSC_ALARM_OPERATION, out code, out detail_code);
            if (ans != SSC_OK)
            {
                Debug.WriteLine($"sscGetAlarm failure. sscGetLastError=0x{sscGetLastError():X}\n");
                Logger.Log($"axis{axisNumber} sscGetAlarm failure");
                return;
                // 알람 코드와 상세 코드가 반환되면 출력
            }
            else
            {
                Debug.WriteLine("sscGetAlarm success.\n");
            }
            if (code == 0)
                return;
            else
            {
                switch (code)
                {
                    case 33:   // 지령속도 에러
                        Logger.Log($"axis{axisNumber} 지령 속도, 속도 제한값을 1이상으로 설정해주십시오.");
                        Debug.WriteLine("지령 속도, 속도 제한값을 1이상으로 설정해주십시오.\n");
                        ans = sscResetAlarm(board_id, channel, axisNumber, SSC_ALARM_OPERATION);
                        break;
                    case 152:   // 원점복귀 에러
                        Logger.Log($"axis{axisNumber} 이동량이,원점서치리미트(파라미터No.024A, 024B)이상이되었습니다.");
                        Debug.WriteLine("이동량이,원점서치리미트(파라미터No.024A, 024B)이상이되었습니다.\n");
                        ans = sscResetAlarm(board_id, channel, axisNumber, SSC_ALARM_OPERATION);
                        break;
                    case 160:   // 리미트스위치 에러
                        Logger.Log($"axis{axisNumber} 리미트스위치가 off되었습니다.JOG 운전등에의해 역방향으로 이동해,리미트스위치 범위내로 되돌려주십시오.");
                        Debug.WriteLine("리미트스위치가 off되었습니다.JOG 운전등에의해역방향으로이동해,리미트스위치범위내로되돌려주십시오.\n");
                        ans = sscResetAlarm(board_id, channel, axisNumber, SSC_ALARM_OPERATION);
                        break;
                    case 178:   // survo off
                        Logger.Log($"axis{axisNumber} 서보 ON으로 바꿔주십시오.");
                        Debug.WriteLine("서보 ON으로 바꿔주십시오.\n");
                        ans = sscResetAlarm(board_id, channel, axisNumber, SSC_ALARM_OPERATION);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public class Axis1 : BaseAxis { public Axis1() : base(1) { } }
    public class Axis2 : BaseAxis { public Axis2() : base(2) { } }
    public class Axis3 : BaseAxis { public Axis3() : base(3) { } }
}
