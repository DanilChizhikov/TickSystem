using System;

namespace DTech.TickSystem
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TickOrderAttribute : Attribute
	{
		public int Value { get; }

		public TickOrderAttribute(int order)
		{
			Value = order;
		}
	}
}