using Demo.WebApi.Domain.Common.Events;

namespace Demo.WebApi.Application.Catalog.Products.EventHandlers;

public class ProductUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Product>>
{
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(ILogger<ProductUpdatedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityUpdatedEvent<Product> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}