using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Basler.Pylon;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace MotorControl
{
    public class CameraManager
    {
        private CancellationTokenSource _cancellationTokenSource;

        // 이벤트: 이미지 업데이트 시 발생
        public event Action<Mat, int> OnImageUpdated;
        int time = 0;
        bool SetExposure = false;
        // 카메라 이미지 캡처 시작
        public void Start()
        {
            if (_cancellationTokenSource != null)
            {
                Logger.Log("CameraManager is already running.");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            Task.Run(() =>
            {
                try
                {
                    using (Camera camera = new Camera())
                    {
                        camera.CameraOpened += Configuration.AcquireContinuous;
                        camera.Open();
                        camera.Parameters[PLGigECamera.Width].SetValue(2056);
                        camera.Parameters[PLGigECamera.Height].SetValue(2052);
                        camera.Parameters[PLGigECamera.CenterX].SetValue(true);

                        camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByUser);
                        int count = 0;
                        Stopwatch stopwatch = new Stopwatch();
                        Logger.Log("camera open");

                        while (!token.IsCancellationRequested)
                        {
                            stopwatch.Restart();

                            // GrabResult 가져오기
                            IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);

                            using (grabResult)
                            {
                                if (grabResult.GrabSucceeded)
                                {
                                    // GrabResult를 Mat으로 변환
                                    Mat mat = ConvertToMat(grabResult);

                                    // Mat 리사이즈
                                    //Cv2.Resize(mat, mat, new OpenCvSharp.Size(520, 520), 0, 0, InterpolationFlags.Linear);

                                    // Mat을 Bitmap으로 변환
                                    //Bitmap bitmap = BitmapConverter.ToBitmap(mat);

                                    // 이벤트를 통해 UI로 전달
                                    OnImageUpdated?.Invoke(mat.Clone(), count);

                                    mat.Dispose();
                                }
                                else
                                {
                                    Logger.Log($"Error: {grabResult.ErrorCode} {grabResult.ErrorDescription}");
                                }
                            }

                            count++;
                            if (SetExposure)
                            {
                                camera.Parameters[PLGigECamera.ExposureTimeAbs].SetValue(time);
                                SetExposure = false;
                            }
                        }

                        camera.StreamGrabber.Stop();
                        camera.Close();
                        Logger.Log("camera close");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Exception: {ex.Message}");
                }
            }, token);
        }

        // 카메라 이미지 캡처 중지
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private Mat ConvertToMat(IGrabResult grabResult)
        {
            PixelDataConverter converter = new PixelDataConverter
            {
                OutputPixelFormat = PixelType.BGRA8packed
            };

            Mat mat = new Mat(grabResult.Height, grabResult.Width, MatType.CV_8UC4);
            IntPtr matDataPointer = mat.Data;

            converter.Convert(matDataPointer, mat.Total() * mat.ElemSize(), grabResult);
            return mat;
        }

        public void SetExposureTime(int time)
        {
            Logger.Log($"set ExposureTime {time}");
            this.time = time;
            SetExposure = true;
        }
    }
}
