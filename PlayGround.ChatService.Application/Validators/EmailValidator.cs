using System.Net.Sockets;
using System.Text;

namespace PlayGround.ChatService.Application.Validators
{

    public static class EmailValidator
    {
        public static bool IsEmailValid(string email)
        {
            try
            {
                using (TcpClient client = new TcpClient("gmail-smtp-in.l.google.com", 25))
                using (NetworkStream netStream = client.GetStream())
                using (StreamReader reader = new StreamReader(netStream))
                {
                    string CRLF = "\r\n";
                    byte[] dataBuffer;
                    string responseString;

                    // HELO command
                    dataBuffer = Encoding.ASCII.GetBytes("HELO example.com" + CRLF);
                    netStream.Write(dataBuffer, 0, dataBuffer.Length);
                    responseString = reader.ReadLine();

                    // MAIL FROM command
                    dataBuffer = Encoding.ASCII.GetBytes("MAIL FROM:<your-email@gmail.com>" + CRLF);
                    netStream.Write(dataBuffer, 0, dataBuffer.Length);
                    responseString = reader.ReadLine();

                    // RCPT TO command
                    dataBuffer = Encoding.ASCII.GetBytes($"RCPT TO:<{email}>" + CRLF);
                    netStream.Write(dataBuffer, 0, dataBuffer.Length);
                    responseString = reader.ReadLine();

                    // Check response code
                    if (responseString.Contains("250"))
                    {
                        return true; // Email exists
                    }
                    else if (responseString.Contains("550"))
                    {
                        return false; // Email does not exist
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }
    }
}
