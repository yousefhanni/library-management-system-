using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bookify.Web.Helpers
{
    ///purpose this class:
    ///The purpose of the ActiveTag class is to dynamically add an active CSS class to<a>
    ///elements based on the current controller name, enabling highlighting of the active navigation link.

    // This attribute specifies that the TagHelper targets <a> elements with an "active-when" attribute
    [HtmlTargetElement("a", Attributes = "active-when")]
    public class ActiveTag : TagHelper
    {
        // The attribute "active-when" is bound to this property. 
        // It specifies the controller name when the <a> tag should be active.
        public string? ActiveWhen { get; set; }

        // This property holds the ViewContext which provides access to the current HTTP context.
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContextData { get; set; }

        // This method processes the <a> tag and adds the "active" class if the conditions are met.
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // If the ActiveWhen property is null or empty, do nothing.
            if (string.IsNullOrEmpty(ActiveWhen))
                return;

            // Retrieve the current controller name from the route data.
            var currentController = ViewContextData?.RouteData.Values["controller"]?.ToString();

            // If the current controller matches the ActiveWhen value, add the "active" class to the <a> tag.
            if (currentController!.Equals(ActiveWhen))
            {
                // Check if the <a> tag already has a "class" attribute.
                if (output.Attributes.ContainsName("class"))
                    // Append "active" to the existing class attribute value.
                    output.Attributes.SetAttribute("class", $"{output.Attributes["class"].Value} active");
                else
                    // Add a new class attribute with the value "active".
                    output.Attributes.SetAttribute("class", "active");
            }
        }
    }
}
