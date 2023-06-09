// largely based on https://gist.github.com/amimaro/10e879ccb54b2cacae4b81abea455b10
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;

public class HTTPListener : MonoBehaviour
{

	private HttpListener listener;
	private Thread listenerThread;
	public ActivityReceiver ActivityReceiver;
	public GazeDataSender GazeDataSender;
	public GameObject actiReceiver;

	public GameObject HL2IPText;

	public string httpTmpActivity = "";
	public float httpTmpProbability = 0f;
	public bool httpNewActivityArrived = false;

	public bool NewDesktopIPArrived = false;
	public string TmpDesktopIP = "";
	public string TmpDesktopPort = "";



	void Start()
	{
		Debug.Log("start HTTPListener script");

		listener = new HttpListener();

		var thisIP = GetIP4Address();
		var thisPort = GetRandomUnusedPort();
		var ipAndPort = $"http://{thisIP}:{thisPort}/";
		HL2IPText.GetComponent<TextMeshPro>().text = ipAndPort;
		
		listener.Prefixes.Add(ipAndPort); 

		listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
		listener.Start();

		listenerThread = new Thread(StartListener);
		listenerThread.Start();
		Debug.Log($"Server Started at {ipAndPort}");

		ActivityReceiver = actiReceiver.GetComponent<ActivityReceiver>();

		// there is only one ActivityReceiver in the scene
		if (ActivityReceiver == null)
		{
			Debug.Log($"ActivityReceiver is null");
			var activityReceivers = GameObject.FindGameObjectsWithTag("Activity");
			ActivityReceiver = activityReceivers[0].GetComponent<ActivityReceiver>();
		}
		Debug.Log($"ActivityReceiver is {ActivityReceiver.tag}");

	}


	/// <summary>
	/// Gets the IP v4 address from the current device
	/// </summary>
	/// <returns>ip address as string</returns>
	public static string GetIP4Address()
	{
		string IP4Address = String.Empty;
		foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
		{
			if (IPA.AddressFamily == AddressFamily.InterNetwork)
			{
				IP4Address = IPA.ToString();
				break;
			}
		}
		return IP4Address;
	}

	// from here: https://stackoverflow.com/a/3978040
	public static int GetRandomUnusedPort()
	{
		var listener = new TcpListener(IPAddress.Any, 0);
		listener.Start();
		var port = ((IPEndPoint)listener.LocalEndpoint).Port;
		listener.Stop();
		return port;
	}

	private void StartListener()
	{
		while (true)
		{
			var result = listener.BeginGetContext(ListenerCallback, listener);
			result.AsyncWaitHandle.WaitOne();
		}
	}

	private void ListenerCallback(IAsyncResult result)
	{
		var context = listener.EndGetContext(result);

		Debug.Log("Request received");
		var activity = "";
		var probability = 0f;
		var desktopIP = "";
		var desktopPort = "";

		try
		{
			if (context.Request.QueryString.AllKeys.Length > 0)
			{
				foreach (var key in context.Request.QueryString.AllKeys)
				{
					var value = context.Request.QueryString.GetValues(key)[0];

					Debug.Log($"key: {key}");
					Debug.Log($"value: {value}");


					if (key == "activity")
					{
						activity = value;
						Debug.Log("is activity");

					}
					else if (key == "probability")
					{
						Debug.Log("is activity");
						float.TryParse(value, out probability);

					}

					if (key == "desktopip")
					{
						Debug.Log("is desktopip");
						desktopIP = value;

					}
					else if (key == "port")
					{

						desktopPort = value;
						Debug.Log($"is port: {desktopPort}");
						Debug.Log($"Did receive full new desktop IP: {desktopIP}:{desktopPort}.");

					}
					Debug.Log($"Did receive full new desktop IP: {desktopIP}:{desktopPort}.");
				}

			}
			
		}
		finally
        {
			// context.Response.Close();
			context.Response.OutputStream.Close();

			// if (desktopIP != "" && desktopPort != "")
			if (!string.IsNullOrEmpty(desktopIP) && !string.IsNullOrEmpty(desktopPort))
			{
				Debug.Log($"Received new desktop IP: {desktopIP}:{desktopPort}.");
				NewDesktopIPArrived = true;
				TmpDesktopIP = desktopIP;
				TmpDesktopPort = desktopPort;
				
			}
			// only proceed if we have both, a new activity and a new probability
			else if (activity != "" && probability != 0f)
			{
				ActivityReceiver.tmpActivity = activity;
				ActivityReceiver.tmpProbability = probability;
				ActivityReceiver.newActivityArrived = true;
				httpTmpActivity = activity;
				httpTmpProbability = probability;
				httpNewActivityArrived = true;
				Debug.Log($"ActivityReceiver.newActivityArrived: {ActivityReceiver.newActivityArrived}");
				Debug.Log($"Received new activity: {activity} with probability {probability}.");
			}



		}

	

	}
}
