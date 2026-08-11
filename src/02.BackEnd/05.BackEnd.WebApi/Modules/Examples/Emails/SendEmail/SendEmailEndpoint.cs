using EBVL.BackEnd.Logics.Modules.Examples.Emails.SendEmail;
using EBVL.Shared.Dto.Modules.Examples.Emails;
using EBVL.Shared.Dto.Modules.Examples.Emails.SendEmail;

namespace EBVL.BackEnd.WebApi.Modules.Examples.Emails.SendEmail;

public sealed class SendEmailEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(SendEmailRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(SendEmailRoute.Name)
            .WithDescription(SendEmailRoute.Description)
            .Produces<SendEmailResponse>();
    }

    private static async Task<IResult> Handle(
        [FromForm] SendEmailCommand request,
        IFormFileCollection? attachments,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var attachmentItems = new List<EmailAttachmentItem>();

        if (attachments is not null)
        {
            foreach (var attachment in attachments)
            {
                attachmentItems.Add(new EmailAttachmentItem
                {
                    FileName = attachment.FileName,
                    ContentType = attachment.ContentType,
                    FileContent = await attachment.ToBytesAsync(cancellationToken)
                });
            }
        }

        var command = new SendEmailCommand
        {
            Tos = request.Tos,
            Ccs = request.Ccs,
            Bccs = request.Bccs,
            Subject = request.Subject,
            Body = request.Body,
            Attachments = attachmentItems
        };

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
