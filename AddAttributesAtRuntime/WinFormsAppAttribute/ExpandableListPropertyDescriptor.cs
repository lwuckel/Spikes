using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace WinFormsAppAttribute
{
	public class ExpandableListPropertyDescriptor : PropertyDescriptor
	{
		private IList list;
		private readonly int index;

		public ExpandableListPropertyDescriptor(IList list, int idx)
				: base(GetDisplayName(list, idx), null)
		{
			this.list = list;
			this.index = idx;
		}

		private static string GetDisplayName(IList list, int index)
		{
			object? listObject = list[index];
			Type type = listObject.GetType();
			string displayName = GetDisplayNameOf(type);
			return $"[{index + 1}]  {displayName}";
		}

		private static string GetDisplayNameOf(Type type)
		{
			var attributes = TypeDescriptor.GetAttributes(type);

			var attribute = attributes.Cast<Attribute>().Where(a => a is DisplayNameAttribute).FirstOrDefault() as DisplayNameAttribute;

			if (attribute != null)
				return attribute.DisplayName;

			if (!type.IsGenericType)
				return type.Name;

			string name = type.Name;
			string genericClassName = GetGenericClassName(name);
			string parameterClassName = GetGenericParameterName(type);
			return @$"{genericClassName}<{parameterClassName}>";
		}

		private static string GetGenericParameterName(Type type)
		{
			var parameters = type.GetGenericArguments()
													.Select(GetDisplayNameOf);
			return string.Join(", ", parameters);
		}

		private static string GetGenericClassName(string name)
		{
			int index = name.IndexOf('`');
			string value = name.Substring(0, index);
			return value;
		}

		public override bool CanResetValue(object component) => true;

		public override Type ComponentType => this.list.GetType();

		public override object GetValue(object component) 
			=> this.list[this.index];

		public override bool IsReadOnly => false;

		public override string Name
			=> this.index.ToString(CultureInfo.InvariantCulture);

		public override Type PropertyType
			=> this.list[this.index].GetType();

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component) => true;
		public override void SetValue(object component, object value) 
			=> this.list[this.index] = value;
	}
}
