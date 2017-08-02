using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Loki.Interfaces.Data;
using Loki.Server.Exceptions;

namespace Loki.Server.Data
{
    public class HttpMetadata : IHttpMetadata
    {
        public string Route { get; private set; }
        public NameValueCollection QueryStrings { get; private set; }
        public Dictionary<string,string> Headers { get; } = new Dictionary<string, string>();

        public HttpMetadata(Stream stream)
        {
            Parse(stream);
        }

        private void Parse(Stream stream)
        {
            string headerBlob = GetHeaderFromStream(stream);
            string[] headersWithRoute = GetHeadersFromStreamBlob(headerBlob);

            if (headersWithRoute == null)
                return;

            ParseAndSet(headersWithRoute);
        }

        private string GetHeaderFromStream(Stream incomingStream)
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
                    bytesRead = incomingStream.Read(buffer, offset, HEADER_LENGTH - offset);
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

        private string[] GetHeadersFromStreamBlob(string header)
        {
            if (string.IsNullOrEmpty(header))
                return null;

            string[] headerLines = header.Split('\r');

            if (headerLines.Length <= 0 || headerLines[0] == null)
                return null;

            List<string> headers = new List<string>();
            for (int i = 0; i < headerLines.Length; ++i)
            {
                if (headerLines[i] == "\n")
                    continue;

                headerLines[i] = headerLines[i].Replace("\n", "");
                headers.Add(headerLines[i]);
            }

            return headers.ToArray();
        }

        private void ParseAndSet(string[] headers)
        {
            ParseAndSetHeaders(headers);

            string[] routeParts = headers?[0].Split(' ');

            if (routeParts?.Length != 3)
                return;

            ParseAndSetRoute(routeParts[1]);
            ParseAndSetQueryStrings(routeParts[1]);
        }

        private void ParseAndSetHeaders(string[] headers)
        {
            if (headers == null || headers.Length == 0)
                return;
            
            foreach (string header in headers)
            {
                if (string.IsNullOrEmpty(header))
                    continue;

                if (header.Contains("HTTP"))
                    continue;

                int idx = header.IndexOf(":", StringComparison.Ordinal);
                if (idx == -1)
                    continue;

                string key = header.Substring(0, idx);
                string value = header.Substring(idx + 2, header.Length - idx - 2); //The +2 accounts for the separator and the space after it

                Headers.Add(key, value);
            }
        }

        private void ParseAndSetRoute(string route)
        {
            int queryStringStartIndex = route.IndexOf("?", StringComparison.Ordinal);

            Route = queryStringStartIndex == -1 ? route : route.Substring(0, queryStringStartIndex);
        }

        private void ParseAndSetQueryStrings(string route)
        {
            if (!route.Contains("?"))
                return;

            int separatorIndex = route.IndexOf("?", StringComparison.Ordinal);
            if (separatorIndex == -1)
                return;

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

            QueryStrings = queryStrings;
        }
    }
}
