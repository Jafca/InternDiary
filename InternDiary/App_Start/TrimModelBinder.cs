using System.ComponentModel;
using System.Web.Mvc;

namespace InternDiary
{
    public class TrimModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                var stringValue = (string)value;
                value = string.IsNullOrEmpty(stringValue) ? stringValue : stringValue.Trim();
            }

            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }
    }
}