using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Loki.Server.Exceptions;

namespace Loki.Server.Helpers
{
    public class HttpHelper
    {
        /// <summary>
        /// Gets the header from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        /// <exception cref="HttpHeaderException">Http header message too large to fit in buffer (16KB)</exception>
        public string GetHeaderFromStream(Stream stream)
        {
            const int HEADER_LENGTH = 1024 * 16; // 16KB
            const string HEADER_END = "\r\n\r\n";

            byte[] buffer = new byte[HEADER_LENGTH];
            int offset = 0;
            int bytesRead = 0;

            do
            {
                if (offset >= HEADER_LENGTH)
                    throw new HttpHeaderException("Http header message too large to fit in buffer (16KB)");

                try
                {
                    bytesRead = stream.Read(buffer, offset, HEADER_LENGTH - offset);
                }
                catch (IOException)
                {
                    return null;
                }

                offset += bytesRead;
                
                string header = Encoding.UTF8.GetString(buffer, 0, offset);

                if (header.Contains(HEADER_END))
                    return header;

            } while (bytesRead > 0);

            return null;
        }
        
        public string ParseRoute(string header)
        {
            string[] headers = GetHeadersFromStreamBlob(header);
            string[] routeParts = headers?[0].Split(' ');

            if (routeParts?.Length != 3)
                return null;

            return routeParts[1];
        }

        public NameValueCollection ParseQueryStrings(string route)
        {
            if (!route.Contains("?"))
                return null;

            int separatorIndex = route.IndexOf("?", StringComparison.Ordinal);
            if (separatorIndex == -1)
                return null;

            route = route.Remove(0, separatorIndex + 1);
            
            NameValueCollection queryStrings = new NameValueCollection();
            
            int endingIndex = route.IndexOf("&", 0, StringComparison.Ordinal);
            string[] queryStringParts = endingIndex > -1 ? route.Split('&') : new[] { route };

            for (int i = 0; i < queryStringParts.Length; ++i)
            {
                string[] queryString = queryStringParts[i].Split('=');
                if (queryString.Length != 2)
                    continue;

                queryStrings.Add(queryString[0], queryString[1]);
            }

            return queryStrings;
        }

        public static void SetHeader(string response, Stream stream)
        {
            response = response.Trim() + Environment.NewLine + Environment.NewLine;

            byte[] bytes = Encoding.UTF8.GetBytes(response);

            try
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (SocketException)
            {
            }
            catch (IOException)
            {
            }
        }

        private string[] GetHeadersFromStreamBlob(string header)
        {
            if (string.IsNullOrEmpty(header))
                return null;

            string[] headerLines = header.Split('\r');
            if (headerLines.Length <= 0 || headerLines[0] == null)
                return null;

            return headerLines;
        }
    }
}
