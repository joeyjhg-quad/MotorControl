using OpenCvSharp;
using OpenCvSharp.Extensions;
using mc2xxstd;
using static mc2xxstd.SscApi;
using Grpc.Net.Client;
using System.IO.Ports;
using System.Windows.Forms;
using System;
using System.Diagnostics;
using gRPCBfx;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.VisualBasic.Logging;
using System.Text;

namespace MotorControl
{
    public partial class Main : Form
    {
        MotorControlManager motorControlManager = new MotorControlManager();
        ThreadManager threadManager = new ThreadManager();
        CameraManager cameraManager = new CameraManager();
        TextFileManager textFileManager = new TextFileManager();
        ImageProcessing imageProcessing = new ImageProcessing();
        private SerialPort serialPort;
        private GrpcChannel channel;
        private BFXClientService.BFXClientServiceClient bfxClient;
        PNT_DATA_EX PntData;
        Mat img = null;
        private System.Windows.Forms.Timer positionUpdateTimer;
        bool isExpanded = false;
        bool isProcessingImg = false;
        bool isJogMode = false;
        bool BFXConnect = false;
        private ShowWindowResponse showWindow;
        private HideWindowResponse hideWindow;
        private DispenserCommandResponse dispenserCommandResponse;
        //Camera  camera;
        OpenCvSharp.Point[][] SelectedPoints_Test;
        public Main()
        {
            InitializeComponent();
            Logger.Initialize(this);
        }
        private void Main_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(0, 0); // 화면의 (0, 0) 위치로 설정

            SetAllValues(textFileManager.GetAllValues());
            //UpdatePanel();
            motorControlManager.Open();
            motorControlManager.RebootAndStart();
            motorControlManager.ServoOn(1);
            motorControlManager.ServoOn(2);
            motorControlManager.ServoOn(3);

            PntData.position = 0;
            PntData.dwell = 0;                              /* 0ms */
            PntData.subcmd = SSC_SUBCMD_POS_ABS             /* Absolute Position */
                | SSC_SUBCMD_STOP_SMZ;                      /* Smoothing Stop */
            PntData.s_curve = 100;                          /* 100% */
            motorControlManager.SetPoint(PntData);
            Update_Speed();
            HomeReturn();

            positionUpdateTimer = new System.Windows.Forms.Timer();
            positionUpdateTimer.Interval = 50; // 50ms마다 위치 갱신
            positionUpdateTimer.Tick += PositionUpdate_Tick;
            positionUpdateTimer.Start();

            cameraManager.OnImageUpdated += (mat, count) =>
            {
                if (isProcessingImg)
                    return;
                // 기존 Mat 객체 삭제 (메모리 누수 방지)
                img?.Dispose();

                // Mat으로 변환하여 img에 저장
                img = mat.Clone();
                mat.Dispose();

                ShowImg();
                Invoke(new Action(() => label27.Text = count.ToString()));
            };

