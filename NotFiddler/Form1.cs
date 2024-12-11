using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BCCertMaker;
using Fiddler;
using Newtonsoft.Json.Linq;

namespace NotFiddler;

public class Form1 : Form
{
	private string DefaultDomain;

	private string bhvrSession;

	private string LastBhvrSession;

	private string queueRequest;

	private string queueResponse;

	private Thread LookupThreadHandle;

	private bool TargetFound;

	private string TargetQueueData;

	private SoundPlayer SoundNotifier;

	private string MyKeywords;

	private string MyExpludeList;

	private bool StopRequested;

	private bool CloseRequested;

	private string QueueRegion;

	private int SearchForIdx;

	private bool DontWait;

	private bool SkipSameLobbies;

	private List<string> LastFoundMatches;

	private int QueueDelayed;

	private string WaitOthersForMatch;

	public List<string> LastPlayerIds;

	public List<string> PlayersCached;

	public List<string> Steam64Cached;

	public string CloudSteamCached;

	public string DesiredNicknameCached;

	public string AppliedNickname;

	public int SkipSameDelay;

	public bool SkipSameWaitOthers;

	public bool SkipSameWaitKillerOnly;

	public string SkipSameWaitExcludeList;

	public bool DontCheckOthers;

	public string MyCloudID;

	private string x_kraken_client_version;

	private string user_agent;

	private string CDN_GameConfigs;

	private const int MOUSEEVENTF_LEFTDOWN = 2;

	private const int MOUSEEVENTF_LEFTUP = 4;

	private IContainer components;

	private RichTextBox richTextBox1;

	private Button button1;

	private Button button2;

	private GroupBox groupBox1;

	private RichTextBox richTextBox2;

	private GroupBox groupBox3;

	private GroupBox groupBox4;

	private Label label1;

	private ComboBox comboBox1;

	private GroupBox groupBox2;

	private RichTextBox richTextBox3;

	private Label label2;

	private LinkLabel linkLabel1;

	private Label label3;

	private CheckBox checkBox1;

	private ToolTip toolTip1;

	private Button button3;

	private Button button4;

	private CheckBox checkBox3;

	private ComboBox comboBox2;

	private GroupBox groupBox5;

	private GroupBox groupBox6;

	private Button button5;

	public Form1()
	{
		InitializeComponent();
		button1.Enabled = true;
		button2.Enabled = false;
		label1.Text = "";
		comboBox1.Items.Add("ap-south-1");
		comboBox1.Items.Add("eu-west-1");
		comboBox1.Items.Add("ap-southeast-1");
		comboBox1.Items.Add("ap-southeast-2");
		comboBox1.Items.Add("eu-central-1");
		comboBox1.Items.Add("ap-northeast-2");
		comboBox1.Items.Add("ap-northeast-1");
		comboBox1.Items.Add("us-east-1");
		comboBox1.Items.Add("sa-east-1");
		comboBox1.Items.Add("us-west-2");
		comboBox1.SelectedIndex = 4;
		comboBox2.Items.Add("Both");
		comboBox2.Items.Add("Survivors Only");
		comboBox2.Items.Add("Killers Only");
		comboBox2.SelectedIndex = 0;
		if (File.Exists("keywords.txt"))
		{
			richTextBox2.Text = File.ReadAllText("keywords.txt");
		}
		if (File.Exists("excludeList.txt"))
		{
			richTextBox3.Text = File.ReadAllText("excludeList.txt");
		}
		toolTip1.SetToolTip(checkBox1, "The tool won't wait 5 players in a found match");
		toolTip1.SetToolTip(checkBox3, "Skip same lobbies to increase queue time (do not use for survivors queue)");
		DefaultDomain = "";
		bhvrSession = "";
		LastBhvrSession = "";
		queueRequest = "";
		queueResponse = "";
		LookupThreadHandle = new Thread(LookupThread);
		LookupThreadHandle.Start();
		TargetFound = false;
		TargetQueueData = "";
		SoundNotifier = new SoundPlayer("notify.wav");
		MyKeywords = "";
		MyExpludeList = "";
		StopRequested = false;
		CloseRequested = false;
		QueueRegion = "";
		SearchForIdx = 0;
		DontWait = false;
		SkipSameLobbies = false;
		LastFoundMatches = new List<string>();
		QueueDelayed = 0;
		WaitOthersForMatch = "";
		LastPlayerIds = new List<string>();
		PlayersCached = new List<string>();
		Steam64Cached = new List<string>();
		CloudSteamCached = "";
		DesiredNicknameCached = "";
		AppliedNickname = "";
		SkipSameDelay = 4;
		SkipSameWaitOthers = false;
		SkipSameWaitKillerOnly = false;
		SkipSameWaitExcludeList = "";
		DontCheckOthers = false;
		MyCloudID = "";
		CDN_GameConfigs = "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";
		x_kraken_client_version = "8.4.1";
		user_agent = "DeadByDaylight/DBD_Gelato_HF1_WinGDK_Shipping_5_2184626 WinGDK/10.0.22621.0.0.64bit";
		global::BCCertMaker.BCCertMaker bCCertMaker = (global::BCCertMaker.BCCertMaker)(CertMaker.oCertProvider = new global::BCCertMaker.BCCertMaker());
		string text = "RootCertificate.p12";
		string password = "S0m3T0pS3cr3tP4ssw0rd";
		if (!File.Exists(text))
		{
			bCCertMaker.CreateRootCertificate();
			bCCertMaker.WriteRootCertificateAndPrivateKeyToPkcs12File(text, password);
		}
		else
		{
			bCCertMaker.ReadRootCertificateAndPrivateKeyFromPkcs12File(text, password);
		}
	}

