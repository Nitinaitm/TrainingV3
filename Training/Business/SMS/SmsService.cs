using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net;

namespace Training.Business.SMS
{
    public static class SmsService
    {
        private static string baseUrl = "https://api.pinnacle.in/index.php/sms/urlsms";
        private static string username = "dbabsphcl";
        private static string password = "Dba$7803";
        private static string sender = "BSPHCE";

        public static bool SendSms(string mobileNumber, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mobileNumber) || string.IsNullOrWhiteSpace(message))
                    return false;

                string encodedMessage = HttpUtility.UrlEncode(message);

                string url = baseUrl +
                             "?sender=" + HttpUtility.UrlEncode(sender) +
                             "&numbers=" + HttpUtility.UrlEncode(mobileNumber.Trim()) +
                             "&messagetype=TXT" +
                             "&message=" + encodedMessage +
                             "&response=Y" +
                             "&username=" + HttpUtility.UrlEncode(username) +
                             "&pass=" + HttpUtility.UrlEncode(password);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.Timeout = 15000;

                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    return ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300);
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateOtp()
        {
            byte[] bytes = new byte[4];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            uint value = BitConverter.ToUInt32(bytes, 0);
            return ((value % 9000) + 1000).ToString();
        }

        public static string GetLoginOtpMessage(string otp)
        {
            return "BSPHCL Training Portal Login OTP: " + otp + ". Do not share this OTP with anyone. - BSPHCL";
        }

        public static string GetTrainingStartedMessage(string trainingID, string trainingName)
        {
            return "Training " + trainingID + " has started. Please attend the training as per the scheduled programme. - BSPHCL";
        }

        public static string GetManagerAssignedMessage(string managerID, string trainingLocation)
        {
            return "You have been assigned as Training Manager" + (string.IsNullOrWhiteSpace(trainingLocation) ? "" : " for " + trainingLocation) + ". Manager ID: " + managerID + ". - BSPHCL";
        }

        public static string GetAttendanceCompletedMessage(string trainingID, string trainingName, string sessionName)
        {
            return "Attendance for " + trainingID + ", Session " + sessionName + " has been completed. - BSPHCL Training Portal";
        }

        public static string GetPreTestPublishedMessage(string trainingID, string trainingName, string sessionName)
        {
            return "Pre-Training Test for " + trainingID + ", Session " + sessionName + " is now available. Please login to the BSPHCL Training Portal and complete it. - BSPHCL";
        }

        public static string GetPostTestPublishedMessage(string trainingID, string trainingName, string sessionName)
        {
            return "Post-Training Test for " + trainingID + ", Session " + sessionName + " is now available. Please login to the BSPHCL Training Portal and complete it. - BSPHCL";
        }

        public static string GetFeedbackSubmittedMessage(string trainingID, string trainingName)
        {
            return "Trainee feedback has been submitted for Training " + trainingID + ". Please review it on the BSPHCL Training Portal. - BSPHCL";
        }
    }
}