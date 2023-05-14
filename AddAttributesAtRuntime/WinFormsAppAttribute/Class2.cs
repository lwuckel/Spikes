using System.ComponentModel;

namespace WinFormsAppAttribute
{
	[DisplayName("Class 2")]
	public class Class2
	{
		public string Test { get; set; }
		public List<Class4> TestList4 { get; set; }
	}

	public class Class3
	{
		public string Test { get; set; }
	}

	[DisplayName("Class 4")]
	public class Class4
	{
		public string Test { get; set; }
		public List<Class3> TestList3 { get; set; }

		public Class3 class3 { get; set; }
	}

}
