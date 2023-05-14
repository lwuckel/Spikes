using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace WinFormsAppAttribute
{
	public class ExpandableCollectionConverter : CollectionConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			var collection = value as ICollection;

			if (!ExistCollection(collection))
				return base.GetProperties(context, value, attributes);

			var items = CreatePropertyDescriptorCollection(collection);

			return items;
		}

		private PropertyDescriptorCollection CreatePropertyDescriptorCollection(ICollection? collection)
		{
			var listPropertyDescriptors = GenerateListPropertyDescriptors(collection);
			var propertieDescriptors = listPropertyDescriptors.ToArray();
			var items = new PropertyDescriptorCollection(propertieDescriptors);
			return items;
		}

		private bool ExistCollection(ICollection? collection) => collection?.Count > 0;

		IEnumerable<PropertyDescriptor> GenerateListPropertyDescriptors(ICollection collection) 
		{
			for (int i = 0; i < collection.Count; i++)
			{
				IList list = GetOrCreateListFrom(collection);
				var propertyDescriptor = CreateListPropertyDescriptor(i, list);
				yield return propertyDescriptor;
			}
		}

		private ExpandableListPropertyDescriptor CreateListPropertyDescriptor(int i, IList list) 
			=> new ExpandableListPropertyDescriptor(list, i);

		private IList GetOrCreateListFrom(ICollection collection) 
			=> collection as IList ?? new ArrayList(collection);

		public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
		{
			if (value is ICollection collection)
				return GetCollectionInfo(collection);

			return base.ConvertTo(context, culture, value, destinationType);
		}

		private string GetCollectionInfo(ICollection collection)
		{
			int count = collection.Count;

			string eintrag = count == 1
				? "Eintrag"
				: "Einträge";

			return @$"{count} {eintrag}";
		}
	}
}
