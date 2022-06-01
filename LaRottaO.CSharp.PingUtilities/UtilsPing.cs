using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LaRottaO.CSharp.PingUtilities
{
    public class UtilsPing
    {
        /// <summary>
        ///
        /// A method to ping an  IP, or IP:PORT
        ///
        /// 2021 06 14
        ///
        /// by Luis Felipe La Rotta, reusing code from S.O.
        ///
        /// </summary>

        protected UtilsPing()
        {
            //Not implemented
        }

        public Task<Boolean> ping(string argAddress, int argTimeout = 2000, Boolean argShowDebug = true)
        {
            return Task.Run(() =>
            {
                Boolean pingSuccess;
                String extractedIpAndPort = "";

                /*******************************************************************/
                // Detects if the address contains IP:Port, or just IP
                // and executes the appropiate method.
                /*******************************************************************/

                if (argAddress.Contains(":"))
                {
                    int startPos = argAddress.IndexOf("://") + 3;

                    int endPos = argAddress.IndexOf("/", startPos);

                    if (endPos == -1)
                    {
                        endPos = argAddress.Length;
                    }

                    extractedIpAndPort = argAddress.Substring(startPos, (endPos - startPos));

                    pingSuccess = pingTcpIp(extractedIpAndPort, argTimeout);
                }
                else
                {
                    extractedIpAndPort = argAddress;
                    pingSuccess = classicPing(argAddress, argTimeout);
                }

                if (argShowDebug)
                {
                    Console.WriteLine("Ping to " + extractedIpAndPort + "..." + pingSuccess);
                }

                return pingSuccess;
            });
        }

        private Boolean classicPing(string argAddress, int argTimeOutMs)
        {
            Ping networkInformationPing = new Ping();

            try
            {
                PingReply reply = networkInformationPing.Send(argAddress, argTimeOutMs);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (networkInformationPing != null)
                {
                    networkInformationPing.Dispose();
                }
            }
        }

        private Boolean pingTcpIp(string argAddress, int argTimeOutMs)
        {
            TcpClient tcpIpClient = new TcpClient();

            try
            {
                String[] uriArray = argAddress.Split(':');

                return tcpIpClient.ConnectAsync(uriArray[0], Convert.ToInt32(uriArray[1])).Wait(argTimeOutMs);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (tcpIpClient != null)
                {
                    tcpIpClient.Close();
                }
            }
        }
    }
}