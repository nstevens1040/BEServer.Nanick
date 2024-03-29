﻿namespace SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data.SQLite;
    using System.Net;
    using Newtonsoft.Json;
    using System.IO;
    using System.Text.RegularExpressions;
    public class Context__
    {
        public string[] AcceptTypes { get; set; }
        public string ContentEncoding { get; set; }
        public long ContentLength64 { get; set; }
        public string ContentType { get; set; }
        public System.Net.CookieCollection Cookies { get; set; }
        public bool HasEntityBody { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string HttpMethod { get; set; }
        public string InputStream { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsLocal { get; set; }
        public bool IsSecureConnection { get; set; }
        public bool IsWebSocketRequest { get; set; }
        public bool KeepAlive { get; set; }
        public string LocalEndPoint { get; set; }
        public System.Version ProtocolVersion { get; set; }
        public Dictionary<string,string> QueryString { get; set; }
        public string RawUrl { get; set; }
        public string RemoteEndPoint { get; set; }
        public System.Guid RequestTraceIdentifier { get; set; }
        public string ServiceName { get; set; }
        public string TransportContext { get; set; }
        public Uri Url { get; set; }
        public Uri UrlReferrer { get; set; }
        public string UserAgent { get; set; }
        public string UserHostAddress { get; set; }
        public string UserHostName { get; set; }
        public string[] UserLanguages { get; set; }
        public Context__()
        {
        }
        public Dictionary<string, string> ConvertHeaders(System.Collections.Specialized.NameValueCollection nvc)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if(nvc.Keys.Count > 0)
            {
                foreach (string key in nvc.Keys)
                {
                    dict.Add(key, nvc[key]);
                }
            }
            return dict;
        }
        public static string RemoveDoubleDblQuote(string value)
        {
            return (new Regex("\"User(.*)Agent\": (\")(\")(.*)(\")(\")").Replace(value, "\"User$1Agent\": \"$4\""));
        }
        public string EscapeDblQuot(string body)
        {
            string input = String.Empty;
            Regex dblquot = new Regex("\"");
            if (!String.IsNullOrEmpty(body))
            {
                input = dblquot.Replace(body, "\\\"");
            }
            return input;
        }
        public Context__(HttpListenerRequest request,string streambody=null)
        {
            this.AcceptTypes = request.AcceptTypes;
            this.ContentEncoding = request.ContentEncoding.EncodingName;
            this.ContentLength64 = request.ContentLength64;
            this.ContentType = request.ContentType;
            this.Cookies = request.Cookies;
            this.HasEntityBody = request.HasEntityBody;
            this.Headers = ConvertHeaders(request.Headers);
            this.HttpMethod = request.HttpMethod;
            this.InputStream = EscapeDblQuot(streambody);
            this.IsAuthenticated = request.IsAuthenticated;
            this.IsLocal = request.IsLocal;
            this.IsSecureConnection = request.IsSecureConnection;
            this.IsWebSocketRequest = request.IsWebSocketRequest;
            this.KeepAlive = request.KeepAlive;
            this.LocalEndPoint = request.LocalEndPoint.Address.ToString() + ':' + request.LocalEndPoint.Port;
            this.ProtocolVersion = request.ProtocolVersion;
            this.QueryString = ConvertHeaders(request.QueryString);
            this.RawUrl = Logger.apos.Replace(request.RawUrl, String.Empty);
            this.RemoteEndPoint = request.RemoteEndPoint.Address.ToString() + ':' + request.RemoteEndPoint.Port;
            this.RequestTraceIdentifier = request.RequestTraceIdentifier;
            this.ServiceName = request.ServiceName;
            this.TransportContext = request.TransportContext.ToString();
            this.Url = request.Url;
            this.UrlReferrer = request.UrlReferrer;
            this.UserAgent = request.UserAgent;
            this.UserHostAddress = request.UserHostAddress;
            this.UserHostName = request.UserHostName;
            this.UserLanguages = request.UserLanguages;
        }
    }
    public class Logger
    {
        public Logger()
        {
        }
        public static Regex apos = new Regex("'");
        public static Regex dblquot = new Regex("\"");
        public static string home_ = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOME")) ? Environment.GetEnvironmentVariable("USERPROFILE") : Environment.GetEnvironmentVariable("HOME");
        public SQLiteConnection sqliteConnection;
        public SQLiteCommand sqliteCommand;
        public string connection_string = @"Data Source=" + home_ + Path.DirectorySeparatorChar + @"WebServerLog.sqlite;PRAGMA journal_mode=WAL;";
        public Double Epoch()
        {
            return Math.Round((DateTime.UtcNow - DateTime.Parse("1970-01-01")).TotalSeconds);
        }
        public DateTime UnEpoch(Double timestamp)
        {
            return DateTime.Parse("1970-01-01").AddSeconds(timestamp).ToLocalTime();
        }
        public System.Data.DataTable ExportDataTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            this.sqliteCommand = new SQLiteCommand(this.sqliteConnection);
            this.sqliteCommand.CommandText = "SELECT * FROM WebServerLog";
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(this.sqliteCommand))
            {
                System.Data.DataSet dataset = new System.Data.DataSet();
                try
                {
                    adapter.Fill(dataset);
                    if (dataset.Tables.Count > 0)
                    {
                        dt = dataset.Tables[0];
                    }
                }
                catch
                {
                }
            }
            return dt;
        }
        public bool CheckForTable()
        {
            bool table_exists = false;
            this.sqliteCommand = new SQLiteCommand(this.sqliteConnection);
            this.sqliteCommand.CommandText = "SELECT * FROM WebServerLog";
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(this.sqliteCommand))
            {
                System.Data.DataSet dataset = new System.Data.DataSet();
                try
                {
                    adapter.Fill(dataset);
                    if (dataset.Tables.Count > 0)
                    {
                        table_exists = true;
                    }
                }
                catch
                {
                    table_exists = false;
                }
            }
            return table_exists;
        }
        public void SQLiteConnect()
        {
            this.sqliteConnection = new SQLiteConnection() { ConnectionString = this.connection_string };
            this.sqliteConnection.Open();
            if (!CheckForTable())
            {
                this.sqliteCommand = new SQLiteCommand(this.sqliteConnection);
                this.sqliteCommand.CommandText = "CREATE TABLE WebServerLog (\n    timestamp INTEGER,\n    request_json TEXT(4000),\n    source_ip_address TEXT(4000),\n    method TEXT(4000),\n    absolute_uri TEXT(4000),\n    content_length BIGINT(255),\n    referer TEXT(4000),\n    cookies INT,\n    is_auth BIT(1)\n);";
                this.sqliteCommand.ExecuteNonQuery();
            }
        }
        public void SQLiteInsert(HttpListenerContext context,string streambody=null)
        {
            string timestamp_string = Epoch().ToString();
            Context__ context_obj;
            if (String.IsNullOrEmpty(streambody))
            {
                context_obj = new Context__(context.Request);
            } else
            {
                context_obj = new Context__(context.Request,streambody);
            }
            string request_json = JsonConvert.SerializeObject(context_obj, Formatting.Indented).Replace((Char)39, (Char)34);
            string source_ip = context.Request.RemoteEndPoint.Address.ToString();
            string method = context.Request.HttpMethod;
            string req_uri = context.Request.Url.AbsoluteUri;
            string req_len = context.Request.ContentLength64.ToString();
            string referer = String.Empty;
            if (context.Request.UrlReferrer != null)
            {
                referer = context.Request.UrlReferrer.AbsoluteUri;
            }
            int is_auth = 0;
            if (context.Request.IsAuthenticated)
            {
                is_auth = 1;
            }
            string insert1 = "INSERT INTO WebServerLog(timestamp,request_json,source_ip_address,method,absolute_uri,content_length,referer,cookies,is_auth)";
            string insert2 = insert1 + "VALUES(" + timestamp_string + ", '" + request_json + "', '" + source_ip + "', '" + method + "', '" + req_uri + "', '" + req_len + "', '" + referer + "', '" + context.Request.Cookies.Count.ToString() + "', '" + is_auth.ToString() + "');";
            this.sqliteCommand.CommandText = insert2;
            this.sqliteCommand.ExecuteNonQuery();
        }
    }
}
