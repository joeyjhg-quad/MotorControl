using System;
using System.IO;
using System.Windows.Forms;
using log4net;
using log4net.Config;

namespace MotorControl
{
    public static class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));
        private static Main _form;

        static Logger()
        {
            // log4net 설정 로드 (log4net.config 파일 직접 지정)
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            var configFile = new FileInfo("log4net.config");
            XmlConfigurator.Configure(logRepository, configFile);
        }

        // Form 등록
        public static void Initialize(Main form)
        {
            _form = form;
        }

        // 일반 로그 출력
        public static void Log(string message)
        {
            log.Info(message);  // log4net 로그 기록

            if (_form == null || _form.IsDisposed) return;

            if (_form.InvokeRequired)
            {
                _form.Invoke(new Action(() => LogToUI(message)));
                return;
            }

            LogToUI(message);
        }

        // 에러 로그 출력
        public static void LogError(string message, Exception ex = null)
        {
            if (ex != null)
                log.Error(message, ex);
            else
                log.Error(message);

            if (_form == null || _form.IsDisposed) return;

            if (_form.InvokeRequired)
            {
                _form.Invoke(new Action(() => LogToUI($"[ERROR] {message}")));
                return;
            }

            LogToUI($"[ERROR] {message}");
        }

        // UI에 로그 출력 (richTextBox1)
        private static void LogToUI(string message)
        {
            if (_form == null || _form.IsDisposed) return;

            _form.richTextBox1.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\r\n");

            // 가장 아래로 스크롤
            _form.richTextBox1.SelectionStart = _form.richTextBox1.Text.Length;
            _form.richTextBox1.ScrollToCaret();
        }
    }
}
