using System.ComponentModel;

namespace WinFormsAppAttribute
{
	public class PropertyOverridingTypeDescriptor : CustomTypeDescriptor
	{
		private readonly Dictionary<string, PropertyDescriptor> overridePropertyDescriptor = new Dictionary<string, PropertyDescriptor>();
		public PropertyOverridingTypeDescriptor(ICustomTypeDescriptor parent) : base(parent) { }
		public void OverrideProperty(PropertyDescriptor pd) 
		{ 
			this.overridePropertyDescriptor[pd.Name] = pd; 
		}

		public List<Attribute> Attributes { get; } = new List<Attribute>();

		public override AttributeCollection GetAttributes()
		{
			var attribures = base.GetAttributes();
			
			var list = attribures.Cast<Attribute>().ToList();
			list.AddRange(this.Attributes);

			Attribute[] attributes = list.ToArray();
			return new AttributeCollection(attributes);
		}

		public override object GetPropertyOwner(PropertyDescriptor pd)
		{
			object o = base.GetPropertyOwner(pd);

			if (o == null)
				return this;

			return o;
		}
		public PropertyDescriptorCollection CreatePropertyCollectionWithNewAttributes(PropertyDescriptorCollection propertyDescriptorCollection)
		{
			var propertyDescriptorArray = CreatePropertyDescriptorsWithNewAttributes(propertyDescriptorCollection);
			var propertyDescriptorCollectionWithRewrittenAttributes = new PropertyDescriptorCollection(propertyDescriptorArray);

			return propertyDescriptorCollectionWithRewrittenAttributes;
		}

		private PropertyDescriptor[] CreatePropertyDescriptorsWithNewAttributes(PropertyDescriptorCollection propertyDescriptorCollection)
		{
			var descriptors = GenerateDescriptors(propertyDescriptorCollection);
			return GetArrayOf(descriptors);
		}

		private PropertyDescriptor[] GetArrayOf(IEnumerable<PropertyDescriptor> descriptors) 
			=> descriptors.ToArray();

		private IEnumerable<PropertyDescriptor> GenerateDescriptors(PropertyDescriptorCollection propertyDescriptorCollection)
		{
			foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
				yield return GetOrCreatePropertyDescriptorFor(propertyDescriptor);
		}

		private PropertyDescriptor GetOrCreatePropertyDescriptorFor(PropertyDescriptor pd)
		{
			if (!overridePropertyDescriptor.ContainsKey(pd.Name))
				return pd;

			var overridePropertyDescriptors = overridePropertyDescriptor[pd.Name];
			return overridePropertyDescriptors;
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			var propertyDescriptorCollection = base.GetProperties();
			return CreatePropertyCollectionWithNewAttributes(propertyDescriptorCollection);
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var propertyDescriptorCollection = base.GetProperties(attributes);
			return CreatePropertyCollectionWithNewAttributes(propertyDescriptorCollection);
		}
	}
}
