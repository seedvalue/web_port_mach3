using System.Linq;
using UnityEngine;

namespace Tutorial
{
	public abstract class Element : MonoBehaviour
	{
		private bool _active;

		private bool _complete;

		private bool _subComplete;

		private bool _completeState;

		private bool _subCompleteState;

		private bool ignoreSubChanged;

		private bool started;

		public Element parentElement
		{
			get;
			private set;
		}

		public Element[] subElements
		{
			get;
			private set;
		}

		public bool active
		{
			get
			{
				return _active;
			}
			protected set
			{
				if (_active != value)
				{
					_active = value;
					NotifyActive();
				}
			}
		}

		public bool complete
		{
			get
			{
				return _complete;
			}
			protected set
			{
				if (_complete != value)
				{
					_complete = value;
					NotifyComplete();
				}
			}
		}

		public bool subComplete
		{
			get
			{
				return _subComplete;
			}
			private set
			{
				if (_subComplete != value)
				{
					_subComplete = value;
					NotifySubComplete();
				}
			}
		}

		protected virtual void Start()
		{
			Transform parent = base.transform.parent;
			if ((bool)parent)
			{
				parentElement = parent.GetComponentInParent<Element>();
			}
			subElements = (from i in Enumerable.Range(0, base.transform.childCount)
				select base.transform.GetChild(i).GetComponent<Element>() into e
				where e != null
				select e).ToArray();
			started = true;
			if (active)
			{
				NotifyActive();
			}
		}

		private void NotifyActive()
		{
			if (started)
			{
				ignoreSubChanged = true;
				if (active)
				{
					OnActivate();
					RefreshSub();
					NotifySubComplete();
					NotifyComplete();
				}
				if (!active)
				{
					RefreshSub();
					OnDeactivate();
				}
				ignoreSubChanged = false;
			}
		}

		private void NotifyComplete()
		{
			if (active && _completeState != complete)
			{
				_completeState = complete;
				RefreshSub();
				if (complete)
				{
					OnComplete();
				}
				else
				{
					OnIncomplete();
				}
				if ((bool)parentElement)
				{
					parentElement.OnSubChanged();
				}
			}
		}

		private void NotifySubComplete()
		{
			if (active && _subCompleteState != subComplete)
			{
				_subCompleteState = subComplete;
				if (subComplete)
				{
					OnSubComplete();
				}
				else
				{
					OnSubIncomplete();
				}
			}
		}

		private void RefreshSub()
		{
			if (!active || complete)
			{
				Element[] subElements = this.subElements;
				foreach (Element element in subElements)
				{
					element.active = false;
				}
				return;
			}
			bool flag = true;
			Element[] subElements2 = this.subElements;
			foreach (Element element2 in subElements2)
			{
				element2.active = flag;
				flag &= element2.complete;
			}
			subComplete = flag;
		}

		public void OnSubChanged()
		{
			if (!ignoreSubChanged)
			{
				RefreshSub();
			}
		}

		protected virtual void OnActivate()
		{
		}

		protected virtual void OnDeactivate()
		{
		}

		protected virtual void OnComplete()
		{
		}

		protected virtual void OnIncomplete()
		{
		}

		protected virtual void OnSubComplete()
		{
		}

		protected virtual void OnSubIncomplete()
		{
		}
	}
}
