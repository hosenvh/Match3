using System;
using System.Net.Mail;

namespace Match3.Utility
{
    public static class Validator
    {
        public static bool IsEmailValid(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}