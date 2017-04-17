using System;
using WebAPIPrototypeA.Tests;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace TestRunner
{
	/// <summary>
	/// Mono doesn't like the test runner, microsoft test suite, or NUNIT (the system gets confused if nunit is in two assemblies, the running assembly and the test assembly)
	/// and was kinda fruitless trying to figure out how to get it to work.
	/// Here is a quick and dirty test runner which runs through all non static classes in the tests assembly
	/// then runs through the methods on the class, calls setup and cleanup alongside the method
	/// Nunit on mono kinda sucks too - just very finicky and brittle. Add a custom attribute to the class, init, teardown and test methods 
	/// that we want to call
	/// </summary>
	class MainClass
	{
		public static void Main(string[] args)
		{
			TestRunner testRunner = new TestRunner(Assembly.GetAssembly(typeof(WebAPIPrototypeA.Tests.ChannelRepositoryTests)));
			testRunner.RunTests();

			Console.WriteLine($"Total tests run: { testRunner.TotalTestsRun }");
			Console.WriteLine($"Total tests failed: { testRunner.TotalTestsFailed }");
			Console.WriteLine($"Test run finished in { testRunner.ElapsedRunTime }");
			Console.WriteLine($"Success rate: { testRunner.PercentSuccess }%");
		}
	}

	internal class TestRunner 
	{
		private Assembly testAssembly { get; set; }
		private Stopwatch timer { get; set; }
		private string divider {get{return "-----------------------------------------------------------------------------------------------------------------------";}}
		
		public string ElapsedRunTime { get; set; }
		public decimal PercentSuccess
		{
			get
			{
				if (this.TotalTestsRun > 0)
					if (this.TotalTestsFailed > 0)
					{
						decimal successRate = ((decimal)(this.TotalTestsRun - this.TotalTestsFailed) / (decimal)this.TotalTestsRun);
					return Math.Round((successRate * 100), 2);
				}
					else return 100;
				else return 0;
			}
		}
				
		public int TotalTestsRun
		{
			get;
			set;
		}
		public int TotalTestsFailed
		{
			get;
			set;
		}

		public TestRunner(Assembly passedTestAssembly)
		{
			this.testAssembly = passedTestAssembly;
			this.timer = new Stopwatch();
		}

		public void RunTests()
		{
			this.timer.Start();

			// CLR uses the is static and is sealed to denote static classes. An optimisation, so check that the class is NOT abstract and sealed
			foreach (Type item in this.testAssembly.GetTypes().Where(x => x.IsClass && !(x.IsSealed == true && x.IsAbstract == true)))
			{
				// check for our test fixture classes
				if (item.GetCustomAttributes().OfType<WebApiTestClassAttribute>().Any())
					RunClassTestMethods(item);
			}

			timer.Stop();
			TimeSpan timeTestsTaken = timer.Elapsed;

			this.ElapsedRunTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeTestsTaken.Hours, timeTestsTaken.Minutes, timeTestsTaken.Seconds, timeTestsTaken.Milliseconds / 10);
		}

		private void RunClassTestMethods(Type item)
		{
			// only get the methods with our custom test decoration
			MethodInfo[] methods = item.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
									   .Where(x => x.GetCustomAttributes().OfType<WebApiTest>().Any()).ToArray();

			Console.WriteLine($"Running { methods.Count().ToString() } tests for {item.Name}");
			Console.WriteLine();
			this.TotalTestsRun += methods.Count();

			var currentTestClassType = item as Type;
			var instance = Activator.CreateInstance(currentTestClassType);

			foreach (MethodInfo method in methods)
				this.InvokeTestMethod(currentTestClassType, instance, method);

			Console.WriteLine();
			Console.WriteLine($"Finished running tests for {item.Name}");
			Console.WriteLine(this.divider);
		}

		private void InvokeTestMethod(Type currentTestClassType, object instance, MethodInfo method)
		{
			var setUp = currentTestClassType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
								.Where(x => x.GetCustomAttributes().OfType<WebApiTestInitialise>().Any()).FirstOrDefault();

			var cleanUp = currentTestClassType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
								.Where(x => x.GetCustomAttributes().OfType<WebApiTestCleanUp>().Any()).FirstOrDefault();

			if (setUp != null)
				setUp.Invoke(instance, null);

			try
			{
				method.Invoke(instance, null);
				Console.WriteLine($"Ran {method.Name} ... PASS");

			}
			catch (Exception ex)
			{
				string exceptionMessage = (ex.InnerException != null ? ex.InnerException.Message : "no inner exception exists");
				string stackTrace = (ex.InnerException != null ? ex.InnerException.StackTrace : "no inner exception exists");
				Console.WriteLine();
				Console.WriteLine($"Ran {method.Name} ... FAILED");
				Console.WriteLine();
				Console.WriteLine($"Test failed for { method.Name }. The exception was { exceptionMessage } which happened at { stackTrace }");

				Console.WriteLine();

				this.TotalTestsFailed += 1;
			}
			finally
			{
				if (cleanUp != null)
					cleanUp.Invoke(instance, null);
			}
		}
	}
}