	private void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		CloseRequested = true;
		FiddlerApplication.Shutdown();
	}

	public void OutputLog(string value)
	{
		if (base.InvokeRequired)
		{
			richTextBox1.Invoke(new Action<string>(OutputLog), value);
		}
		else
		{
			richTextBox1.AppendText(value + "\n");
			richTextBox1.ScrollToCaret();
		}
	}

	private void SetMMR(string value)
	{
		if (base.InvokeRequired)
		{
			label1.Invoke(new Action<string>(SetMMR), value);
		}
		else
		{
			label1.Text = value;
		}
	}

	private void SetStartButton(bool enabled)
	{
		if (base.InvokeRequired)
		{
			button1.Invoke(new Action<bool>(SetStartButton), enabled);
		}
		else
		{
			button1.Enabled = enabled;
		}
	}

	private void SetCloseButton(bool enabled)
	{
		if (base.InvokeRequired)
		{
			button2.Invoke(new Action<bool>(SetCloseButton), enabled);
		}
		else
		{
			button2.Enabled = enabled;
		}
	}

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
	public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

	private void LookupThread()
	{
		while (!CloseRequested)
		{
			Thread.Sleep(1000);
			if (StopRequested)
			{
				bhvrSession = "";
				queueRequest = "";
				queueResponse = "";
				TargetFound = false;
				SetStartButton(enabled: true);
				SetCloseButton(enabled: false);
				LastFoundMatches.Clear();
				QueueDelayed = 0;
				WaitOthersForMatch = "";
				if (FiddlerApplication.oProxy.Detach())
				{
					OutputLog("fiddler just stopped");
				}
				StopRequested = false;
				continue;
			}
			if (bhvrSession.Length == 0 || TargetFound)
			{
				continue;
			}
			if (SkipSameLobbies && (SkipSameWaitOthers || SkipSameWaitKillerOnly) && WaitOthersForMatch.Length > 0)
			{
				string text = DBD_GetMatchInfo(WaitOthersForMatch);
				try
				{
					JObject jObject = JObject.Parse(text);
					JArray jArray = (JArray)jObject["sideA"];
					int count = jArray.Count;
					JArray content = (JArray)jObject["sideB"];
					jArray.Merge(content);
					int num = (int)jObject["props"]["countA"] + (int)jObject["props"]["countB"];
					if ((SkipSameWaitOthers && jArray.Count < num) || (SkipSameWaitKillerOnly && count == 0))
					{
						OutputLog($"same match waiting {WaitOthersForMatch} players {jArray.Count}...");
						continue;
					}
					bool flag = false;
					string[] array = SkipSameWaitExcludeList.Split('\n');
					foreach (JToken item in jArray)
					{
						string text2 = (string?)item;
						string[] array2 = array;
						foreach (string text3 in array2)
						{
							if (text3.Trim().Length > 0 && text2.ToLower().Contains(text3))
							{
								flag = true;
								OutputLog($"same match waiting {WaitOthersForMatch} players {jArray.Count}, but contains a guy excluded {text3}");
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
					if (flag)
					{
						continue;
					}
					WaitOthersForMatch = "";
					goto IL_02ba;
				}
				catch (Exception ex)
				{
					OutputLog("something went wrong in looking thread (" + ex.Message + "): " + text);
					goto IL_02ba;
				}
			}
			if (QueueDelayed > 0)
			{
				OutputLog($"queue delayed {QueueDelayed}...");
				QueueDelayed--;
				continue;
			}
			goto IL_02ba;
			IL_02ba:
			string text4 = DBD_QueueRequest();
			try
			{
				JObject jObject2 = JObject.Parse(text4);
				if ((string?)jObject2["status"] == "MATCHED")
				{
					bool flag2 = false;
					JObject jObject3 = JObject.Parse(DBD_GetMatchInfo((string?)jObject2["matchData"]["matchId"]));
					JArray jArray2 = (JArray)jObject3["sideA"];
					JArray jArray3 = (JArray)jObject3["sideB"];
					JArray jArray4 = new JArray();
					jArray4.Merge(jArray2);
					jArray4.Merge(jArray3);
					int num2 = (int)jObject3["props"]["countA"] + (int)jObject3["props"]["countB"];
					string text5 = (string?)jObject3["matchId"];
					OutputLog($"match found {text5} players {jArray4.Count}");
					if (jArray4.Count < num2 && !DontWait)
					{
						continue;
					}
					if (jArray4.Count == num2 && SkipSameLobbies && LastFoundMatches.Contains(text5))
					{
						jArray4.Clear();
						if (SkipSameWaitOthers || SkipSameWaitKillerOnly)
						{
							WaitOthersForMatch = text5;
						}
						else
						{
							QueueDelayed = SkipSameDelay;
						}
						OutputLog($"SAME match found {text5}, skipping (WaitOthers={SkipSameWaitOthers}, WaitKiller={SkipSameWaitKillerOnly}, Delay={SkipSameDelay})");
					}
					foreach (JToken item2 in jArray4)
					{
						string text6 = (string?)item2;
						if (DontCheckOthers && ((SearchForIdx == 1 && jArray2.ToObject<List<string>>().Contains(text6)) || (SearchForIdx == 2 && jArray3.ToObject<List<string>>().Contains(text6))))
						{
							continue;
						}
						JObject jObject4 = JObject.Parse(DBD_GetNicknameByCloudID(text6));
						string text7 = (string?)jObject4["playerName"];
						string text8 = "";
						if (jObject4["providerPlayerNames"] != null && jObject4["providerPlayerNames"]["steam"] != null)
						{
							string text9 = DBD_GetSteamByCloudID(text6);
							if (text9.Contains("\"providerId\":"))
							{
								text8 = (string?)JObject.Parse(text9)["providerId"];
							}
						}
						if (SearchForIdx == 1 && jArray2.ToObject<List<string>>().Contains(text6))
						{
							OutputLog("[Killer-SKIP] " + text6 + " | " + text7 + " | " + text8);
							continue;
						}
						if (SearchForIdx == 2 && jArray3.ToObject<List<string>>().Contains(text6))
						{
							OutputLog("[Survivor-SKIP] " + text6 + " | " + text7 + " | " + text8);
							continue;
						}
						TargetFound = CompareWithKeywords(text6, text7, text8);
						if (TargetFound)
						{
							if (jObject2["matchData"]["customData"]["SessionSettings"] != null)
							{
								TargetQueueData = text4;
								SoundNotifier.Play();
								OutputLog("PRESS READY IN THE GAME TO JOIN");
								PlayersCached.Clear();
								Steam64Cached.Clear();
								LastPlayerIds.Clear();
								LastPlayerIds = jArray4.ToObject<List<string>>();
							}
							else
							{
								TargetFound = false;
								flag2 = true;
							}
							break;
						}
						OutputLog(text6 + " | " + text7 + " | " + text8);
					}
					if (!flag2 && !TargetFound && DBD_QueueCancel().Length > 0)
					{
						if (jArray4.Count == num2 && SkipSameLobbies && !LastFoundMatches.Contains(text5))
						{
							LastFoundMatches.Add(text5);
						}
						queueRequest = queueRequest.Replace("\"checkOnly\":true", "\"checkOnly\":false");
						if (QueueDelayed == 0 && WaitOthersForMatch.Length == 0)
						{
							OutputLog("starting new queue");
						}
					}
					continue;
				}
				if ((string?)jObject2["status"] == "NONE" || jObject2["queueData"] == null)
				{
					if (DBD_QueueCancel().Length > 0)
					{
						queueRequest = queueRequest.Replace("\"checkOnly\":true", "\"checkOnly\":false");
						OutputLog("unknown error has occured, starting new queue");
					}
				}
				else
				{
					queueRequest = queueRequest.Replace("\"checkOnly\":false", "\"checkOnly\":true");
					OutputLog(string.Format("queue position {0}", (int)jObject2["queueData"]["position"]));
				}
			}
			catch (Exception ex2)
			{
				OutputLog("something went wrong in looking thread (" + ex2.Message + "): " + text4);
			}
		}
	}

	private bool CompareWithKeywords(string cloudID, string playerName, string steamID)
	{
		string[] array = Interlocked.CompareExchange(ref MyKeywords, null, null).Split('\n');
		string[] array2 = Interlocked.CompareExchange(ref MyExpludeList, null, null).Split('\n');
		string[] array3 = array;
		for (int i = 0; i < array3.Length; i++)
		{
			string text = array3[i].Trim();
			if (text.Length == 0 || (!cloudID.ToLower().Contains(text.ToLower()) && !playerName.ToLower().Contains(text.ToLower()) && !steamID.ToLower().Contains(text.ToLower())))
			{
				continue;
			}
			bool flag = false;
			string[] array4 = array2;
			for (int j = 0; j < array4.Length; j++)
			{
				string text2 = array4[j].Trim();
				if (text2.Length != 0 && (cloudID.ToLower().Contains(text2.ToLower()) || playerName.ToLower().Contains(text2.ToLower()) || steamID.ToLower().Contains(text2.ToLower())))
				{
					flag = true;
					OutputLog("EXCLUDED: " + text2 + " | " + cloudID + " | " + playerName + " | " + steamID);
					break;
				}
			}
			if (!flag)
			{
				OutputLog("TARGET FOUND: " + text + " | " + cloudID + " | " + playerName + " | " + steamID);
				return true;
			}
		}
		return false;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		button1.Enabled = false;
		if (!FiddlerApplication.IsStarted())
		{
			if (!CertMaker.rootCertIsTrusted())
			{
				CertMaker.trustRootCert();
			}
			FiddlerApplication.BeforeRequest += Fiddler_BeforeRequest;
			FiddlerApplication.ResponseHeadersAvailable += Fiddler_ResponseHeadersAvailable;
			FiddlerApplication.BeforeResponse += Fiddler_BeforeResponse;
			FiddlerCoreStartupSettings startupSettings = new FiddlerCoreStartupSettingsBuilder().ListenOnPort(9999).RegisterAsSystemProxy().ChainToUpstreamGateway()
				.DecryptSSL()
				.OptimizeThreadPool()
				.Build();
			CONFIG.DecryptWhichProcesses = ProcessFilterCategories.NonBrowsers;
			FiddlerApplication.Startup(startupSettings);
		}
		else if (!FiddlerApplication.oProxy.Attach())
		{
			button2.Enabled = true;
			OutputLog("failed to start fiddler");
			return;
		}
		Interlocked.Exchange(ref MyKeywords, richTextBox2.Text);
		Interlocked.Exchange(ref MyExpludeList, richTextBox3.Text);
		QueueRegion = comboBox1.GetItemText(comboBox1.SelectedItem);
		SearchForIdx = comboBox2.SelectedIndex;
		DontWait = checkBox1.Checked;
		SkipSameLobbies = checkBox3.Checked;
		button2.Enabled = true;
		OutputLog("fiddler just started");
		OutputLog("Press Ready in the game");
	}

	private void Fiddler_BeforeRequest(Session oSession)
	{
		if (oSession.fullUrl.Contains("bhvrdbd.com") && DefaultDomain.Length == 0)
		{
			DefaultDomain = oSession.host;
		}
		if (oSession.fullUrl.EndsWith("/queue") && !button1.Enabled)
		{
			if (bhvrSession.Length == 0)
			{
				bhvrSession = oSession.oRequest["Cookie"].Replace("bhvrSession=", "");
				LastBhvrSession = bhvrSession;
				queueRequest = Encoding.UTF8.GetString(oSession.requestBodyBytes);
				queueRequest = Regex.Replace(queueRequest, "latency\":([0-9]+)", "latency\":1000");
				queueRequest = queueRequest.Replace("\"latency\":1000,\"regionName\":\"" + QueueRegion + "\"", "\"latency\":10,\"regionName\":\"" + QueueRegion + "\"");
				oSession.utilSetRequestBody(queueRequest);
				queueRequest = queueRequest.Replace("\"checkOnly\":false", "\"checkOnly\":true");
				if (MyCloudID.Length == 0)
				{
					try
					{
						string text = DBD_GetInventories();
						JObject jObject = JObject.Parse(text);
						MyCloudID = (string?)jObject["data"]["playerId"];
						if (MyCloudID.Length == 0)
						{
							OutputLog("ERROR: can't get my cloudid: " + text);
						}
					}
					catch (Exception ex)
					{
						OutputLog("something went wrong while tried to get my cloudid (" + ex.Message + ")");
					}
				}
				OutputLog("GOT IT! (region: " + QueueRegion + ")");
			}
			else
			{
				oSession.utilCreateResponseAndBypassServer();
				oSession.responseCode = 200;
			}
		}
		else if (bhvrSession.Length > 0 && oSession.fullUrl.EndsWith("/queue/cancel"))
		{
			oSession.utilCreateResponseAndBypassServer();
			oSession.responseCode = 200;
			OutputLog("queue cancel bypassed");
		}
	}

	private void Fiddler_ResponseHeadersAvailable(Session oSession)
	{
		if (oSession.fullUrl.EndsWith("/queue") || oSession.fullUrl.EndsWith("GameConfigs.json"))
		{
			oSession.bBufferResponse = true;
		}
	}

	private void Fiddler_BeforeResponse(Session oSession)
	{
		if (oSession.fullUrl.EndsWith("/queue") && !button1.Enabled)
		{
			if (TargetFound && TargetQueueData.Length > 0)
			{
				oSession.utilDecodeResponse();
				oSession.utilSetResponseBody(TargetQueueData);
				OutputLog("queue response replaced ");
				TargetQueueData = "";
				StopRequested = true;
			}
			else if (queueResponse.Length == 0)
			{
				queueResponse = Encoding.UTF8.GetString(oSession.responseBodyBytes);
			}
			else
			{
				oSession.utilDecodeResponse();
				oSession.utilSetResponseBody(queueResponse);
			}
		}
		else if (oSession.fullUrl.EndsWith("GameConfigs.json"))
		{
			oSession.utilDecodeResponse();
			oSession.utilSetResponseBody(CDN_GameConfigs);
			OutputLog("GameConfigs.json was replaced");
		}
		else if (oSession.fullUrl.EndsWith("/config"))
		{
			LastBhvrSession = oSession.oRequest["Cookie"].Replace("bhvrSession=", "");
		}
	}

	private void button2_Click(object sender, EventArgs e)
	{
		button2.Enabled = false;
		if (!FiddlerApplication.IsStarted())
		{
			OutputLog("fiddler is not running");
		}
		else
		{
			StopRequested = true;
		}
	}

	private string GetKrakenPlatformProviderFromHostname(string hostname)
	{
		if (hostname.Contains("steam"))
		{
			return "steam";
		}
		if (hostname.Contains("grdk"))
		{
			return "grdk";
		}
		if (hostname.Contains("egs"))
		{
			return "egs";
		}
		return "steam";
	}

	public string DBD_QueueRequest()
	{
		string result = "";
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/queue");
			obj.Proxy = new WebProxy();
			obj.Method = "POST";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-provider", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			byte[] bytes = Encoding.UTF8.GetBytes(queueRequest);
			obj.ContentLength = bytes.Length;
			obj.GetRequestStream().Write(bytes, 0, bytes.Length);
			WebResponse response = obj.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	public string DBD_QueueCancel()
	{
		string result = "";
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/queue/cancel");
			obj.Proxy = new WebProxy();
			obj.Method = "POST";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-provider", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			obj.ContentLength = 0L;
			WebResponse response = obj.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	public int DBD_CreateMatch(string category, string userid)
	{
		int num = 0;
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/match/create");
			httpWebRequest.Proxy = new WebProxy();
			httpWebRequest.Method = "POST";
			httpWebRequest.ProtocolVersion = HttpVersion.Version11;
			httpWebRequest.Host = DefaultDomain;
			httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			httpWebRequest.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			httpWebRequest.Headers.Add("x-kraken-client-platform", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			httpWebRequest.Headers.Add("x-kraken-client-provider", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			httpWebRequest.Headers.Add("x-kraken-client-resolution", "1920x1080");
			httpWebRequest.Headers.Add("x-kraken-client-timezone-offset", "-180");
			httpWebRequest.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			httpWebRequest.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			httpWebRequest.UserAgent = user_agent;
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			byte[] bytes = Encoding.UTF8.GetBytes("{\"category\":\"" + category + "\",\"latencies\":[{\"latency\":77,\"regionName\":\"eu-central-1\"}],\"playersA\":[],\"playersB\":[\"" + userid + "\"],\"region\":\"all\"}");
			httpWebRequest.ContentLength = bytes.Length;
			httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			WebResponse response = httpWebRequest.GetResponse();
			num = (int)((HttpWebResponse)response).StatusCode;
			response.Close();
		}
		catch (WebException ex)
		{
			num = (int)((ex.Response as HttpWebResponse)?.StatusCode).Value;
		}
		return num;
	}

	public int DBD_AddRecentlyPlayed(string ids)
	{
		int num = 0;
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/players/recentlyplayed/add");
			obj.Proxy = new WebProxy();
			obj.Method = "POST";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-provider", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			byte[] bytes = Encoding.UTF8.GetBytes("{\"ids\":[" + ids + "]}");
			obj.ContentLength = bytes.Length;
			obj.GetRequestStream().Write(bytes, 0, bytes.Length);
			WebResponse response = obj.GetResponse();
			num = (int)((HttpWebResponse)response).StatusCode;
			response.Close();
		}
		catch (WebException ex)
		{
			num = (int)((ex.Response as HttpWebResponse)?.StatusCode).Value;
		}
		return num;
	}

	public string DBD_GetInventories()
	{
		string result = "";
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/inventories");
			obj.Proxy = new WebProxy();
			obj.Method = "GET";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", "steam");
			obj.Headers.Add("x-kraken-client-provider", "steam");
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			obj.ContentLength = 0L;
			WebResponse response = obj.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	public string DBD_GetNicknameByCloudID(string cloudid)
	{
		string result = "";
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/playername/byId/" + cloudid);
			obj.Proxy = new WebProxy();
			obj.Method = "GET";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", "steam");
			obj.Headers.Add("x-kraken-client-provider", "steam");
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			obj.ContentLength = 0L;
			WebResponse response = obj.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	public string DBD_GetSteamByCloudID(string cloudid)
	{
		string result = "";
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/players/" + cloudid + "/provider/provider-id");
			obj.Proxy = new WebProxy();
			obj.Method = "GET";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", "steam");
			obj.Headers.Add("x-kraken-client-provider", "steam");
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			obj.ContentLength = 0L;
			WebResponse response = obj.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	public string DBD_ChangeNickname(string newName)
	{
		string result = "";
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/playername/" + Uri.EscapeDataString(newName));
			obj.Proxy = new WebProxy();
			obj.Method = "POST";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-provider", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			obj.ContentLength = 0L;
			WebResponse response = obj.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	public string DBD_AddRemoveFriend(string platformId, string platformName, bool add)
	{
		string result = "";
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/players/friends/" + (add ? "add" : "remove"));
			httpWebRequest.Proxy = new WebProxy();
			httpWebRequest.Method = "POST";
			httpWebRequest.ProtocolVersion = HttpVersion.Version11;
			httpWebRequest.Host = DefaultDomain;
			httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			httpWebRequest.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			httpWebRequest.Headers.Add("x-kraken-client-platform", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			httpWebRequest.Headers.Add("x-kraken-client-provider", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			httpWebRequest.Headers.Add("x-kraken-client-resolution", "1920x1080");
			httpWebRequest.Headers.Add("x-kraken-client-timezone-offset", "-180");
			httpWebRequest.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			httpWebRequest.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			httpWebRequest.UserAgent = user_agent;
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			byte[] bytes = Encoding.UTF8.GetBytes("{\"ids\":[\"" + platformId + "\"],\"platform\":\"" + platformName + "\"}");
			httpWebRequest.ContentLength = bytes.Length;
			httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			WebResponse response = httpWebRequest.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.Created)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	public string DBD_GetMyFriends()
	{
		string result = "";
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/players/me/friends");
			obj.Proxy = new WebProxy();
			obj.Method = "GET";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-provider", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			obj.ContentLength = 0L;
			WebResponse response = obj.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	public string DBD_GetMatchInfo(string matchID)
	{
		string result = "";
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://" + DefaultDomain + "/api/v1/match/" + matchID);
			obj.Proxy = new WebProxy();
			obj.Method = "GET";
			obj.ProtocolVersion = HttpVersion.Version11;
			obj.Host = DefaultDomain;
			obj.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			obj.Headers.Add("Cookie", "bhvrSession=" + LastBhvrSession);
			obj.Headers.Add("x-kraken-client-platform", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-provider", GetKrakenPlatformProviderFromHostname(DefaultDomain));
			obj.Headers.Add("x-kraken-client-resolution", "1920x1080");
			obj.Headers.Add("x-kraken-client-timezone-offset", "-180");
			obj.Headers.Add("x-kraken-client-os", "10.0.22621.1.256.64bit");
			obj.Headers.Add("x-kraken-client-version", x_kraken_client_version);
			obj.UserAgent = user_agent;
			obj.ContentType = "application/json";
			obj.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			obj.ContentLength = 0L;
			WebResponse response = obj.GetResponse();
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				result = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet)).ReadToEnd();
				responseStream.Close();
			}
			response.Close();
		}
		catch (WebException ex)
		{
			result = ((ex.Response as HttpWebResponse)?.StatusCode).ToString();
		}
		return result;
	}

	private void richTextBox2_TextChanged(object sender, EventArgs e)
	{
		Interlocked.Exchange(ref MyKeywords, richTextBox2.Text);
		File.WriteAllText("keywords.txt", richTextBox2.Text);
	}

	private void richTextBox3_TextChanged(object sender, EventArgs e)
	{
		Interlocked.Exchange(ref MyExpludeList, richTextBox3.Text);
		File.WriteAllText("excludeList.txt", richTextBox3.Text);
	}

	private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start("https://fmp.su");
	}

	private void button3_Click(object sender, EventArgs e)
	{
		if (LastPlayerIds.Count > 0)
		{
			new Form2(this).ShowDialog();
			return;
		}
		string[] array = new string[4] { "FUCK U", "GO DIE", "KYS", "UR IDIOT?" };
		Random random = new Random();
		MessageBox.Show("Players list is empty", array[random.Next(0, array.Length)], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	}

	private void label3_DoubleClick(object sender, EventArgs e)
	{
		DontCheckOthers = !DontCheckOthers;
		OutputLog($"DontCheckOthers = {DontCheckOthers}");
		MessageBox.Show("Yes it is!\t", "Omg...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	}

	private void label1_DoubleClick(object sender, EventArgs e)
	{
		richTextBox1.Clear();
		OutputLog("output log just cleared");
	}

	private void button4_Click(object sender, EventArgs e)
	{
		if (LastBhvrSession.Length > 0 && DefaultDomain.Length > 0)
		{
			new AddMePlease(this).ShowDialog();
		}
		else
		{
			MessageBox.Show("Session is invalid!\t", "???", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private void checkBox1_Click(object sender, EventArgs e)
	{
		if (!DontWait)
		{
			DontWait = true;
		}
		else
		{
			checkBox1.Checked = false;
			DontWait = false;
		}
		OutputLog($"DontWait = {DontWait}");
	}

	private void checkBox3_Click(object sender, EventArgs e)
	{
		if (!SkipSameLobbies)
		{
			new Form3(this).ShowDialog();
			OutputLog($"SkipSameDelay = {SkipSameDelay}");
			OutputLog($"SkipSameWaitOthers = {SkipSameWaitOthers}");
			OutputLog($"SkipSameWaitKillerOnly = {SkipSameWaitKillerOnly}");
			SkipSameLobbies = true;
		}
		else
		{
			checkBox3.Checked = false;
			SkipSameLobbies = false;
		}
		OutputLog($"SkipSameLobbies = {SkipSameLobbies}");
	}

	private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (SearchForIdx != comboBox2.SelectedIndex)
		{
			SearchForIdx = comboBox2.SelectedIndex;
			OutputLog($"SearchFor = {comboBox2.GetItemText(comboBox2.SelectedItem)}, Idx = {comboBox2.SelectedIndex}");
		}
	}

	private void button5_Click(object sender, EventArgs e)
	{
		if (LastBhvrSession.Length > 0)
		{
			Clipboard.SetText(LastBhvrSession);
			MessageBox.Show("bhvrSession copied!\t", "Fap on it", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
		else
		{
			MessageBox.Show("No cookie!\t", "???!!??", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.richTextBox1 = new System.Windows.Forms.RichTextBox();
		this.button1 = new System.Windows.Forms.Button();
		this.button2 = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.richTextBox2 = new System.Windows.Forms.RichTextBox();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.groupBox4 = new System.Windows.Forms.GroupBox();
		this.label1 = new System.Windows.Forms.Label();
		this.comboBox1 = new System.Windows.Forms.ComboBox();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.richTextBox3 = new System.Windows.Forms.RichTextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.linkLabel1 = new System.Windows.Forms.LinkLabel();
		this.label3 = new System.Windows.Forms.Label();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
		this.button3 = new System.Windows.Forms.Button();
		this.button4 = new System.Windows.Forms.Button();
		this.checkBox3 = new System.Windows.Forms.CheckBox();
		this.comboBox2 = new System.Windows.Forms.ComboBox();
		this.groupBox5 = new System.Windows.Forms.GroupBox();
		this.groupBox6 = new System.Windows.Forms.GroupBox();
		this.button5 = new System.Windows.Forms.Button();
		this.groupBox1.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.groupBox4.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox5.SuspendLayout();
		this.groupBox6.SuspendLayout();
		base.SuspendLayout();
		this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBox1.Location = new System.Drawing.Point(8, 23);
		this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.ReadOnly = true;
		this.richTextBox1.Size = new System.Drawing.Size(665, 182);
		this.richTextBox1.TabIndex = 0;
		this.richTextBox1.Text = "";
		this.button1.Location = new System.Drawing.Point(8, 23);
		this.button1.Margin = new System.Windows.Forms.Padding(4);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(100, 28);
		this.button1.TabIndex = 1;
		this.button1.Text = "Start";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.button2.Location = new System.Drawing.Point(116, 23);
		this.button2.Margin = new System.Windows.Forms.Padding(4);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(100, 28);
		this.button2.TabIndex = 2;
		this.button2.Text = "Stop";
		this.button2.UseVisualStyleBackColor = true;
		this.button2.Click += new System.EventHandler(button2_Click);
		this.groupBox1.Controls.Add(this.richTextBox2);
		this.groupBox1.Location = new System.Drawing.Point(16, 295);
		this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox1.Size = new System.Drawing.Size(341, 213);
		this.groupBox1.TabIndex = 6;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Keywords (nickname, cloudid, steam64):";
		this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBox2.Location = new System.Drawing.Point(8, 23);
		this.richTextBox2.Margin = new System.Windows.Forms.Padding(4);
		this.richTextBox2.Name = "richTextBox2";
		this.richTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
		this.richTextBox2.Size = new System.Drawing.Size(333, 182);
		this.richTextBox2.TabIndex = 7;
		this.richTextBox2.Text = "";
		this.richTextBox2.WordWrap = false;
		this.richTextBox2.TextChanged += new System.EventHandler(richTextBox2_TextChanged);
		this.groupBox3.Controls.Add(this.button1);
		this.groupBox3.Controls.Add(this.button2);
		this.groupBox3.Location = new System.Drawing.Point(16, 9);
		this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox3.Size = new System.Drawing.Size(223, 58);
		this.groupBox3.TabIndex = 8;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "Proxy";
		this.groupBox4.Controls.Add(this.richTextBox1);
		this.groupBox4.Location = new System.Drawing.Point(16, 75);
		this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox4.Name = "groupBox4";
		this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox4.Size = new System.Drawing.Size(683, 213);
		this.groupBox4.TabIndex = 9;
		this.groupBox4.TabStop = false;
		this.groupBox4.Text = "Logs:";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(432, 2);
		this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(73, 16);
		this.label1.TabIndex = 10;
		this.label1.Text = "MMR: 1800";
		this.label1.DoubleClick += new System.EventHandler(label1_DoubleClick);
		this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Location = new System.Drawing.Point(8, 23);
		this.comboBox1.Margin = new System.Windows.Forms.Padding(4);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(141, 24);
		this.comboBox1.TabIndex = 11;
		this.groupBox2.Controls.Add(this.richTextBox3);
		this.groupBox2.Location = new System.Drawing.Point(373, 295);
		this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox2.Size = new System.Drawing.Size(325, 213);
		this.groupBox2.TabIndex = 12;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Exclude List:";
		this.richTextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBox3.Location = new System.Drawing.Point(8, 23);
		this.richTextBox3.Margin = new System.Windows.Forms.Padding(4);
		this.richTextBox3.Name = "richTextBox3";
		this.richTextBox3.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
		this.richTextBox3.Size = new System.Drawing.Size(309, 182);
		this.richTextBox3.TabIndex = 7;
		this.richTextBox3.Text = "";
		this.richTextBox3.WordWrap = false;
		this.richTextBox3.TextChanged += new System.EventHandler(richTextBox3_TextChanged);
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(517, 522);
		this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(82, 16);
		this.label2.TabIndex = 13;
		this.label2.Text = "Powered by ";
		this.label2.DoubleClick += new System.EventHandler(label1_DoubleClick);
		this.linkLabel1.AutoSize = true;
		this.linkLabel1.Location = new System.Drawing.Point(600, 522);
		this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.linkLabel1.Name = "linkLabel1";
		this.linkLabel1.Size = new System.Drawing.Size(85, 16);
		this.linkLabel1.TabIndex = 14;
		this.linkLabel1.TabStop = true;
		this.linkLabel1.Text = "https://fmp.su";
		this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(12, 522);
		this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(84, 16);
		this.label3.TabIndex = 15;
		this.label3.Text = "Free release";
		this.label3.DoubleClick += new System.EventHandler(label3_DoubleClick);
		this.checkBox1.AutoSize = true;
		this.checkBox1.Location = new System.Drawing.Point(429, 22);
		this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(86, 20);
		this.checkBox1.TabIndex = 16;
		this.checkBox1.Text = "Don't wait";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox1.Click += new System.EventHandler(checkBox1_Click);
		this.button3.Location = new System.Drawing.Point(296, 516);
		this.button3.Margin = new System.Windows.Forms.Padding(4);
		this.button3.Name = "button3";
		this.button3.Size = new System.Drawing.Size(100, 28);
		this.button3.TabIndex = 18;
		this.button3.Text = "Players List";
		this.button3.UseVisualStyleBackColor = true;
		this.button3.Click += new System.EventHandler(button3_Click);
		this.button4.Location = new System.Drawing.Point(409, 516);
		this.button4.Margin = new System.Windows.Forms.Padding(4);
		this.button4.Name = "button4";
		this.button4.Size = new System.Drawing.Size(87, 28);
		this.button4.TabIndex = 19;
		this.button4.Text = "Sender";
		this.button4.UseVisualStyleBackColor = true;
		this.button4.Click += new System.EventHandler(button4_Click);
		this.checkBox3.AutoSize = true;
		this.checkBox3.Location = new System.Drawing.Point(429, 46);
		this.checkBox3.Margin = new System.Windows.Forms.Padding(4);
		this.checkBox3.Name = "checkBox3";
		this.checkBox3.Size = new System.Drawing.Size(95, 20);
		this.checkBox3.TabIndex = 20;
		this.checkBox3.Text = "Skip Same";
		this.checkBox3.UseVisualStyleBackColor = true;
		this.checkBox3.Click += new System.EventHandler(checkBox3_Click);
		this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBox2.FormattingEnabled = true;
		this.comboBox2.Location = new System.Drawing.Point(8, 23);
		this.comboBox2.Margin = new System.Windows.Forms.Padding(4);
		this.comboBox2.Name = "comboBox2";
		this.comboBox2.Size = new System.Drawing.Size(131, 24);
		this.comboBox2.TabIndex = 21;
		this.comboBox2.SelectedIndexChanged += new System.EventHandler(comboBox2_SelectedIndexChanged);
		this.groupBox5.Controls.Add(this.comboBox2);
		this.groupBox5.Location = new System.Drawing.Point(261, 9);
		this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox5.Name = "groupBox5";
		this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox5.Size = new System.Drawing.Size(148, 59);
		this.groupBox5.TabIndex = 22;
		this.groupBox5.TabStop = false;
		this.groupBox5.Text = "Search For:";
		this.groupBox6.Controls.Add(this.comboBox1);
		this.groupBox6.Location = new System.Drawing.Point(540, 9);
		this.groupBox6.Margin = new System.Windows.Forms.Padding(4);
		this.groupBox6.Name = "groupBox6";
		this.groupBox6.Padding = new System.Windows.Forms.Padding(4);
		this.groupBox6.Size = new System.Drawing.Size(159, 59);
		this.groupBox6.TabIndex = 23;
		this.groupBox6.TabStop = false;
		this.groupBox6.Text = "Region:";
		this.button5.Location = new System.Drawing.Point(107, 516);
		this.button5.Margin = new System.Windows.Forms.Padding(4);
		this.button5.Name = "button5";
		this.button5.Size = new System.Drawing.Size(100, 28);
		this.button5.TabIndex = 24;
		this.button5.Text = "Grab Cookie";
		this.button5.UseVisualStyleBackColor = true;
		this.button5.Click += new System.EventHandler(button5_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(715, 549);
		base.Controls.Add(this.button5);
		base.Controls.Add(this.groupBox6);
		base.Controls.Add(this.groupBox5);
		base.Controls.Add(this.checkBox3);
		base.Controls.Add(this.button4);
		base.Controls.Add(this.button3);
		base.Controls.Add(this.checkBox1);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.linkLabel1);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.groupBox4);
		base.Controls.Add(this.groupBox3);
		base.Controls.Add(this.groupBox1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Margin = new System.Windows.Forms.Padding(4);
		base.MaximizeBox = false;
		base.Name = "Form1";
		this.Text = "DBD Finder v2.6 (8.4.1+)";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);
		this.groupBox1.ResumeLayout(false);
		this.groupBox3.ResumeLayout(false);
		this.groupBox4.ResumeLayout(false);
		this.groupBox2.ResumeLayout(false);
		this.groupBox5.ResumeLayout(false);
		this.groupBox6.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
