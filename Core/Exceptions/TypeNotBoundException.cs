using System;

namespace Core.Exceptions
{
	public class TypeNotBoundException : BindingException
	{
		public TypeNotBoundException(
			Type type
		) : base(
			type,
			$"Type {type} has not been bound"
		)
		{}
	}
}