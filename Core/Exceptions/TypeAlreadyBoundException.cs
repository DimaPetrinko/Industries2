using System;

namespace Core.Exceptions
{
	public class TypeAlreadyBoundException : BindingException
	{
		public object Instance { get; }
		public object BoundInstance { get; }

		public TypeAlreadyBoundException(
			Type type,
			object instance,
			object boundInstance
		) : base(
			type,
			$"Type {type} is already bound to {boundInstance}. Was trying to bind {instance}"
		)
		{
			Instance = instance;
			BoundInstance = boundInstance;
		}
	}
}