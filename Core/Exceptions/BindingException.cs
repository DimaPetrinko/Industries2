using System;

namespace Core.Exceptions
{
	public abstract class BindingException : Exception
	{
		public Type Type { get; }

		protected BindingException(
			Type type,
			string message
		) : base(message)
		{
			Type = type;
		}
	}
}