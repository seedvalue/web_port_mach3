using System;
using UnityEngine;
using Utils;

namespace Tutorial
{
	public class Tutorial : Element
	{
		public bool activeInMeta = true;

		public bool activeInM3;

		public Action completeCallback;

		private bool activePending;

		protected override void Start()
		{
			base.Start();
			SceneLoader.BeginLoading += RefreshActivity;
			SceneLoader.EndLoading += RefreshActivity;
			SequenceManager.BeginSequence += RefreshActivity;
			SequenceManager.EndSequence += RefreshActivity;
			RefreshActivity();
		}

		protected virtual void OnDestroy()
		{
			SceneLoader.BeginLoading -= RefreshActivity;
			SceneLoader.EndLoading -= RefreshActivity;
			SequenceManager.BeginSequence -= RefreshActivity;
			SequenceManager.EndSequence -= RefreshActivity;
		}

		protected override void OnComplete()
		{
			base.OnComplete();
			if (completeCallback != null)
			{
				completeCallback();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		protected override void OnSubComplete()
		{
			base.OnSubComplete();
			base.complete = true;
		}

		private void RefreshActivity()
		{
			SceneLoader instance = Singleton<SceneLoader>.Instance;
			SequenceManager instance2 = Singleton<SequenceManager>.Instance;
			bool flag = false;
			if (instance.loading || instance2.playing)
			{
				flag = false;
			}
			else if (instance.inMeta)
			{
				flag = activeInMeta;
			}
			else if (instance.inMatch3)
			{
				flag = activeInM3;
			}
			if (flag)
			{
				activePending = true;
				return;
			}
			base.active = false;
			activePending = false;
		}

		private void LateUpdate()
		{
			base.active = activePending;
		}
	}
}
