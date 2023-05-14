using System.Collections;
using System.ComponentModel;

namespace WinFormsAppAttribute
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			var data = CreateDemoData();

			RewriteAttributes(data);

			this.propertyGrid1.SelectedObject = data;
		}

		private void RewriteAttributes(Class2 data)
		{
			var rewriter = new AttributeRewriter();
			rewriter.RewriteTypeAttributes += Rewriter_RewriteTypeAttributes;
			rewriter.RewritePropertyAttributes += Rewriter_ChangeAttributes;

			rewriter.RewriteAttributes(data);
		}

		private Class2 CreateDemoData()
		{
			var c = new Class2();
			c.Test = "class2";
			c.TestList4 = new List<Class4> {
				new Class4()
				{
					Test ="cl4",
					TestList3 = new List<Class3>
					{
						new Class3{  Test ="class4"}
					},
					class3 = new Class3
					{
						Test = "poi"
					}
				}
			};
			return c;
		}

		private Attribute[] Rewriter_RewriteTypeAttributes(Type arg)
		{
			var typeConverterAttribute = new TypeConverterAttribute(typeof(ExpandableObjectEmptyValueConverter));

			if (arg == typeof(Class3))
				return new Attribute[] { new DisplayNameAttribute("Test Class 3"), typeConverterAttribute };

			return new Attribute[] {typeConverterAttribute};
			}

		private Attribute[] Rewriter_ChangeAttributes(Type arg1, System.ComponentModel.PropertyDescriptor pd)
		{
			if (pd.Name == "Test")
				return new Attribute[] { new DisplayNameAttribute("Test Property") };

			if (typeof(ICollection).IsAssignableFrom(pd.PropertyType))
				return new Attribute[] { new TypeConverterAttribute(typeof(ExpandableCollectionConverter)) };

			if (!pd.PropertyType.IsValueType)
				return new Attribute[] { new TypeConverterAttribute(typeof(ExpandableObjectEmptyValueConverter)) };

			return null;
		}
	}
}