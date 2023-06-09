﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Smartstore.Core.Widgets;

namespace Smartstore.Web.Razor
{
    public interface IRazorViewInvoker
    {
        /// <summary>
        /// Invokes a view and returns its html content.
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <param name="isPartial"><c>true</c>: View is partial, otherwise main page.</param>
        /// <returns>View rendering result</returns>
        Task<string> InvokeViewAsync(string viewName, object model, bool isPartial = true);

        /// <summary>
        /// Invokes a view and returns its html content.
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="viewData">The <see cref="ViewDataDictionary"/> instance that also contains the model.</param>
        /// <param name="isPartial"><c>true</c>: View is partial, otherwise main page.</param>
        /// <returns>View rendering result</returns>
        Task<string> InvokeViewAsync(string viewName, ViewDataDictionary viewData, bool isPartial = true);

        /// <summary>
        /// Invokes a view component and returns its html content.
        /// </summary>
        /// <param name="componentName">The view component name.</param>
        /// <param name="viewData">View name</param>
        /// <param name="arguments">
        /// An <see cref="object"/> with properties representing arguments to be passed to the invoked view component
        /// method. Alternatively, an <see cref="System.Collections.Generic.IDictionary{String, Object}"/> instance
        /// containing the invocation arguments.
        /// </param>
        /// <returns>View component rendering result</returns>
        Task<string> InvokeViewComponentAsync(string componentName, ViewDataDictionary viewData, object arguments);

        /// <summary>
        /// Invokes a view component and returns its html content.
        /// </summary>
        /// <param name="componentType">The view component type.</param>
        /// <param name="viewData">View name</param>
        /// <param name="arguments">
        /// An <see cref="object"/> with properties representing arguments to be passed to the invoked view component
        /// method. Alternatively, an <see cref="System.Collections.Generic.IDictionary{String, Object}"/> instance
        /// containing the invocation arguments.
        /// </param>
        /// <returns>View component rendering result</returns>
        Task<string> InvokeViewComponentAsync(Type componentType, ViewDataDictionary viewData, object arguments);

        /// <summary>
        /// Invokes a widget and returns its html content.
        /// </summary>
        /// <param name="widget">Widget to invoke.</param>
        /// <returns>Widget rendering result</returns>
        Task<string> InvokeWidgetAsync(WidgetInvoker widget);
    }
}
