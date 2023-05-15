using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace Study.BigFiles
{
    public class BigFileHttpHost : IDisposable
    {
        public const String HOST_CONFIG_SECTION = "bigfile.hosts";
        const String URI_PREFIX_FORMAT = "http://{0}:{1}/";
        const String API_NAME = "BigFileApi";
        const String TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        const string WWWROOT = "wwwroot";

        public Int32 Port { get; private set; }
        public String FilePath { get; private set; }
        public Int64 FileSize { get; private set; }
        public String User { get; private set; }
        public String Passwd { get; private set; }

        public Boolean IsDisposed { get; private set; }
        protected HttpListener Listerner { get; private set; }
        protected Thread ListernerThread { get; private set; }

        public BigFileHttpHost(Int32 port, String filePath, Int64 fileSize, String user, String passwd)
        {
            Port = port;
            FilePath = filePath;
            FileSize = fileSize;
            User = user;
            Passwd = passwd;
        }

        private HttpListener CreateListener()
        {
            HttpListener listerner = new HttpListener();
            listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listerner.Prefixes.Add(GetURI("*"));

            return listerner;
        }

        private String GetURI(String ip)
        {
            return String.Format(URI_PREFIX_FORMAT, ip, this.Port);
        }

        public void Run(HttpListener listerner)
        {
            while (listerner.IsListening)
            {
                Trace.Write("waiting request...");
                HttpListenerContext ctx = listerner.GetContext();

                ThreadPool.QueueUserWorkItem(obj =>
                {
                    HttpListenerContext httpCtx = obj as HttpListenerContext;

                    try
                    {
                        DoHandler(httpCtx);
                    }
                    catch (Exception ex)
                    {
                        String exMsg = String.Format("Request Url:{0}, Http Method:{1}, File Path:{2}, Exception:{3}",
                            ctx.Request.Url, ctx.Request.HttpMethod, this.FilePath, ex.ToString());

                        Trace.WriteLine(exMsg);

                        try
                        {
                            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
                            {
                                writer.WriteLine(exMsg);

                                httpCtx.Response.StatusCode = (Int32)HttpStatusCode.OK;
                                httpCtx.Response.ContentType = "text/plain";
                                httpCtx.Response.ContentEncoding = Encoding.UTF8;
                                writer.Close();
                            }
                        }
                        catch (Exception ctxEx)
                        {
                            Console.WriteLine(ctxEx);
                        }
                    }
                }, ctx);
            }
        }

        public void Start()
        {
            if (Listerner != null && Listerner.IsListening)
            {
                return;
            }

            Listerner = CreateListener();

            ListernerThread = new Thread(obj =>
            {
                HttpListener http = (HttpListener)obj;
                Run(http);
            });
            ListernerThread.Name = "HttpServer:" + this.Port;
            ListernerThread.IsBackground = true;

            Listerner.Start();
            ListernerThread.Start(this.Listerner);
            IsDisposed = false;
            Trace.WriteLine(ListernerThread.Name + " started.");
        }

        public void Stop()
        {
            if (Listerner == null || ListernerThread == null || IsDisposed)
            {
                return;
            }

            ListernerThread.Abort();
            Listerner.Stop();

            Trace.WriteLine(ListernerThread.Name + " stopped.");
        }

        protected void DoHandler(HttpListenerContext ctx)
        {
            String url = (ctx.Request.RawUrl ?? String.Empty).Trim('/');
            Trace.Write(String.Format("Client:{0} {1} Request:{2}", ctx.Request.RemoteEndPoint.Address, ctx.Request.HttpMethod, ctx.Request.Url));

            if (String.Equals(url, API_NAME, StringComparison.OrdinalIgnoreCase) && ctx.Request.HttpMethod == "POST")
            {
                UploadFile(ctx);
                return;
            }

            if (url.StartsWith(API_NAME, StringComparison.OrdinalIgnoreCase) && ctx.Request.HttpMethod == "GET")
            {
                Stopwatch watch = Stopwatch.StartNew();
                Int32 fileLength = DownloadFile(ctx);
                watch.Stop();
                Trace.WriteLine(string.Format("Url:{0}, 获取文件({1})耗时:{2}s", url, fileLength, Math.Round(watch.Elapsed.TotalSeconds, 2)));

                return;
            }
            String htmlFile = Path.Combine("wwwroot", url);
            if (url.Equals("upload.html", StringComparison.OrdinalIgnoreCase) && File.Exists(htmlFile))
            {
                String html = File.ReadAllText(htmlFile, Encoding.UTF8);

                html = html.Replace("localhost", ctx.Request.LocalEndPoint.Address.ToString() + ":" + ctx.Request.LocalEndPoint.Port);
                
                ctx.Response.StatusCode = (Int32)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/html";
                ctx.Response.ContentEncoding = Encoding.UTF8;

                using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
                {
                    writer.WriteLine(html);
                    writer.Close();
                }

                return;
            }

            if (File.Exists(htmlFile))
            {
                using (BinaryWriter writer = new BinaryWriter(ctx.Response.OutputStream))
                {
                    Byte[] buffer = File.ReadAllBytes(htmlFile);
                    writer.Write(buffer, 0, buffer.Length);

                    ctx.Response.StatusCode = (Int32)HttpStatusCode.OK;
                    ctx.Response.Close();
                    writer.Close();
                }

                return;
            }

            StartInfo(ctx);
        }

        private void UploadFile(HttpListenerContext ctx)
        {
            String fileId = String.Empty;
            Stopwatch watch = Stopwatch.StartNew();
            Byte[] buffer = GetFile(ctx);
            Int32 fileLength = buffer.Length;
            using (BigFile bigFile = new BigFile(this.FilePath, this.FileSize, this.User, this.Passwd))
            {
                fileId = bigFile.Write(buffer).ToString();
            }
            buffer = null;
            String fileUrl = GetApiUrl(ctx.Request.LocalEndPoint.Address.ToString(), ctx.Request.LocalEndPoint.Port) + "/" + fileId;
            watch.Stop();
            Trace.WriteLine(string.Format("Url:{0}, 保存文件({1})耗时:{2}s", fileUrl, fileLength, Math.Round(watch.Elapsed.TotalSeconds, 2)));

            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream))
            {
                writer.Write(fileUrl);

                ctx.Response.AppendHeader("File-Id", fileId);
                ctx.Response.StatusCode = (Int32)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/plain";
                writer.Close();
            }
        }

        private String GetBoundary(String ctype)
        {
            if (ctype == null)
            {
                return String.Empty;
            }

            return "--" + ctype.Split(';')[1].Split('=')[1];
        }

        private Byte[] GetFile(HttpListenerContext ctx)
        {
            String contentType = ctx.Request.ContentType;

            if (contentType == null)
            {
                Int64 bufferSize = ctx.Request.ContentLength64;
                Int64 total = 0L;
                Byte[] result = new Byte[bufferSize];

                BinaryReader reader = new BinaryReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);

                while (true)
                {
                    Int32 dataleft = result.Length - (Int32)total;
                    Int32 offset = (Int32)total;

                    Int32 cnt = reader.Read(result, offset, dataleft);
                    if (cnt <= 0)
                    {
                        break;
                    }
                    total += cnt;
                    if (bufferSize <= total)
                    {
                        break;
                    }
                }

                return result;
            }

            return GetFile(ctx.Request.ContentEncoding, GetBoundary(contentType), ctx.Request.InputStream);
        }

        private Byte[] GetFile(Encoding enc, String boundary, Stream input)
        {
            if (input == Stream.Null)
            {
                return new Byte[0];
            }

            Byte[] boundaryBytes = enc.GetBytes(boundary);
            Int32 boundaryLen = boundaryBytes.Length;

            using (MemoryStream output = new MemoryStream())
            {
                Byte[] buffer = new Byte[1024];
                Int32 len = input.Read(buffer, 0, 1024);
                Int32 startPos = -1;

                // Find start boundary
                while (true)
                {
                    if (len == 0)
                    {
                        throw new Exception("Start Boundaray Not Found");
                    }

                    startPos = IndexOf(buffer, len, boundaryBytes);
                    if (startPos >= 0)
                    {
                        break;
                    }
                    else
                    {
                        Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                        len = input.Read(buffer, boundaryLen, 1024 - boundaryLen);
                    }
                }

                // Skip four lines (Boundary, Content-Disposition, Content-Type, and a blank)
                for (Int32 i = 0; i < 4; i++)
                {
                    while (true)
                    {
                        if (len == 0)
                        {
                            throw new Exception("Preamble not Found.");
                        }

                        startPos = Array.IndexOf(buffer, enc.GetBytes("\n")[0], startPos);
                        if (startPos >= 0)
                        {
                            startPos++;
                            break;
                        }
                        else
                        {
                            len = input.Read(buffer, 0, 1024);
                        }
                    }
                }

                Array.Copy(buffer, startPos, buffer, 0, len - startPos);
                len = len - startPos;

                while (true)
                {
                    Int32 endPos = IndexOf(buffer, len, boundaryBytes);
                    if (endPos >= 0)
                    {
                        if (endPos > 0) output.Write(buffer, 0, endPos - 2);
                        break;
                    }
                    else if (len <= boundaryLen)
                    {
                        throw new Exception("End Boundaray Not Found");
                    }
                    else
                    {
                        output.Write(buffer, 0, len - boundaryLen);
                        Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                        len = input.Read(buffer, boundaryLen, 1024 - boundaryLen) + boundaryLen;
                    }
                }

                return output.ToArray();
            }
        }

        private Int32 IndexOf(Byte[] buffer, Int32 len, Byte[] boundaryBytes)
        {
            for (Int32 i = 0; i <= len - boundaryBytes.Length; i++)
            {
                Boolean match = true;
                for (Int32 j = 0; j < boundaryBytes.Length && match; j++)
                {
                    match = buffer[i + j] == boundaryBytes[j];
                }

                if (match)
                {
                    return i;
                }
            }

            return -1;
        }

        private Int32 DownloadFile(HttpListenerContext ctx)
        {
            Int32 length = 0;
            Byte[] emptyBuffer = new Byte[0];
            String fileIdUrl = ctx.Request.RawUrl.Replace(API_NAME, String.Empty).Trim('/');
            Int64 fileId = 0L;
            Int64.TryParse(fileIdUrl, out fileId);

            using (BinaryWriter writer = new BinaryWriter(ctx.Response.OutputStream))
            {
                if (fileId == 0)
                {
                    writer.Write(emptyBuffer, 0, 0);
                }
                else
                {
                    using (BigFile bigFile = new BigFile(this.FilePath, this.FileSize, this.User, this.Passwd))
                    {
                        Byte[] buffer = bigFile.Read(fileId) ?? emptyBuffer;
                        length = buffer.Length;
                        writer.Write(buffer, 0, buffer.Length);
                    }
                }

                ctx.Response.StatusCode = (Int32)HttpStatusCode.OK;
                ctx.Response.Close();
                writer.Close();
            }

            return length;
        }

        private String GetApiUrl(String ip, Int32 port)
        {
            String url = String.Format(URI_PREFIX_FORMAT, ip, port) + API_NAME;
            return url;
        }

        private void StartInfo(HttpListenerContext ctx)
        {
            const Int32 MB_SIZE = 1048576;
            Process process = Process.GetCurrentProcess();

            PerformanceCounter pcMemory = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
            PerformanceCounter pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            pcCpuLoad.NextValue();

            CounterSample cs1 = pcCpuLoad.NextSample();
            Thread.Sleep(200);
            CounterSample cs2 = pcCpuLoad.NextSample();
            Single finalCpuCounter = CounterSample.Calculate(cs1, cs2);

            HostConfig section = ConfigurationManager.GetSection(HOST_CONFIG_SECTION) as HostConfig;
            ctx.Response.StatusCode = (Int32)HttpStatusCode.OK;
            ctx.Response.ContentType = "text/html;charset=utf-8";
            ctx.Response.ContentEncoding = Encoding.UTF8;

            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
            {
                writer.WriteLine(@"<!DOCTYPE html>
<html>
<head>
    <title>Web文件传输服务</title>
</head>
<body>");
                WriteHtmlpTag(writer, "Web文件传输服务已经启动!");
                WriteHtmlpTag(writer, "启动时间:" + process.StartTime.ToString(TIME_FORMAT));
                WriteHtmlpTag(writer, "系统时间:" + DateTime.Now.ToString(TIME_FORMAT));
                WriteHtmlpTag(writer, "请求地址:" + ctx.Request.RawUrl);
                WriteHtmlpTag(writer, "客户端IP:" + ctx.Request.RemoteEndPoint.Address);
                WriteHtmlpTag(writer, "线程数量:" + process.Threads.Count);
                WriteHtmlpTag(writer, "程序内存:" + Math.Round(pcMemory.NextValue() / MB_SIZE, 2) + "MB");
                WriteHtmlpTag(writer, "系统CPU:" + Math.Round(finalCpuCounter, 0) + "%");
                WriteHtmlpTag(writer, "OS版本:" + Environment.OSVersion.VersionString);

                if (section != null && section.Hosts != null)
                {
                    writer.WriteLine();
                    foreach (HostElement setting in section.Hosts)
                    {
                        WriteHtmlpTag(writer, "API:" + GetApiUrl(ctx.Request.LocalEndPoint.Address.ToString(), setting.Port));
                        String uploadUrl = String.Format(URI_PREFIX_FORMAT, ctx.Request.LocalEndPoint.Address.ToString(), setting.Port) + "upload.html";
                        WriteHtmlpTag(writer, "上传测试:<a href=\"" + uploadUrl + "\" target=\"_blank\">" + uploadUrl + "</a>");
                        WriteHtmlpTag(writer, "文件路径:" + setting.GetFilePath() + " 文件大小:" + setting.Size + " Http端口:" + setting.Port);

                        BigFile.Header header = null;
                        using (BigFile file = new BigFile(setting.GetFilePath(), setting.FileSize, setting.User, setting.Passwd))
                        {
                            header = file.GetHeader();
                        }

                        WriteHtmlpTag(writer, "剩余容量:" + Math.Round((Decimal)header.FreeStorage / MB_SIZE, 2) + "MB");
                        WriteHtmlpTag(writer, "文件个数:" + header.FileCount);
                        WriteHtmlpTag(writer, "覆盖前文件个数:" + header.CycleTotalFileCount + " 覆盖次数:" + header.OverwriteCount + " 覆盖时间:" + header.OverwriteTime.ToString(TIME_FORMAT));
                        WriteHtmlpTag(writer, "上一文件刻度:" + header.PrevOffset + " 上一文件存储时间:" + header.ActiveTime.ToString(TIME_FORMAT));

                        if (header.FinalOffset > 0)
                        {
                            WriteHtmlpTag(writer, "末尾文件刻度:" + header.FinalOffset + " 末尾文件存储时间:" + header.FinalFileTime.ToString(TIME_FORMAT));
                        }

                        WriteHtmlpTag(writer, "当前文件刻度:" + header.CurrentOffset);
                        writer.WriteLine();
                    }
                }
                writer.WriteLine(@"</body></html>");
                writer.Close();
            }

        }

        private void WriteHtmlTag(StreamWriter writer, String tag, String html)
        {
            writer.WriteLine("<" + tag + ">" + html + "</" + tag + ">");
        }

        private void WriteHtmlpTag(StreamWriter writer, String html)
        {
            writer.WriteLine("<p>" + html + "</p>");
        }

        ~BigFileHttpHost()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (IsDisposed || !disposing)
            {
                return;
            }

            try
            {
                if (ListernerThread != null)
                {
                    ListernerThread.Abort();

                    Trace.WriteLine(ListernerThread.Name + " disposed.");
                }

                if (Listerner != null)
                {
                    Listerner.Stop();
                    Listerner.Close();
                }

                IsDisposed = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                IsDisposed = true;
            }
        }
    }
}
