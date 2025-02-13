using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotorControl
{
    public partial class CustomMove : Form
    {
        public ThreadManager threadManager;
        public MotorControlManager motorControlManager;
        bool isJogMode = false;
        public CustomMove(ThreadManager threadManager, MotorControlManager motorControlManager)
        {
            InitializeComponent(); 
            this.threadManager = threadManager;
            this.motorControlManager = motorControlManager;
        }
        private void JogButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isJogMode)
                return;
            if (sender is Button button && button.Tag is string tagValue)
            {
                // 문자열 "1,0"을 ','로 분리하여 튜플로 변환
                var parts = tagValue.Split(',');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int axis) &&
                    int.TryParse(parts[1], out int direction))
                {
                    threadManager.AddWorkerTask((token) => motorControlManager.JogMove(axis, direction));
                }
            }
        }
        private void JogButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isJogMode)
                return;
            if (sender is Button button && button.Tag is string tagValue)
            {
                // 문자열 "1,0"을 ','로 분리하여 튜플로 변환
                var parts = tagValue.Split(',');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int axis))
                {
                    threadManager.AddWorkerTask((token) => motorControlManager.JogStop(axis));
                }
            }
        }
        private void ExpansionCustomMove(object sender, EventArgs e)
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            if (sender is Button button && button.Tag is string tagValue)
            {
                if (string.IsNullOrWhiteSpace(tb_CustomMove.Text))
                {
                    MessageBox.Show("값을 입력해주세요.");
                    return;
                }
                if (!int.TryParse(tb_CustomMove.Text, out int distance))
                {
                    MessageBox.Show("정수를 입력해주세요.");
                    return;
                }
                distance *= (int)(double.Parse(tb_Distance.Text) * 1000);
                // 문자열 "1,0"을 ','로 분리하여 튜플로 변환
                var parts = tagValue.Split(',');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int axis) &&
                    int.TryParse(parts[1], out int direction))
                {
                    if (direction == 0)
                    {
                        threadManager.AddWorkerTask((token) => motorControlManager.CustomMove(axis, distance));
                    }
                    else if (direction == 1)
                    {
                        distance *= -1;
                        threadManager.AddWorkerTask((token) => motorControlManager.CustomMove(axis, distance));
                    }
                }
            }
        }

        private void btn_ChangeJogMode_Click(object sender, EventArgs e)
        {
            // isExpanded 상태 토글
            isJogMode = !isJogMode;

            // 버튼의 눌린 현상 추가
            if (isJogMode)
            {
                btn_ChangeJogMode.FlatStyle = FlatStyle.Popup; // 눌린 스타일
                btn_ChangeJogMode.BackColor = SystemColors.ControlDark; // 눌린 배경색
            }
            else
            {
                btn_ChangeJogMode.FlatStyle = FlatStyle.Standard; // 기본 스타일
                btn_ChangeJogMode.BackColor = SystemColors.ControlLight; // 기본 배경색
            }
        }

        private void btn_DistanceSet_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is string tagValue)
            {
                tb_Distance.Text = tagValue;
            }
        }
    }
}
