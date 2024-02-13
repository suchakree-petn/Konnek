using Tutor.Utility;

namespace Konnek.Util
{
    public class KonnekUtil : Utility
    {
        public static string[] GetStringArrayFromByteArray(byte[] data, char delimiter)
        {
            return System.Text.Encoding.UTF8.GetString(data).Split(delimiter);
        }
    }
}

