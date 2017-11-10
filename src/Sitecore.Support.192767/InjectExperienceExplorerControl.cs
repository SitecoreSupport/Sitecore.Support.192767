using Sitecore;
using Sitecore.ExperienceExplorer.Business.Constants;
using Sitecore.ExperienceExplorer.Business.Helpers;
using Sitecore.Mvc.ExperienceEditor.Pipelines.RenderPageExtenders;
using Sitecore.Publishing;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace Sitecore.Support.ExperienceExplorer.Business.Pipelines.Mvc.RenderPageExtenders
{
    public class InjectExperienceExplorerControl : RenderPageExtendersProcessor
    {
        protected override bool Render(TextWriter writer)
        {
            if (!Context.IsLoggedIn)
            {
                PreviewManager.RestoreUser();
            }
            if (string.IsNullOrEmpty(PreviewManager.GetShellUser()) || Context.Item == null || !SettingsHelper.ExperienceModePipelineEnabled || !SettingsHelper.IsEnabledForCurrentSite || !PageModeHelper.IsExperienceMode || Context.Site.DisplayMode != Sitecore.Sites.DisplayMode.Normal)
            {
                return false;
            }
            System.Web.UI.Control control = new Sitecore.Web.UI.HtmlControls.Page().LoadControl(Paths.Module.Controls.GlobalHeaderPath);
            writer.Write(HtmlUtil.RenderControl(control));
            RenderPartial("ExperienceExplorerView", null, writer);
            return true;
        }

        public static void RenderPartial(string partialName, object model, TextWriter writer)
        {
            HttpContextWrapper httpContext = new HttpContextWrapper(HttpContext.Current);
            RouteData routeData = new RouteData
            {
                Values = { {
                    "controller",
                    "WebFormController"
                } }
            };
            ControllerContext controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new WebFormController());
            IView view = ViewEngines.Engines.FindPartialView(controllerContext, partialName).View;
            ViewDataDictionary viewData = new ViewDataDictionary
            {
                Model = model
            };
            ViewContext viewContext = new ViewContext(controllerContext, view, viewData, new TempDataDictionary(), writer);
            view.Render(viewContext, HttpContext.Current.Response.Output);
        }

        private class WebFormController : Controller
        {
        }
    }
}