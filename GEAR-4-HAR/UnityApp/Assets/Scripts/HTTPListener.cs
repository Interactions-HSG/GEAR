// based on https://gist.github.com/amimaro/10e879ccb54b2cacae4b81abea455b10
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class HTTPListener : MonoBehaviour
{

	private HttpListener listener;
	private Thread listenerThread;
	public ActivityReceiver ActivityReceiver;
	public GameObject actiReceiver;

	public string httpTmpActivity = "";
	public float httpTmpProbability = 0f;
	public bool httpNewActivityArrived = false;

	void Start()
	{
		Debug.Log("start HTTPListener script");

		listener = new HttpListener();

		var thisIP = GetIP4Address();
		listener.Prefixes.Add($"http://{thisIP}:5000/"); 

		listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
		listener.Start();

		listenerThread = new Thread(StartListener);
		listenerThread.Start();
		Debug.Log("Server Started");

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
					Debug.Log("is key");

				} else if (key == "probability")
                {
					float.TryParse(value, out probability);
					
				}

			}
			if (activity != "" && probability != 0f)
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
		context.Response.Close();
	}
}