            imageProcessing.OnProcessingCompleted += (mat) =>
            {
                // 기존 Mat 객체 삭제 (메모리 누수 방지)
                img?.Dispose();

                // Mat으로 변환하여 img에 저장
                img = mat.Clone();

                ShowImg();
                isProcessingImg = true;
            };
            BFX_Connect();
            InitializeSerialPort();
            cameraManager.Start();

        }
        public void ShowImg()
        {
            Invoke(new Action(() =>
            {
                // 원본 이미지 크기 구하기
                int width = img.Width;
                int height = img.Height;

                // 중심 좌표 계산
                int centerX = width / 2;
                int centerY = height / 2;

                // 기존 이미지 삭제
                if (pictureBox1.Image != null)
                {
                    var oldImage = pictureBox1.Image;
                    pictureBox1.Image = null;
                    oldImage.Dispose();
                }
                Mat displayMat;

                if (isExpanded)
                {
                    // tb_Expanded의 값 가져오기 (예: 200% -> 2배 확대)
                    if (int.TryParse(tb_Expanded.Text, out int expandPercent))
                    {
                        double scaleFactor = expandPercent / 100.0; // 확대 비율
                        int cropWidth = (int)(width / scaleFactor);
                        int cropHeight = (int)(height / scaleFactor);

                        // 확대할 영역 계산 (중앙 확대)
                        int cropX = Math.Max(0, centerX - cropWidth / 2);
                        int cropY = Math.Max(0, centerY - cropHeight / 2);
                        cropWidth = Math.Min(cropWidth, width - cropX);
                        cropHeight = Math.Min(cropHeight, height - cropY);

                        // ROI(Region of Interest) 설정 후 잘라내기
                        Mat croppedMat = new Mat(img, new OpenCvSharp.Rect(cropX, cropY, cropWidth, cropHeight));

                        // PictureBox 크기에 맞춰 확대
                        displayMat = new Mat();
                        Cv2.Resize(croppedMat, displayMat, new OpenCvSharp.Size(width, height), 0, 0, InterpolationFlags.Linear);

                        // 메모리 해제
                        croppedMat.Dispose();
                    }
                    else
                    {
                        displayMat = img.Clone(); // 변환 실패 시 원본 유지
                    }
                }
                else
                {
                    displayMat = img.Clone();
                }
                // 확대된 이미지에 빨간색 십자가 그리기 (수직선은 확대된 후에 그림)
                Scalar redColor = new Scalar(0, 0, 255); // BGR 순서로 빨간색
                int thickness = 10; // 선 두께

                // 새 중심 좌표 계산
                int newCenterX = displayMat.Width / 2;
                int newCenterY = displayMat.Height / 2;

                // 가로선
                Cv2.Line(displayMat, new OpenCvSharp.Point(0, newCenterY), new OpenCvSharp.Point(displayMat.Width, newCenterY), redColor, thickness);

                // 세로선 (확대 후에 그림)
                Cv2.Line(displayMat, new OpenCvSharp.Point(newCenterX, 0), new OpenCvSharp.Point(newCenterX, displayMat.Height), redColor, thickness);

                // Mat을 Bitmap으로 변환하여 PictureBox에 표시
                Bitmap bitmap = BitmapConverter.ToBitmap(displayMat);
                pictureBox1.Image = bitmap;

                // 메모리 해제
                displayMat.Dispose();

            }));
        }
        public void SetAllValues(string[] values)
        {
            // 문자열 배열에서 값을 적절한 타입으로 변환
            PntData.speed = Convert.ToUInt32(values[ColumnIndex.SPEED]);
            PntData.actime = Convert.ToUInt16(values[ColumnIndex.ACTIME]);
            PntData.dctime = Convert.ToUInt16(values[ColumnIndex.DCTIME]);
            Constants.AREA_LOW = Convert.ToInt32(values[ColumnIndex.AREA_LOW]);
            Constants.AREA_HIGH = Convert.ToInt32(values[ColumnIndex.AREA_HIGH]);
            Constants.ROUND_LOW = Convert.ToInt32(values[ColumnIndex.ROUND_LOW]);
            Constants.ROUND_HIGH = Convert.ToInt32(values[ColumnIndex.ROUND_HIGH]);
            Constants.offset = Convert.ToDouble(values[ColumnIndex.OFFSET]);
            Constants.size = Convert.ToInt32(values[ColumnIndex.SIZE]);
            Constants.emptyCount = Convert.ToInt32(values[ColumnIndex.EMPTYCOUNT]);
            Constants.ExposureTime = Convert.ToInt32(values[ColumnIndex.EXPOSURETIME]);
            Constants.Dispensing_X = Convert.ToInt32(values[ColumnIndex.DISPENSING_X]);
            Constants.Dispensing_Y = Convert.ToInt32(values[ColumnIndex.DISPENSING_Y]);
            Constants.Dispensing_Z = Convert.ToInt32(values[ColumnIndex.DISPENSING_Z]);
            Constants.Dispensing_First_X = Convert.ToInt32(values[ColumnIndex.DISPENSING_FIRST_X]);
            Constants.Dispensing_First_Y = Convert.ToInt32(values[ColumnIndex.DISPENSING_FIRST_Y]);
            Constants.Dispensing_First_Z = Convert.ToInt32(values[ColumnIndex.DISPENSING_FIRST_Z]);
            Constants.Dispensing_Cam_X = Convert.ToInt32(values[ColumnIndex.DISPENSING_CAM_X]);
            Constants.Dispensing_Cam_Y = Convert.ToInt32(values[ColumnIndex.DISPENSING_CAM_Y]);
            Constants.Dispensing_Cam_Z = Convert.ToInt32(values[ColumnIndex.DISPENSING_CAM_Z]);
            Constants.Mold_X = Convert.ToInt32(values[ColumnIndex.MOLD_X]);
            Constants.Mold_Y = Convert.ToInt32(values[ColumnIndex.MOLD_Y]);
            Constants.Mold_Z = Convert.ToInt32(values[ColumnIndex.MOLD_Z]);

            Constants.Path = values[ColumnIndex.PATH];

            // 텍스트 박스에 값 설정
            tb_AREA_LOW.Text = Constants.AREA_LOW.ToString();
            tb_AREA_HIGH.Text = Constants.AREA_HIGH.ToString();
            tb_ROUND_LOW.Text = Constants.ROUND_LOW.ToString();
            tb_ROUND_HIGH.Text = Constants.ROUND_HIGH.ToString();
            tb_Offset.Text = Constants.offset.ToString();
            tb_SizeOfArr.Text = Constants.size.ToString();
            tb_EmptyCount.Text = Constants.emptyCount.ToString();
            tb_ExposureTime.Text = Constants.ExposureTime.ToString();
            tb_Dispensing_X.Text = Constants.Dispensing_X.ToString();
            tb_Dispensing_Y.Text = Constants.Dispensing_Y.ToString();
            tb_Dispensing_Z.Text = Constants.Dispensing_Z.ToString();
            tb_Dispensing_FirstX.Text = Constants.Dispensing_First_X.ToString();
            tb_Dispensing_FirstY.Text = Constants.Dispensing_First_Y.ToString();
            tb_Dispensing_FirstZ.Text = Constants.Dispensing_First_Z.ToString();
            tb_Dispensing_Cam_X.Text = Constants.Dispensing_Cam_X.ToString();
            tb_Dispensing_Cam_Y.Text = Constants.Dispensing_Cam_Y.ToString();
            tb_Dispensing_Cam_Z.Text = Constants.Dispensing_Cam_Z.ToString();
            tb_Mold_X.Text = Constants.Mold_X.ToString();
            tb_Mold_Y.Text = Constants.Mold_Y.ToString();
            tb_Mold_Z.Text = Constants.Mold_Z.ToString();

            tb_Path.Text = Constants.Path.ToString();

            tb_Distance_X.Text = (int.Parse(tb_Dispensing_FirstX.Text) - int.Parse(tb_Dispensing_Cam_X.Text)).ToString();
            tb_Distance_Y.Text = (int.Parse(tb_Dispensing_FirstY.Text) - int.Parse(tb_Dispensing_Cam_Y.Text)).ToString();
            tb_Distance_Z.Text = (int.Parse(tb_Dispensing_FirstZ.Text) - int.Parse(tb_Dispensing_Cam_Z.Text)).ToString();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadManager.Stop();
            motorControlManager.ServoOff(1);
            motorControlManager.ServoOff(2);
            motorControlManager.ServoOff(3);
            motorControlManager.Close();
            positionUpdateTimer.Stop();
        }
        private void BFX_Connect()
        {
            try
            {
                // gRPC 채널 생성
                channel = GrpcChannel.ForAddress("http://localhost:50055");

                // BFX-Client 서비스 클라이언트 생성
                bfxClient = new BFXClientService.BFXClientServiceClient(channel);

                var versionInfo = bfxClient.getVersionInfo(new GetVersionInfoRequest());
                Console.WriteLine($"BFX-Client Version: {versionInfo.VersionInfo}");

                showWindow = bfxClient.showWindow(new ShowWindowRequest { AlwaysOnTop = false });

                bfxClient.setDispenseFrequency(new SetDispenseFrequencyRequest { Frequency = 10 });
                bfxClient.setNumberOfDispenses(new SetNumberOfDispensesRequest { Channel = 1, NumberOfDispenses = 1 });
                BFXConnect = true;
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
            }
        }
        private void InitializeSerialPort()
        {
            try
            {
                serialPort = new SerialPort
                {
                    PortName = "COM7", // 사용할 포트 번호
                    BaudRate = 9600,   // 통신 속도 (장치에 맞게 설정)
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    //Encoding = Encoding.UTF8 // UTF-8 인코딩 사용
                };

                // 데이터 수신 이벤트 핸들러 등록
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();

                serialPort.Write("S100" + "\r" + "\n");
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // 수신된 데이터 읽기
                string receivedData = serialPort.ReadExisting().Trim(); // 공백 및 개행 제거
                Logger.Log("수신 데이터: " + receivedData);

                // "R1"로 시작하는 데이터인지 확인
                if (receivedData.StartsWith("R1") && receivedData.Length >= 4)
                {
                    string hexValue = receivedData.Substring(2, 2); // "R1" 뒤의 16진수 값 가져오기

                    if (int.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out int decimalValue))
                    {
                        Logger.Log($"16진수: {hexValue} → 10진수: {decimalValue}");

                        // UI 스레드에서 tb_Lamp 값 변경
                        this.Invoke(new Action(() =>
                        {
                            tb_Lamp.Text = decimalValue.ToString();
                        }));
                    }
                    else
                    {
                        Logger.Log("16진수 변환 실패: " + hexValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("데이터 수신 오류: " + ex.Message);
            }
        }

        private void PositionUpdate_Tick(object sender, EventArgs e)
        {
            int[] positions = new int[3];
            motorControlManager.GetCurrentPosition(positions);

            // UI 스레드에서 실행하도록 Invoke 사용
            if (tb_Position_X.InvokeRequired)
            {
                tb_Position_X.Invoke(new Action(() =>
                {
                    tb_Position_X.Text = positions[0].ToString();
                    tb_Position_Y.Text = positions[1].ToString();
                    tb_Position_Z.Text = positions[2].ToString();
                }));
            }
            else
            {
                tb_Position_X.Text = positions[0].ToString();
                tb_Position_Y.Text = positions[1].ToString();
                tb_Position_Z.Text = positions[2].ToString();
            }
        }

        private void Update_Speed()
        {
            tb_Speed_Now.Text = PntData.speed.ToString();
            tb_Tca_Now.Text = PntData.actime.ToString();
            tb_Tcd_Now.Text = PntData.dctime.ToString();
        }
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            threadManager.AddWorkerTask((token) =>
            {
                try
                {
                    motorControlManager.Open();
                    motorControlManager.RebootAndStart();
                    motorControlManager.SetPoint(PntData);
                }
                catch (Exception ex)
                {
                    // 에러 메시지 로그 출력 또는 UI 알림
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            });
        }
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            threadManager.AddWorkerTask((token) =>
            {
                try
                {
                    motorControlManager.Close();
                }
                catch (Exception ex)
                {
                    // 에러 메시지 로그 출력 또는 UI 알림
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            });
        }
        private void btn_SpeedSetUp_Click(object sender, EventArgs e)
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            if (uint.TryParse(tb_Speed_Target.Text, out uint speed) && ushort.TryParse(tb_Tca_Target.Text, out ushort tca) && ushort.TryParse(tb_Tcd_Target.Text, out ushort tcd))
            {
                PntData.speed = speed;
                PntData.actime = tca;
                PntData.dctime = tcd;
                threadManager.AddWorkerTask((token) => motorControlManager.SetPoint(PntData));
                Invoke(new Action(() =>
                {
                    Update_Speed();
                }));
                textFileManager.SetValue(ColumnIndex.SPEED, speed.ToString());
                textFileManager.SetValue(ColumnIndex.ACTIME, tca.ToString());
                textFileManager.SetValue(ColumnIndex.DCTIME, tcd.ToString());
            }
            else
            {
                MessageBox.Show("알맞은 값 확인");
                return;
            }

            //UI스레드 사용 
        }
        private void Servo_ON_OFF(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.Button button && button.Tag is string tagValue)
            {
                // 문자열 "1,0"을 ','로 분리하여 튜플로 변환
                var parts = tagValue.Split(',');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int axis) &&
                    int.TryParse(parts[1], out int direction))
                {
                    if (direction == 0)
                    {
                        threadManager.AddWorkerTask((token) => motorControlManager.ServoOn(axis));
                    }
                    else if (direction == 1)
                    {
                        threadManager.AddWorkerTask((token) => motorControlManager.ServoOff(axis));
                    }
                }
            }
        }
        private void CustomMove(object sender, EventArgs e)
        {
            if (isJogMode)
                return;
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            if (sender is System.Windows.Forms.Button button && button.Tag is string tagValue)
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
        private void JogButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isJogMode)
                return;
            if (sender is System.Windows.Forms.Button button && button.Tag is string tagValue)
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
            if (sender is System.Windows.Forms.Button button && button.Tag is string tagValue)
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

        private void btn_PositionMove_Click(object sender, EventArgs e)
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            if (int.TryParse(tb_PositionMove_X.Text, out int positionX) && int.TryParse(tb_PositionMove_Y.Text, out int positionY) && int.TryParse(tb_PositionMove_Z.Text, out int positionZ))
            {
                int[] position = { positionX, positionY, positionZ };
                threadManager.AddWorkerTask((token) => motorControlManager.Sequence(positionX, positionY, positionZ));
            }
            else
            {
                MessageBox.Show("알맞은 값 확인");
                return;
            }
        }
        private void btn_DistanceSet_Click(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.Button button && button.Tag is string tagValue)
            {
                tb_Distance.Text = tagValue;
            }
        }
        private void btn_EmergencyStop_Click(object sender, EventArgs e)
        {
            threadManager.EmergencyStop();
            motorControlManager.EmergencyStop();
        }

        private void HomeReturn()
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            Thread.Sleep(1000);
            threadManager.AddWorkerTask((token) => motorControlManager.HomeReturn());

        }

        private void button1_Click(object sender, EventArgs e)
        {
            isProcessingImg = false;
            cameraManager.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cameraManager.Stop();
        }

        private void btn_ImgSave_Click(object sender, EventArgs e)
        {
            imageProcessing.Init(img, motorControlManager);
            threadManager.AddWorkerTask((token) => imageProcessing.SaveMatImage(Constants.Path));
        }
        private void btn_Constants_Change_Click(object sender, EventArgs e)
        {
            textFileManager.SetValue(ColumnIndex.AREA_LOW, tb_AREA_LOW.Text);
            textFileManager.SetValue(ColumnIndex.AREA_HIGH, tb_AREA_HIGH.Text);
            textFileManager.SetValue(ColumnIndex.ROUND_LOW, tb_ROUND_LOW.Text);
            textFileManager.SetValue(ColumnIndex.ROUND_HIGH, tb_ROUND_HIGH.Text);
            textFileManager.SetValue(ColumnIndex.OFFSET, tb_Offset.Text);
            textFileManager.SetValue(ColumnIndex.SIZE, tb_SizeOfArr.Text);
            textFileManager.SetValue(ColumnIndex.EMPTYCOUNT, tb_EmptyCount.Text);
            textFileManager.SetValue(ColumnIndex.EXPOSURETIME, tb_ExposureTime.Text);
            SetAllValues(textFileManager.GetAllValues());
            cameraManager.SetExposureTime(int.Parse(tb_ExposureTime.Text.ToString()));
            MessageBox.Show("변경 완료");
        }

        private void btn_Expanded_Click(object sender, EventArgs e)
        {
            // isExpanded 상태 토글
            isExpanded = !isExpanded;

            // 버튼의 눌린 현상 추가
            if (isExpanded)
            {
                btn_Expanded.FlatStyle = FlatStyle.Popup; // 눌린 스타일
                btn_Expanded.BackColor = SystemColors.ControlDark; // 눌린 배경색
                tb_Expanded.ReadOnly = true;
            }
            else
            {
                btn_Expanded.FlatStyle = FlatStyle.Standard; // 기본 스타일
                btn_Expanded.BackColor = SystemColors.ControlLight; // 기본 배경색
                //btn_Expanded.Text = "확장"; // 버튼 텍스트 변경
                tb_Expanded.ReadOnly = false;
            }
            ShowImg();
        }

        private void btn_Fullscreen_Click(object sender, EventArgs e)
        {
            // img를 Clone하여 새로운 Mat 생성
            Mat resizedImg = new Mat();
            Cv2.Resize(img, resizedImg, new OpenCvSharp.Size(1000, 1000), 0, 0, InterpolationFlags.Linear);

            // OpenCV 창 이름 설정
            string windowName = "Resized Image";

            // 창을 생성 (일반 창)
            Cv2.NamedWindow(windowName, WindowFlags.AutoSize);

            // 이미지 표시
            Cv2.ImShow(windowName, resizedImg);

            // ESC 키 입력 시 창 닫기
            while (true)
            {
                int key = Cv2.WaitKey(1); // 1ms 대기하면서 키 입력 감지
                if (key == 27) // ESC 키(ASCII 코드 27) 감지
                {
                    Cv2.DestroyWindow(windowName);
                    break;
                }
            }

            // 메모리 해제
            resizedImg.Dispose();
        }

        private void btn_PathSetting_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // 폴더 선택 창 설정
                folderBrowserDialog.Description = "폴더를 선택하세요";
                folderBrowserDialog.ShowNewFolderButton = false; // 새 폴더 생성 버튼 비활성화

                // 폴더 선택 후 확인 버튼을 눌렀을 때
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    tb_Path.Text = folderBrowserDialog.SelectedPath;
                    Constants.Path = folderBrowserDialog.SelectedPath;
                    textFileManager.SetValue(ColumnIndex.PATH, tb_Path.Text);
                }
            }
        }

        //private void button7_Click(object sender, EventArgs e)
        //{
        //    // 텍스트박스 값 읽기
        //    string name = tb_SaveName.Text;
        //    string P_X = tb_Position_X.Text;
        //    string P_Y = tb_Position_Y.Text;
        //    string P_Z = tb_Position_Z.Text;

        //    // 값이 비어있는지 확인
        //    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(P_X) || string.IsNullOrEmpty(P_Y) || string.IsNullOrEmpty(P_Z))
        //    {
        //        MessageBox.Show("모든 입력 필드를 채워주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return; // 값이 비어있다면 함수 종료
        //    }

        //    // 값이 비어있지 않으면 배열에 저장
        //    string[] val = new string[4];
        //    val[0] = name;
        //    val[1] = P_X;
        //    val[2] = P_Y;
        //    val[3] = P_Z;

        //    // SetValue 함수 호출
        //    textFileManager.SetValue(ColumnIndex.SAVEXYZ, val);

        //    //UpdatePanel();
        //    tb_SaveName.Text = "";
        //    //TextBox textBox = new TextBox();
        //    //textBox.Location = new System.Drawing.Point(100, currentY);

        //    //Button button = new Button();
        //    //button.Text = "버튼 " + currentY;
        //    //button.Location = new System.Drawing.Point(250, currentY);

        //}
        //private void UpdatePanel()
        //{
        //    // 기존 컨트롤 초기화
        //    panel_SaveXYZ.Controls.Clear();

        //    // GetValue_XYZ()로 값을 받음
        //    string[][] xyzValues = textFileManager.GetValue_XYZ();

        //    if (xyzValues == null)
        //    {
        //        return; // 값이 없다면 아무 것도 추가하지 않음
        //    }

        //    // 각 항목에 대해 라벨과 버튼 추가
        //    int yOffset = 10; // 시작 위치

        //    for (int i = 0; i < xyzValues.Length; i++)
        //    {
        //        // XYZ 값
        //        string name = xyzValues[i][0];
        //        string pX = xyzValues[i][1];
        //        string pY = xyzValues[i][2];
        //        string pZ = xyzValues[i][3];

        //        // 라벨 1: Name
        //        Label lblName = new Label();
        //        lblName.Text = name;
        //        lblName.Size = new System.Drawing.Size(70, 20); // 너비 70, 높이 20
        //        lblName.Location = new System.Drawing.Point(10, yOffset);
        //        panel_SaveXYZ.Controls.Add(lblName);

        //        // 라벨 2: X
        //        Label lblX = new Label();
        //        lblX.Text = pX;
        //        lblX.AutoSize = true;
        //        lblX.Size = new System.Drawing.Size(30, 20); // 너비 50, 높이 20
        //        lblX.Location = new System.Drawing.Point(80, yOffset);
        //        panel_SaveXYZ.Controls.Add(lblX);

        //        // 라벨 3: Y
        //        Label lblY = new Label();
        //        lblY.Text = pY;
        //        lblY.AutoSize = true;
        //        lblY.Size = new System.Drawing.Size(30, 20);
        //        lblY.Location = new System.Drawing.Point(125, yOffset);
        //        panel_SaveXYZ.Controls.Add(lblY);

        //        // 라벨 4: Z
        //        Label lblZ = new Label();
        //        lblZ.Text = pZ;
        //        lblZ.AutoSize = true;
        //        lblZ.Size = new System.Drawing.Size(30, 20);
        //        lblZ.Location = new System.Drawing.Point(170, yOffset);
        //        panel_SaveXYZ.Controls.Add(lblZ);

        //        // 이동 버튼
        //        System.Windows.Forms.Button btnMove = new System.Windows.Forms.Button();
        //        btnMove.Text = "이동";
        //        btnMove.Size = new System.Drawing.Size(40, 20);
        //        btnMove.Tag = new string[] { pX, pY, pZ }; // X, Y, Z 값을 Tag에 저장
        //        btnMove.Location = new System.Drawing.Point(210, yOffset - 5);
        //        btnMove.Click += SequenceMove_Click; // 이벤트 핸들러 연결
        //        panel_SaveXYZ.Controls.Add(btnMove);

        //        // 삭제 버튼
        //        System.Windows.Forms.Button btnDelete = new System.Windows.Forms.Button();
        //        btnDelete.Text = "삭제";
        //        btnDelete.Size = new System.Drawing.Size(40, 20);
        //        btnDelete.Tag = i; // i(인덱스)를 Tag에 저장
        //        btnDelete.Location = new System.Drawing.Point(260, yOffset - 5);
        //        btnDelete.Click += DeleteXYZ_Click; // 이벤트 핸들러 연결
        //        panel_SaveXYZ.Controls.Add(btnDelete);

        //        // Y축 위치 업데이트 (다음 항목을 아래로 배치)
        //        yOffset += 30; // 각 항목마다 30px씩 아래로 내려감
        //    }

        //}
        private void btn_Position_Move_Click(object sender, EventArgs e)
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            if (sender is System.Windows.Forms.Button button && button.Tag is string tagValue)
            {
                int num = int.Parse(tagValue);
                int dx, dy, dz;
                switch (num)
                {
                    case 0:
                        dx = int.Parse(tb_Dispensing_X.Text);
                        dy = int.Parse(tb_Dispensing_Y.Text);
                        dz = int.Parse(tb_Dispensing_Z.Text);
                        break;
                    case 1:
                        dx = int.Parse(tb_Dispensing_FirstX.Text);
                        dy = int.Parse(tb_Dispensing_FirstY.Text);
                        dz = int.Parse(tb_Dispensing_FirstZ.Text);
                        break;
                    case 2:
                        dx = int.Parse(tb_Dispensing_Cam_X.Text);
                        dy = int.Parse(tb_Dispensing_Cam_Y.Text);
                        dz = int.Parse(tb_Dispensing_Cam_Z.Text);
                        break;
                    case 3:
                        dx = int.Parse(tb_Mold_X.Text);
                        dy = int.Parse(tb_Mold_Y.Text);
                        dz = int.Parse(tb_Mold_Z.Text);
                        break;
                    default:
                        return;
                }
                threadManager.AddWorkerTask((token) => motorControlManager.Sequence(dx, dy, dz));

            }

        }
        private void btn_Position_Save_Click(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.Button button && button.Tag is string tagValue)
            {
                int num = int.Parse(tagValue);
                string dx, dy, dz;
                dx = tb_Position_X.Text;
                dy = tb_Position_Y.Text;
                dz = tb_Position_Z.Text;
                switch (num)
                {
                    case 0:
                        textFileManager.SetValue(ColumnIndex.DISPENSING_X, dx);
                        textFileManager.SetValue(ColumnIndex.DISPENSING_Y, dy);
                        textFileManager.SetValue(ColumnIndex.DISPENSING_Z, dz);
                        tb_Dispensing_X.Text = dx;
                        tb_Dispensing_Y.Text = dy;
                        tb_Dispensing_Z.Text = dz;
                        break;
                    case 1:
                        textFileManager.SetValue(ColumnIndex.DISPENSING_FIRST_X, dx);
                        textFileManager.SetValue(ColumnIndex.DISPENSING_FIRST_Y, dy);
                        textFileManager.SetValue(ColumnIndex.DISPENSING_FIRST_Z, dz);
                        tb_Dispensing_FirstX.Text = dx;
                        tb_Dispensing_FirstY.Text = dy;
                        tb_Dispensing_FirstZ.Text = dz;
                        break;
                    case 2:
                        textFileManager.SetValue(ColumnIndex.DISPENSING_CAM_X, dx);
                        textFileManager.SetValue(ColumnIndex.DISPENSING_CAM_Y, dy);
                        textFileManager.SetValue(ColumnIndex.DISPENSING_CAM_Z, dz);
                        tb_Dispensing_Cam_X.Text = dx;
                        tb_Dispensing_Cam_Y.Text = dy;
                        tb_Dispensing_Cam_Z.Text = dz;
                        break;
                    case 3:
                        textFileManager.SetValue(ColumnIndex.MOLD_X, dx);
                        textFileManager.SetValue(ColumnIndex.MOLD_Y, dy);
                        textFileManager.SetValue(ColumnIndex.MOLD_Z, dz);
                        tb_Mold_X.Text = dx;
                        tb_Mold_Y.Text = dy;
                        tb_Mold_Z.Text = dz;
                        break;
                    default:
                        Logger.Log("Save Fail.");
                        return;
                }

            }

        }
        private void SequenceMove_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            string[] coordinates = btn.Tag as string[];

            if (coordinates != null)
            {
                int pX = int.Parse(coordinates[0]);
                int pY = int.Parse(coordinates[1]);
                int pZ = int.Parse(coordinates[2]);

                if (threadManager.IsWorkerRunning())
                {
                    MessageBox.Show("작업 중");
                    return;
                }
                threadManager.AddWorkerTask((token) =>
                {
                    try
                    {
                        motorControlManager.Sequence(pX, pY, pZ);
                    }
                    catch (Exception ex)
                    {
                        // 에러 메시지 로그 출력 또는 UI 알림
                        Debug.WriteLine($"Error: {ex.Message}");
                    }
                });
            }
        }
        private void DeleteXYZ_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null || btn.Tag == null)
                return;

            int index = (int)btn.Tag; // 버튼에 저장된 인덱스 가져오기

            textFileManager.DeleteXYZ(index); // 파일에서 해당 항목 삭제
            //UpdatePanel(); // UI 갱신
        }

        private void btn_HomeReturn_Click(object sender, EventArgs e)
        {
            HomeReturn();
        }

        private void btn_ImgProcessing_Click(object sender, EventArgs e)
        {
            if (img == null || img.Empty())
            {
                MessageBox.Show("Error: 이미지가 비어 있습니다.");
                return;
            }
            if (isProcessingImg)
            {
                MessageBox.Show("Error: 이미 처리된 이미지입니다.");
                return;
            }
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            threadManager.AddWorkerTask((token) =>
            {
                try
                {
                    //cameraManager.Stop();
                    //Thread.Sleep(500);
                    Logger.Log("Img imageProcessing Start");
                    if (dpTimer.Enabled)
                    {
                        Logger.Log("dpTimer.Stop");
                        dpTimer.Stop();
                        if (btn_Dispensing_Timer_Stop.InvokeRequired)
                        {
                            btn_Dispensing_Timer_Stop.Invoke(new Action(() =>
                            {
                                btn_Dispensing_Timer_Stop.Enabled = false;
                            }));
                        }
                        Thread.Sleep(500);
                    }
                    int[] currentPosition = new int[3];
                    currentPosition[0] = int.Parse(tb_Position_X.Text);
                    currentPosition[1] = int.Parse(tb_Position_Y.Text);
                    currentPosition[2] = int.Parse(tb_Position_Z.Text);
                    int delay = int.Parse(tb_Processing_delay.Text);
                    int N = int.Parse(tb_MoldCount.Text);
                    int MoldSpacing = int.Parse(tb_MoldSpacing.Text);
                    OpenCvSharp.Point[][] SelectedPoints = new OpenCvSharp.Point[N][];
                    SelectedPoints_Test = new OpenCvSharp.Point[N][];
                    for (int i = 0; i < N; i++)
                    {
                        if (token.IsCancellationRequested) return;
                        isProcessingImg = true;
                        SelectedPoints[i] = new OpenCvSharp.Point[97];
                        imageProcessing.Init(img, motorControlManager);
                        imageProcessing.SaveMatImage(Constants.Path);
                        imageProcessing.Start(ref SelectedPoints[i]); //요따가 위치 저장할 배열?
                        if (i == N - 1)
                        {
                            break;
                        }
                        //이동수행
                        motorControlManager.CustomMove(2, MoldSpacing);
                        //이동 완료시, isProcessingImg = false로 바꿈
                        isProcessingImg = false;
                        //img update시, 다음 반복문 실행
                        Thread.Sleep(3000);
                    }
                    //isProcessingImg = false;
                    if (N != 1)
                    {
                        motorControlManager.CustomMove(2, MoldSpacing * -(N - 1));
                        Thread.Sleep(2000);
                    }
                    Thread.Sleep(1000);
                    int[] camera_dispenser_distance = new int[3];
                    camera_dispenser_distance[0] = int.Parse(tb_Distance_X.Text);
                    camera_dispenser_distance[1] = int.Parse(tb_Distance_Y.Text);
                    camera_dispenser_distance[2] = int.Parse(tb_Distance_Z.Text) - int.Parse(tb_Position_Z.Text);
                    //int test = 400;
                    //camera_dispenser_distance[2] = int.Parse(tb_Distance_Z.Text) + test;

                    imageProcessing.NumberAndMove(SelectedPoints, MoldSpacing, camera_dispenser_distance, dispenserCommandResponse, bfxClient, delay, token);
                    //// 저장된 배열을 매개변수로 받는 이동(배열, 설정한 카메라-디스펜서값) - 이 함수에 디스펜싱 필요할듯?
                    //SelectedPoints_Test = SelectedPoints;
                    motorControlManager.Sequence(currentPosition[0], currentPosition[1], currentPosition[2]);
                    isProcessingImg = false;

                }
                catch (Exception ex)
                {
                    // 에러 메시지 로그 출력 또는 UI 알림
                    Logger.Log($"Error: {ex.Message}");
                    threadManager.EmergencyStop();
                    motorControlManager.EmergencyStop();
                    isProcessingImg = false;
                }
            });

        }

        private void btn_ExpansionCustom_Click(object sender, EventArgs e)
        {
            CustomMove moveForm = new CustomMove(threadManager, motorControlManager);
            moveForm.Show();
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

        private void button3_Click(object sender, EventArgs e)
        {


        }

        private void btn_Dispansing_Ones_Click(object sender, EventArgs e)
        {
            try
            {
                //int N = 1;
                //float frequency = 10;
                //bfxClient.setDispenseFrequency(new SetDispenseFrequencyRequest { Frequency = frequency });
                //bfxClient.setNumberOfDispenses(new SetNumberOfDispensesRequest { Channel = 1, NumberOfDispenses = N });
                //playing = await bfxClient.executeDispenseCommandAndWaitAsync(new DispenseAndWaitCommandRequest());
                dispenserCommandResponse = bfxClient.executeDispenserCommand(new DispenserCommandRequest { Name = "dispense", Cmd = "i" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        private void btn_Distance_Setting_Click(object sender, EventArgs e)
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            int pX = int.Parse(tb_Dispensing_FirstX.Text);
            int pY = int.Parse(tb_Dispensing_FirstY.Text);
            int pZ = int.Parse(tb_Dispensing_FirstZ.Text);
            int pX2 = int.Parse(tb_Dispensing_Cam_X.Text);
            int pY2 = int.Parse(tb_Dispensing_Cam_Y.Text);
            int pZ2 = int.Parse(tb_Dispensing_Cam_Z.Text);
            // 테스트 디스펜싱 위치로 이동 후 디스펜싱. 이후 카메라 위치 이동
            threadManager.AddWorkerTask((token) =>
            {
                try
                {
                    //tb_Distance_X.Enabled = false;
                    //tb_Distance_Y.Enabled = false;
                    //tb_Distance_Z.Enabled = false;


                    motorControlManager.Sequence(pX, pY, pZ);
                    Thread.Sleep(500);

                    dispenserCommandResponse = bfxClient.executeDispenserCommand(new DispenserCommandRequest { Name = "dispense", Cmd = "i" });
                    Thread.Sleep(500);


                    motorControlManager.Sequence(pX2, pY2, pZ2);
                    btn_Distance_OK.Enabled = true;
                }
                catch (Exception ex)
                {
                    // 에러 메시지 로그 출력 또는 UI 알림
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            });
            btn_Distance_OK.Enabled = true;
        }
        private void btn_Distance_Setting_Auto_Click(object sender, EventArgs e)
        {
            if (threadManager.IsWorkerRunning())
            {
                MessageBox.Show("작업 중");
                return;
            }
            int pX = int.Parse(tb_Dispensing_FirstX.Text);
            int pY = int.Parse(tb_Dispensing_FirstY.Text);
            int pZ = int.Parse(tb_Dispensing_FirstZ.Text);
            int pX2 = int.Parse(tb_Dispensing_Cam_X.Text);
            int pY2 = int.Parse(tb_Dispensing_Cam_Y.Text);
            int pZ2 = int.Parse(tb_Dispensing_Cam_Z.Text);
            threadManager.AddWorkerTask((token) =>
            {
                try
                {

                    motorControlManager.Sequence(pX, pY, pZ);
                    Thread.Sleep(500);

                    dispenserCommandResponse = bfxClient.executeDispenserCommand(new DispenserCommandRequest { Name = "dispense", Cmd = "i" });
                    Thread.Sleep(500);

                    motorControlManager.Sequence(pX2, pY2, pZ2);
                    Thread.Sleep(1000);

                    Mat mat = img.Clone();
                    int[] position = new int[2];
                    position = imageProcessing.Setting_Auto(mat);
                    if (position[0] == -1 && position[1] == -1)
                    {
                        Logger.Log("Circle not detected");
                        return;
                    }
                    int width = mat.Width;
                    int height = mat.Height;

                    // 중심 좌표 계산
                    int centerX = width / 2;
                    int centerY = height / 2;

                    int currentPositionX = position[0] - centerX;
                    int currentPositionY = position[1] - centerY;

                    double pixelToUm = 4.928;

                    int moveX = (int)(currentPositionX * pixelToUm);
                    int moveY = (int)(currentPositionY * pixelToUm);

                    motorControlManager.CustomMove(2, moveX);
                    motorControlManager.CustomMove(1, moveY);
                    //mat.Dispose();
                }
                catch (Exception ex)
                {
                    // 에러 메시지 로그 출력 또는 UI 알림
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        private void btn_Distance_OK_Click(object sender, EventArgs e)
        {
            //tb_Dispensing_Cam_X.Text = tb_Position_X.Text;
            //tb_Dispensing_Cam_Y.Text = tb_Position_Y.Text;
            //tb_Dispensing_Cam_Z.Text = tb_Position_Z.Text;

            tb_Distance_X.Text = (int.Parse(tb_Dispensing_FirstX.Text) - int.Parse(tb_Dispensing_Cam_X.Text)).ToString();
            tb_Distance_Y.Text = (int.Parse(tb_Dispensing_FirstY.Text) - int.Parse(tb_Dispensing_Cam_Y.Text)).ToString();
            tb_Distance_Z.Text = tb_Dispensing_FirstZ.Text;
        }

        //private void btn_Distance_Save_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        tb_Distance_X.Text = (int.Parse(tb_Dispensing_FirstX.Text) - int.Parse(tb_Dispensing_Cam_X.Text)).ToString();
        //        tb_Distance_Y.Text = (int.Parse(tb_Dispensing_FirstY.Text) - int.Parse(tb_Dispensing_Cam_Y.Text)).ToString();
        //        tb_Distance_Z.Text = (int.Parse(tb_Dispensing_FirstZ.Text) - int.Parse(tb_Dispensing_Cam_Z.Text)).ToString();

        //        textFileManager.SetValue(ColumnIndex.DISPENSING_X, tb_Dispensing_FirstX.Text);
        //        textFileManager.SetValue(ColumnIndex.DISPENSING_Y, tb_Dispensing_FirstY.Text);
        //        textFileManager.SetValue(ColumnIndex.DISPENSING_Z, tb_Dispensing_FirstZ.Text);
        //        textFileManager.SetValue(ColumnIndex.DISPENSING_CAM_X, tb_Dispensing_Cam_X.Text);
        //        textFileManager.SetValue(ColumnIndex.DISPENSING_CAM_Y, tb_Dispensing_Cam_Y.Text);
        //        textFileManager.SetValue(ColumnIndex.DISPENSING_CAM_Z, tb_Dispensing_Cam_Z.Text);
        //        MessageBox.Show("저장 완료");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}

        private void btn_Dispensing_Position_Set_Click(object sender, EventArgs e)
        {
            tb_Dispensing_FirstX.Text = tb_Position_X.Text;
            tb_Dispensing_FirstY.Text = tb_Position_Y.Text;
            tb_Dispensing_FirstZ.Text = tb_Position_Z.Text;
        }

        private void btn_Dispensing_Cam_Position_Set_Click(object sender, EventArgs e)
        {
            tb_Dispensing_Cam_X.Text = tb_Position_X.Text;
            tb_Dispensing_Cam_Y.Text = tb_Position_Y.Text;
            tb_Dispensing_Cam_Z.Text = tb_Position_Z.Text;
        }

        private void btn_Distance_Setting_Z_Click(object sender, EventArgs e)
        {
            int[] camera_dispenser_distance = new int[3];
            camera_dispenser_distance[0] = int.Parse(tb_Dispensing_FirstX.Text);
            camera_dispenser_distance[1] = int.Parse(tb_Dispensing_FirstY.Text);
            camera_dispenser_distance[2] = int.Parse(tb_Dispensing_FirstZ.Text);

            motorControlManager.Sequence(camera_dispenser_distance[0], camera_dispenser_distance[1], camera_dispenser_distance[2]);
            //btn_Distance_OK.Enabled = true;

        }

        private void btn_Dispencing_Timer_Click(object sender, EventArgs e)
        {
            //bfxClient.setNumberOfDispenses(new SetNumberOfDispensesRequest { Channel = piezoChannel, NumberOfDispenses = N });
            try
            {
                dpTimer.Interval = int.Parse(tb_Dispensing_delay.Text);
                dpTimer.Start();
                btn_Dispensing_Timer_Stop.Enabled = true;

            }
            catch (Exception ex) { }

        }

        private void dpTimer_Tick(object sender, EventArgs e)
        {
            if (BFXConnect)
            {
                dispenserCommandResponse = bfxClient.executeDispenserCommand(new DispenserCommandRequest { Name = "dispense", Cmd = "i" });
            }
        }

        private void btn_Dispensing_Timer_Stop_Click(object sender, EventArgs e)
        {
            dpTimer.Stop();
            btn_Dispensing_Timer_Stop.Enabled = false;
        }

        private void btn_Lamp_Change_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    MessageBox.Show("시리얼 포트가 열려 있지 않습니다.");
                    return;
                }

                // tb_Lamp.Text 값을 16진수로 변환
                if (int.TryParse(tb_Lamp.Text, out int decimalValue))
                {
                    // 16진수 2자리 문자열로 변환 (예: "255" -> "FF")
                    string hexValue = decimalValue.ToString("X2");

                    // "C1XX[CR][LF]" 형식으로 전송
                    string sendData = $"C1{hexValue}\r\n";
                    serialPort.Write(sendData);

                    Logger.Log($"송신 데이터: {sendData}");
                }
                else
                {
                    MessageBox.Show("올바른 숫자를 입력하세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("송신 오류: " + ex.Message);
            }
        }

        private void btn_BFX_Capture_Click(object sender, EventArgs e)
        {
            bfxClient.setCameraRotation(new SetCameraRotationRequest { CameraRotation = (int)gRPCBfx.CAM_ROTATION.Rotate90Clockwise });
            bfxClient.captureReferenceImage(new CaptureReferenceImageRequest());
            var roi = bfxClient.getROI(new GetROIRequest());
            bfxClient.setLeftROI(new SetLeftROIRequest { LeftROI = roi.LeftROI - 100 });
            bfxClient.setRightROI(new SetRightROIRequest { RightROI = roi.RightROI + 100 });
        }
    }
    static class Constants
    {
        public static int AREA_LOW { get; set; }
        public static int AREA_HIGH { get; set; }
        public static int ROUND_LOW { get; set; }
        public static int ROUND_HIGH { get; set; }
        public static double offset { get; set; }
        public static int size { get; set; }
        public static int emptyCount { get; set; }
        public static int ExposureTime { get; set; }
        public static int Dispensing_X { get; set; }
        public static int Dispensing_Y { get; set; }
        public static int Dispensing_Z { get; set; }
        public static int Dispensing_First_X { get; set; }
        public static int Dispensing_First_Y { get; set; }
        public static int Dispensing_First_Z { get; set; }
        public static int Dispensing_Cam_X { get; set; }
        public static int Dispensing_Cam_Y { get; set; }
        public static int Dispensing_Cam_Z { get; set; }
        public static int Mold_X { get; set; }
        public static int Mold_Y { get; set; }
        public static int Mold_Z { get; set; }
        public static string Path { get; set; }

    }
}
