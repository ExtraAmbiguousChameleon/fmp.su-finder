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
		x_kraken_client_version = "8.4.2";
		user_agent = "DeadByDaylight/DBD_Gelato_HF2_WinGDK_Shipping_5_2200456 WinGDK/10.0.22621.0.0.64bit";
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
		this.Text = "DBD Finder v2.6 (8.4.2)";
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
