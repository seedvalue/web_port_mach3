using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;

public class MetaTimeManager : MetaComponent<MetaTimeManager>
{
	[MetaData(null, 0)]
	private TimeSpan _cheatOffset = default(TimeSpan);

	[MetaData(null, 0)]
	private TimeSpan _gameTimeSinceInstall = default(TimeSpan);

	[MetaData(null, 0)]
	private DateTime? _installTime;

	private bool offsetInstallTime;

	private TimeSpan _gameTimeSinceStart;

	protected const float RetryWaitTime = 1f;

	protected const int ConnectionTimeout = 5000;

	protected static string[] hosts = new string[9]
	{
		"time.nist.gov",
		"pool.ntp.org",
		"europe.pool.ntp.org",
		"asia.pool.ntp.org",
		"oceania.pool.ntp.org",
		"north-america.pool.ntp.org",
		"south-america.pool.ntp.org",
		"ntp1.inrim.it",
		"ntp2.inrim.it"
	};

	private int hostIndex;

	public bool syncing
	{
		get;
		private set;
	}

	public bool syncDone
	{
		get;
		private set;
	}

	public TimeSpan serverOffset
	{
		get;
		private set;
	}

	public TimeSpan cheatOffset
	{
		get
		{
			return _cheatOffset;
		}
		set
		{
			_cheatOffset = value;
		}
	}

	public TimeSpan realTimeSinceInstall
	{
		get;
		private set;
	}

	public TimeSpan gameTimeSinceInstall => _gameTimeSinceInstall;

	public DateTime utcNow
	{
		get;
		private set;
	}

	public DateTime localNow
	{
		get;
		private set;
	}

	protected virtual void MetaReset()
	{
		cheatOffset = new TimeSpan(0L);
		_gameTimeSinceInstall = default(TimeSpan);
		_installTime = null;
	}

	protected virtual void MetaStart()
	{
		if (!_installTime.HasValue)
		{
			offsetInstallTime = true;
			_installTime = DateTime.UtcNow;
		}
		realTimeSinceInstall = utcNow - _installTime.Value;
		_gameTimeSinceStart = _gameTimeSinceInstall;
	}

	protected virtual void Awake()
	{
		hostIndex = UnityEngine.Random.Range(0, hosts.Length);
		localNow = DateTime.Now + serverOffset + cheatOffset;
		utcNow = DateTime.UtcNow + serverOffset + cheatOffset;
		UpdateTime();
	}

	protected virtual void Update()
	{
		localNow = DateTime.Now + serverOffset + cheatOffset;
		utcNow = DateTime.UtcNow + serverOffset + cheatOffset;
		if (_installTime.HasValue)
		{
			realTimeSinceInstall = utcNow - _installTime.Value;
		}
		_gameTimeSinceInstall = _gameTimeSinceStart + TimeSpan.FromSeconds(Time.unscaledTime);
	}

	protected virtual void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			UpdateTime();
		}
	}

	private void UpdateTime()
	{
		if (!syncing)
		{
			StartCoroutine(UpdateTimeCoroutine());
		}
	}

	private IEnumerator UpdateTimeCoroutine()
	{
		syncing = true;
		DateTime? serverTime;
		while (true)
		{
			string host = GetNextHost();
			CoroutineWithResult hostTimeCoroutine = this.StartCoroutineWithResult(GetTimeFromHostCoroutine(host));
			yield return hostTimeCoroutine.coroutine;
			serverTime = (hostTimeCoroutine.result as DateTime?);
			if (serverTime.HasValue)
			{
				break;
			}
			UnityEngine.Debug.LogWarningFormat("Unable to resolve host: {0}", host);
			yield return new WaitForSeconds(1f);
		}
		serverOffset = serverTime.GetValueOrDefault() - DateTime.UtcNow;
		syncDone = true;
		if (offsetInstallTime)
		{
			offsetInstallTime = false;
			DateTime? installTime = _installTime;
			_installTime = ((!installTime.HasValue) ? null : new DateTime?(installTime.GetValueOrDefault() + serverOffset));
		}
		yield return serverTime;
		syncing = false;
	}

	private string GetNextHost()
	{
		hostIndex = (hostIndex + 1) % hosts.Length;
		return hosts[hostIndex];
	}

	private IEnumerator GetTimeFromHostCoroutine(string host)
	{
		IAsyncResult asyncResult = null;
		Exception error = null;
		DateTime result = default(DateTime);
		byte[] data = new byte[48];
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		try
		{
			socket.ReceiveTimeout = 5000;
			try
			{
				asyncResult = socket.BeginConnect(host, 123, null, null);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			if (error != null)
			{
				yield return error;
			}
			while (!asyncResult.IsCompleted)
			{
				yield return null;
			}
			try
			{
				socket.EndConnect(asyncResult);
				data[0] = 35;
				for (int i = 1; i < 48; i++)
				{
					data[i] = 0;
				}
				asyncResult = socket.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
			}
			catch (Exception ex2)
			{
				error = ex2;
			}
			if (error != null)
			{
				yield return error;
			}
			while (!asyncResult.IsCompleted)
			{
				yield return null;
			}
			try
			{
				socket.EndSend(asyncResult);
				asyncResult = socket.BeginReceive(data, 0, data.Length, SocketFlags.None, null, null);
			}
			catch (Exception ex3)
			{
				error = ex3;
			}
			if (error != null)
			{
				yield return error;
			}
			while (!asyncResult.IsCompleted)
			{
				yield return null;
			}
			try
			{
				socket.EndReceive(asyncResult);
				socket.Close();
				byte[] value = new byte[4]
				{
					data[43],
					data[42],
					data[41],
					data[40]
				};
				byte[] value2 = new byte[4]
				{
					data[47],
					data[46],
					data[45],
					data[44]
				};
				long num = (long)BitConverter.ToUInt32(value, 0) * 1000L + (long)((ulong)((long)BitConverter.ToUInt32(value2, 0) * 1000L) / 4294967296uL);
				result = new DateTime(1900, 1, 1);
				result += TimeSpan.FromTicks(num * 10000);
			}
			catch (Exception ex4)
			{
				error = ex4;
			}
			if (error != null)
			{
				yield return error;
			}
			yield return result;
		}
		finally
		{
			//base._003C_003E__Finally0();
		}
	}
}
