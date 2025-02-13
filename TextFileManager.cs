using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
/*
 * 열  - 변수명      - 기본값
 * 0   - speed       - 300
 * 1   - actime      - 1000
 * 2   - dctime      - 1000
 * 3   - AREA_LOW    - 2300
 * 4   - AREA_HIGH   - 3800
 * 5   - ROUND_LOW   - 170
 * 6   - ROUND_HIGH  - 320
 * 7   - offset      - 161.8
 * 8   - N*N         - 11
 * 9   - EmptyCount  - 3
 * 10  - ExposureTime  - 3000
 * 11  - Path        - EMPTY
 * 12  - SaveXYZ     - 0 (이만큼 for문 반복)
 */
namespace MotorControl
{
    internal class TextFileManager
    {
        private string filePath = "data.txt";
        public TextFileManager()
        {
            EnsureFileExists();
        }
        private void EnsureFileExists()
        {
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("speed 300");
                    writer.WriteLine("actime 1000");
                    writer.WriteLine("dctime 1000");
                    writer.WriteLine("AREA_LOW 2300");
                    writer.WriteLine("AREA_HIGH 3800");
                    writer.WriteLine("ROUND_LOW 170");
                    writer.WriteLine("ROUND_HIGH 320");
                    writer.WriteLine("offset 161.8");
                    writer.WriteLine("N*N 11");
                    writer.WriteLine("EmptyCount 3");
                    writer.WriteLine("ExposureTime 3000");
                    writer.WriteLine("Path EMPTY");
                    writer.WriteLine("Dispensing_X 0");
                    writer.WriteLine("Dispensing_Y 0");
                    writer.WriteLine("Dispensing_Z 0");
                    writer.WriteLine("Dispensing_Cam_X 0");
                    writer.WriteLine("Dispensing_Cam_Y 0");
                    writer.WriteLine("Dispensing_Cam_Z 0");
                    writer.WriteLine("SaveXYZ 0");
                }
            }
        }


        public string GetValue_String(int columnIndex)
        {
            var lines = File.ReadAllLines(filePath);
            var values = lines[columnIndex].Split(' '); // 열마다 값이 공백으로 구분된다고 가정
            return Convert.ToString(values[1]); // 두 번째 값 (기본값) 반환
        }
        public string[][] GetValue_XYZ()
        {
            var lines = File.ReadAllLines(filePath);
            var values = lines[ColumnIndex.SAVEXYZ].Split(' '); // 'SaveXYZ' 라인 읽기
            if (values[1] == "0")
            {
                return null; // 만약 0이라면 값이 없다는 의미로 null 반환
            }

            int count = Convert.ToInt32(values[1]); // 두 번째 값 (항목 개수) 사용

            // 2D 배열로 각 항목의 값을 저장할 준비
            string[][] xyzValues = new string[count][];

            for (int i = 0; i < count; i++)
            {
                // 각 항목의 라인을 읽고 값을 저장
                string[] xyz = lines[ColumnIndex.SAVEXYZ + 1 + i].Split(' ');
                xyzValues[i] = xyz; // 값을 배열에 할당
            }

            return xyzValues; // 반환
        }

        public void SetValue(int columnIndex, string newValue)
        {
            var lines = File.ReadAllLines(filePath).ToList();
            var values = lines[columnIndex].Split(' ');

            values[1] = newValue;

            // 수정된 값을 다시 파일에 저장
            lines[columnIndex] = string.Join(" ", values);
            File.WriteAllLines(filePath, lines);
        }
        public void SetValue(int columnIndex, string[] newValue)
        {
            // 파일의 모든 줄 읽어오기
            var lines = File.ReadAllLines(filePath).ToList();

            // 지정된 columnIndex에서 해당 라인 가져오기
            var line = lines[columnIndex];

            // "SaveXYZ" 뒤에 있는 숫자 추출하기 (공백 기준으로 split)
            var parts = line.Split(' ');

            if (parts[0].StartsWith("SaveXYZ"))
            {
                // "SaveXYZ" 뒤에 있는 숫자 증가시키기
                int number = int.Parse(parts[1]);
                number++;

                // 증가된 숫자로 수정
                parts[1] = number.ToString();

                // 수정된 라인으로 업데이트
                lines[columnIndex] = string.Join(" ", parts);
            }

            // 새로운 값을 파일의 마지막에 추가하기
            string newLine = string.Join(" ", newValue);
            lines.Add(newLine);

            // 파일에 다시 저장하기
            File.WriteAllLines(filePath, lines);
        }

        public string[] GetAllValues()
        {
            var lines = File.ReadAllLines(filePath);
            string[] values = new string[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(' ');
                values[i] = parts[1].ToString(); // 두 번째 값을 문자열로 변환하여 저장
            }

            return values;
        }
        public void DeleteXYZ(int index)
        {
            var lines = File.ReadAllLines(filePath).ToList();

            if (lines.Count <= ColumnIndex.SAVEXYZ) return; // 파일 형식 이상 체크

            var headerParts = lines[ColumnIndex.SAVEXYZ].Split(' '); // "SaveXYZ XX"
            if (headerParts.Length < 2 || !int.TryParse(headerParts[1], out int count)) return; // 숫자 변환 체크

            if (index < 0 || index >= count) return; // 삭제할 인덱스가 범위 초과 시 종료

            // XYZ 데이터 삭제
            lines.RemoveAt(ColumnIndex.SAVEXYZ + index + 1); // 인덱스+1 번째 줄 삭제 (첫 줄은 헤더이므로 유지)

            // SaveXYZ 개수 업데이트
            lines[ColumnIndex.SAVEXYZ] = $"SaveXYZ {count - 1}";

            // 파일 저장
            File.WriteAllLines(filePath, lines);
        }

    }
    public static class ColumnIndex
    {
        public const int SPEED = 0;
        public const int ACTIME = 1;
        public const int DCTIME = 2;
        public const int AREA_LOW = 3;
        public const int AREA_HIGH = 4;
        public const int ROUND_LOW = 5;
        public const int ROUND_HIGH = 6;
        public const int OFFSET = 7;
        public const int SIZE = 8;
        public const int EMPTYCOUNT = 9;
        public const int EXPOSURETIME = 10;
        public const int PATH = 11;
        public const int DISPENSING_X = 12;
        public const int DISPENSING_Y = 13;
        public const int DISPENSING_Z = 14;
        public const int DISPENSING_CAM_X = 15;
        public const int DISPENSING_CAM_Y = 16;
        public const int DISPENSING_CAM_Z = 17;
        public const int SAVEXYZ = 18;
    }

}
