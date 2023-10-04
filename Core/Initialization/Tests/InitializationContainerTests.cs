using Core.Exceptions;
using Core.Initialization.Implementation;
using NUnit.Framework;

namespace Core.Initialization.Tests
{
	internal class InitializationContainerTests
	{
		private IInitializationContainer mContainer;

		[SetUp]
		public void SetUp()
		{
			mContainer = new InitializationContainer();
		}

		[Test]
		public void Constructor_CreatesEmptyContainer()
		{
			Assert.DoesNotThrow(() =>
			{
				var container = new InitializationContainer();
			});
		}

		[Test]
		public void Bind_Adds_WhenEmpty()
		{
			var instance = "Hello";
			mContainer.Bind(instance);

			var boundInstance = mContainer.Get<string>();

			Assert.AreSame(instance, boundInstance);
		}

		[Test]
		public void Bind_Adds_WhenOtherTypeBound()
		{
			mContainer.Bind("Hello");
			mContainer.Bind(5);

			var boundString = mContainer.Get<string>();
			var boundInt = mContainer.Get<int>();

			Assert.AreEqual("Hello", boundString);
			Assert.AreEqual(5, boundInt);
		}

		[Test]
		public void Bind_Throws_IfAlreadyBound()
		{
			mContainer.Bind("Hello1");
			var e = Assert.Throws<TypeAlreadyBoundException>(() =>
			{
				mContainer.Bind("Hello2");
			});

			Assert.AreEqual(typeof(string), e.Type);
			Assert.AreEqual("Hello1", e.BoundInstance);
			Assert.AreEqual("Hello2", e.Instance);
		}

		[Test]
		public void Get_ReturnsInstance_WhenBound()
		{
			var instance = new object();
			mContainer.Bind(instance);

			var boundInstance = mContainer.Get<object>();

			Assert.AreSame(instance, boundInstance);
		}

		[Test]
		public void Get_Throws_WhenNotBound()
		{
			var e = Assert.Throws<TypeNotBoundException>(() =>
			{
				var boundInstance = mContainer.Get<object>();
			});

			Assert.AreEqual(typeof(object), e.Type);
		}

		[Test]
		public void Has_ReturnsTrue_IfBound()
		{
			mContainer.Bind("Hello");

			var result = mContainer.Has<string>();

			Assert.IsTrue(result);
		}

		[Test]
		public void Has_ReturnsFalse_IfNotBound()
		{
			var result = mContainer.Has<string>();

			Assert.IsFalse(result);
		}

		[Test]
		public void Flush_ClearsAllBindings()
		{
			mContainer.Bind("Hello");

			Assert.IsTrue(mContainer.Has<string>());

			mContainer.Flush();

			Assert.IsFalse(mContainer.Has<string>());
		}
	}
}