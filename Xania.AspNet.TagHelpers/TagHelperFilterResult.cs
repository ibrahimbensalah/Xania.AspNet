﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Xania.AspNet.TagHelpers
{
    public class TagHelperFilterResult : ActionResult
    {
        private readonly IDependencyResolver _defaultDependencyResolver;
        private readonly ActionResult _actionResult;
        private readonly IEnumerable<KeyValuePair<string, Type>> _customTagHandlers;

        public TagHelperFilterResult(IDependencyResolver defaultDependencyResolver, ActionResult actionResult, IEnumerable<KeyValuePair<string, Type>> customTagHandlers)
        {
            _defaultDependencyResolver = defaultDependencyResolver;
            _actionResult = actionResult;
            _customTagHandlers = customTagHandlers;
        }

        public override void ExecuteResult(ControllerContext controllerContext)
        {
            var tagHelperProvider = GetTagHelperProvider(_defaultDependencyResolver, controllerContext);

            var response = controllerContext.HttpContext.Response;
            if (response.Filter != null)
            {
                response.Filter = new HtmlProcessor(response.Output, tagHelperProvider);
            }
            else
            {
                response.Output = new TagHelperWriter(response.Output, tagHelperProvider);
            }
            _actionResult.ExecuteResult(controllerContext);
        }

        protected virtual ITagHelperProvider GetTagHelperProvider(IDependencyResolver resolver, ControllerContext controllerContext)
        {
            var helperContainer = new ActionTagHelperContainer(resolver, controllerContext);
            foreach (var kvp in GetTagHandlers())
                helperContainer.Register(kvp.Key, kvp.Value);

            if (_customTagHandlers != null)
            {
                foreach (var kvp in _customTagHandlers)
                    helperContainer.Register(kvp.Key, kvp.Value);
            }

            return helperContainer;
        }

        protected virtual IEnumerable<KeyValuePair<string, Type>> GetTagHandlers()
        {
            yield return new KeyValuePair<string, Type>("a", typeof(AnchorTagHelper));
        }
    }
}