using System.ComponentModel;
using System.Globalization;

namespace WinFormsAppAttribute
{
	public class ExpandableObjectEmptyValueConverter : ExpandableObjectConverter
	{
		public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
		{
			return "";
		}
	}
}
