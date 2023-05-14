using System.ComponentModel;

namespace WinFormsAppAttribute
{
	public class TypeDescriptorOverridingProvider : TypeDescriptionProvider 
	{ 
		private readonly ICustomTypeDescriptor customTypeDecriptor; 
		public TypeDescriptorOverridingProvider(ICustomTypeDescriptor customTypeDescriptor) 
		{ 
			this.customTypeDecriptor = customTypeDescriptor; 
		}
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) 
			=> customTypeDecriptor;
	}
}
