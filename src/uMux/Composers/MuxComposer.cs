using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using uMux.Configuration;
using uMux.NotificationHandlers;
using uMux.Services;

namespace uMux.Composers;

public class MuxComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // load up the settings.
        var options = builder
            .Services.AddOptions<MuxSettings>()
            .Bind(builder.Config.GetSection(Constants.AppSettingsPath));

        options.ValidateDataAnnotations();

        builder.Services.AddScoped<IMuxService, MuxService>();

        builder.AddBackOfficeOpenApiDocument(
            Constants.Swagger.ApiName,
            configure => configure
                .WithTitle(Constants.Swagger.Title)
                .WithUiTitle(Constants.Swagger.Title)
                .WithBackOfficeAuthentication()
                .ConfigureOpenApiOptions(options =>
                    options.AddOperationTransformer<CustomOperationHandler>()));

        // media
        builder.AddNotificationAsyncHandler<MediaSavingNotification, MediaNotifications>();
        builder.AddNotificationAsyncHandler<MediaDeletedNotification, MediaNotifications>();

        // content
        builder.AddNotificationHandler<ContentCopyingNotification, ContentNotifications>();
        builder.AddNotificationAsyncHandler<
            ContentDeletedBlueprintNotification,
            ContentNotifications
        >();
        builder.AddNotificationAsyncHandler<ContentDeletedNotification, ContentNotifications>();
        builder.AddNotificationHandler<ContentSavedBlueprintNotification, ContentNotifications>();
        builder.AddNotificationAsyncHandler<ContentSavingNotification, ContentNotifications>();

        // member
        builder.AddNotificationAsyncHandler<MemberDeletedNotification, MemberNotifications>();
        builder.AddNotificationAsyncHandler<MemberSavingNotification, MemberNotifications>();

    }

    // This is used to generate nice operation IDs in our swagger json file
    // So that the gnerated TypeScript client has nice method names and not too verbose
    // https://docs.umbraco.com/umbraco-cms/tutorials/creating-a-backoffice-api/umbraco-schema-and-operation-ids#operation-ids
    public class CustomOperationHandler : IOpenApiOperationTransformer
    {
        public Task TransformAsync(
            Microsoft.OpenApi.OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            if (context.Description.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor
                && controllerActionDescriptor.ControllerTypeInfo.Namespace?.StartsWith(
                    "uMux.Controllers",
                    StringComparison.InvariantCultureIgnoreCase) is true)
            {
                operation.OperationId = context.Description.ActionDescriptor.RouteValues["action"];
            }

            return Task.CompletedTask;
        }
    }
}
