using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Lists.Models;
using OrchardCore.Lists.ViewModels;
using YesSql;

namespace OrchardCore.Lists.Drivers
{
    public class ContainedPartDisplayDriver : ContentDisplayDriver
    {
        private readonly IContentManager _contentManager;
        private readonly ISession _session;

        public ContainedPartDisplayDriver(
            IContentManager contentManager,
            ISession session
            )
        {
            _session = session;
            _contentManager = contentManager;
        }

        protected override void BuildPrefix(ContentItem model, string htmlFieldPrefix)
        {
            Prefix = "ContainedPart";

            if (!string.IsNullOrEmpty(htmlFieldPrefix))
            {
                Prefix = htmlFieldPrefix + "." + Prefix;
            }
        }

        public override async Task<IDisplayResult> EditAsync(ContentItem model, IUpdateModel updater)
        {
            // This method can get called when a new content item is created, at that point
            // the query string contains a ListPart.ContainerId value, or when an
            // existing content item has ContainedPart value. In both cases the hidden field
            // needs to be rendered in the edit form to maintain the relationship with the parent.

            if (model.As<ContainedPart>() != null)
            {
                return BuildShape(model.As<ContainedPart>().ListContentItemId);
            }

            var viewModel = new EditContainedPartViewModel();
            if (await updater.TryUpdateModelAsync(viewModel, Prefix) && viewModel.ContainerId != null)
            {
                // We are editing a content item that needs to be added to a container
                // so we render the container id as part of the form

                return BuildShape(viewModel.ContainerId);
            }

            return null;
        }

        private IDisplayResult BuildShape(string containerId)
        {
            return Initialize<EditContainedPartViewModel>("ContainedPart_Edit", shape =>
            {
                shape.ContainerId = containerId;
            })
            .Location("Content");
        }

        public override async Task<IDisplayResult> UpdateAsync(ContentItem model, UpdateEditorContext context)
        {
            var viewModel = new EditContainedPartViewModel();
            BuildPrefix(model, context.HtmlFieldPrefix);

            if (await context.Updater.TryUpdateModelAsync(viewModel, Prefix) && viewModel.ContainerId != null)
            {
                model.Alter<ContainedPart>(x => x.ListContentItemId = viewModel.ContainerId);
            }

            return await EditAsync(model, context);
        }
    }
}