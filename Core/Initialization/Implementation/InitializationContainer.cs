using System;
using System.Collections.Generic;
using Core.Exceptions;

namespace Core.Initialization.Implementation
{
	public class InitializationContainer : IInitializationContainer
	{
		private readonly object mLockObject = new object();
		private readonly Dictionary<Type, object> mBindings = new Dictionary<Type, object>();

		public void Bind<T>(T instance)
		{
			lock (mLockObject)
			{
				var type = typeof(T);
				if (mBindings.TryGetValue(type, out var boundInstance))
				{
					throw new TypeAlreadyBoundException(type, instance, boundInstance);
				}

				mBindings.Add(type, instance);
			}
		}

		public T Get<T>()
		{
			lock (mLockObject)
			{
				var type = typeof(T);
				if (!mBindings.TryGetValue(type, out var instance))
					throw new TypeNotBoundException(type);

				return (T)instance;
			}
		}

		public bool Has<T>()
		{
			lock (mLockObject)
			{
				var type = typeof(T);
				return mBindings.ContainsKey(type);
			}
		}

		public void Flush()
		{
			lock (mLockObject)
			{
				mBindings.Clear();
			}
		}
	}
}