using System.Web.Optimization;

namespace Kursovaya
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // Для работы этих файлов очень важен порядок, так как у них есть явные зависимости
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // Так как bundle проще в переносе на Core, чем через ScriptManager
            var bootstrap = new ScriptBundle("~/bundles/bootstrap")
            .Include("~/Scripts/bootstrap.bundle.min.js");
            bootstrap.Transforms.Clear();
            bundles.Add(bootstrap);

            bundles.Add(new StyleBundle("~/Content/bootstrap")
                .Include("~/Content/bootstrap.min.css"));


            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-3.7.0.js"));


            bundles.Add(new ScriptBundle("~/bundles/jstree")
                .Include("~/Scripts/jstree.js"));

            bundles.Add(new StyleBundle("~/Content/jstree")
                .Include("~/Content/jstree/style.css"));


            bundles.Add(new ScriptBundle("~/bundles/custom")
                .Include("~/Scripts/custom.js"));

            bundles.Add(new StyleBundle("~/Content/style")
                .Include("~/Content/style.css"));


            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));
        }
    }
}