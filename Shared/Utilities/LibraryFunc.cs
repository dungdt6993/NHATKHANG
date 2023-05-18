using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace D69soft.Shared.Utilities
{
    public class LibraryFunc
    {
        //Mã hóa MD5
        public static string GennerateToMD5(string str)
        {
            using MD5 md5Hash = MD5.Create();
            byte[] bHash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sbHash = new StringBuilder();
            foreach (byte b in bHash)
            {
                sbHash.Append(String.Format("{0:x2}", b));
            }
            return sbHash.ToString();
        }

        //Trả về một số ngẫu nhiên nằm giữa khoảng min và max:
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        //Hàm bỏ dấu tiếng việt
        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        //Hàm bỏ khoảng trắng
        public static string RepalceWhiteSpace(string input)
        {
            while (input.IndexOf("  ") >= 0)    //tim trong chuoi vi tri co 2 khoang trong tro len      
                input = input.Replace("  ", " ");   //sau do thay the bang 1 khoang trong
            return input;
        }

        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        //Hàm lấy tên file trong thư mục
        public static String[] GetFilesNameFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption).Select(Path.GetFileNameWithoutExtension));
            }
            return filesFound.ToArray();
        }

        //Hàm xóa file trong thư mục
        public static bool DelFileFrom(string _fileUrl)
        {
            // Delete a file by using File class static method...
            if (File.Exists(_fileUrl))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    File.SetAttributes(_fileUrl, FileAttributes.Normal);
                    File.Delete(_fileUrl);
                }
                catch (IOException)
                {
                    return false;
                }
            }
            return true;
        }

        //Hàm lấy tên thứ
        public static string GetDayName(DateTime _dt)
        {
            var dayName = "";

            switch (_dt.DayOfWeek.ToString())
            {
                case "Monday":
                    dayName = "Thứ 2";
                    break;
                case "Tuesday":
                    dayName = "Thứ 3";
                    break;
                case "Wednesday":
                    dayName = "Thứ 4";
                    break;
                case "Thursday":
                    dayName = "Thứ 5";
                    break;
                case "Friday":
                    dayName = "Thứ 6";
                    break;
                case "Saturday":
                    dayName = "Thứ 7";
                    break;
                case "Sunday":
                    dayName = "Chủ nhật";
                    break;
            }

            return dayName;
        }

        //Hàm format date time
        public static DateTime? FormatDateDDMMYYYY(string _newDate, DateTime? _oldDate)
        {
            DateTime? date;

            if(_newDate == String.Empty)
            {
                date = null;
            }
            else
            {
                var dateToParse = _newDate;
                DateTime parsedDate;

                if (DateTime.TryParseExact(dateToParse, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    date = parsedDate;
                }
                else
                {
                    date = _oldDate;
                }
            }
            return date;
        }

        public static DateTime? FormatTimeHHMM(string _newTime, DateTime? _oldTime)
        {
            DateTime? time;

            if (_newTime == String.Empty)
            {
                time = null;
            }
            else
            {
                var timeToParse = "01/01/1900 " + _newTime;
                DateTime parsedTime;

                if (DateTime.TryParseExact(timeToParse, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedTime))
                {
                    time = parsedTime;
                }
                else
                {
                    time = _oldTime;
                }
            }
            return time;
        }
    }
}