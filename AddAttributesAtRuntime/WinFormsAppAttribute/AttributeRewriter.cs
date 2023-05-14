using System.Collections;
using System.ComponentModel;

namespace WinFormsAppAttribute
{
	internal class AttributeRewriter
	{
		public event Func<Type, Attribute[]> RewriteTypeAttributes;
		public event Func<Type, PropertyDescriptor, Attribute[]> RewritePropertyAttributes;

		List<Type> cachedTypes = new List<Type>();

		public void RewriteAttributes(object o)
		{
			Type type = o.GetType();
			RewriteAttributes(type);
		}

		public void RewriteAttributes(Type type)
		{
			if (IsAlreadyRewritten(type))
				return;

			MarkForAlreadyRewrote(type);

			var customTypeDescriptor = CreateNewTypeDescriptorFor(type);
			AddCustomTypeAttributes(type, customTypeDescriptor);

			var propertyDescriptorCollection = GetPropertyDescriptorCollection(type);

			foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
			{
				AddCustomPropertyAttributes(type, customTypeDescriptor, propertyDescriptor);

				if (IsGenericCollection(propertyDescriptor))
					RewriteGenericTypes(propertyDescriptor);
			}

			var provider = new TypeDescriptorOverridingProvider(customTypeDescriptor);
			TypeDescriptor.AddProvider(provider, type);
		}

		private PropertyDescriptorCollection GetPropertyDescriptorCollection(Type type) 
			=> TypeDescriptor.GetProperties(type);

		private void AddCustomPropertyAttributes(Type type, PropertyOverridingTypeDescriptor customTypeDescriptor, PropertyDescriptor propertyDescriptor)
		{
			var attributes = RewritePropertyAttributes?.Invoke(type, propertyDescriptor);

			if (attributes != null)
			{
				var newDescriptor = TypeDescriptor.CreateProperty(type, propertyDescriptor, attributes);
				customTypeDescriptor.OverrideProperty(newDescriptor);
			}
		}

		private void RewriteGenericTypes(PropertyDescriptor propertyDescriptor)
		{
			var types = propertyDescriptor.PropertyType.GetGenericArguments();

			foreach (var type in types)
				RewriteAttributes(type);
		}

		private bool IsGenericCollection(PropertyDescriptor propertyDescriptor) 
			=> propertyDescriptor.PropertyType.IsGenericType && typeof(ICollection).IsAssignableFrom(propertyDescriptor.PropertyType);

		private void AddCustomTypeAttributes(Type type, PropertyOverridingTypeDescriptor customTypeDescriptor)
		{
			var typeAttributes = RewriteTypeAttributes?.Invoke(type);

			if (typeAttributes != null)
			{
				customTypeDescriptor.Attributes.AddRange(typeAttributes);
				TypeDescriptor.AddAttributes(type, typeAttributes);
			}
		}

		private PropertyOverridingTypeDescriptor CreateNewTypeDescriptorFor(Type type)
		{
			var typeDescriptionProvider = TypeDescriptor.GetProvider(type);
			ICustomTypeDescriptor? parent = typeDescriptionProvider.GetTypeDescriptor(type);
			var ctd = new PropertyOverridingTypeDescriptor(parent);
			return ctd;
		}

		private void MarkForAlreadyRewrote(Type type) 
			=> this.cachedTypes.Add(type);

		private bool IsAlreadyRewritten(Type type) 
			=> this.cachedTypes.Contains(type);
	}
}
