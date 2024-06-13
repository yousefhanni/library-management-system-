using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Bookify.Web.Filters
{

    // This class is created to ensure that the request is an AJAX request only. 
    // It will block any other type of request from accessing the decorated action method.
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        // This method determines whether the action method is valid for the current request.
        // It overrides the IsValidForRequest method from the ActionMethodSelectorAttribute class.
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            // Get the current HTTP request.
            var request = routeContext.HttpContext.Request;

            // Check if the request contains the header "x-requested-with" with the value "XMLHttpRequest".
            // This header is typically added to AJAX requests made with jQuery.
            var isAjax = request.Headers["x-requested-with"] == "XMLHttpRequest";

            // Return true if the request is an AJAX request, otherwise return false.
            return isAjax;
        }
    }
}
