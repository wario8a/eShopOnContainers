﻿using Services.Common;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed((host) => true)
        .AllowCredentials());
});

builder.Services.AddSignalR(builder.Configuration);

builder.Services.AddSingleton<IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>, OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
builder.Services.AddSingleton<IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>, OrderStatusChangedToCancelledIntegrationEventHandler>();
builder.Services.AddSingleton<IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>, OrderStatusChangedToPaidIntegrationEventHandler>();
builder.Services.AddSingleton<IIntegrationEventHandler<OrderStatusChangedToShippedIntegrationEvent>, OrderStatusChangedToShippedIntegrationEventHandler>();
builder.Services.AddSingleton<IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
builder.Services.AddSingleton<IIntegrationEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>, OrderStatusChangedToSubmittedIntegrationEventHandler>();

var app = builder.Build();

if (!await app.CheckHealthAsync())
{
    return;
}

app.UseServiceDefaults();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationsHub>("/hub/notificationhub");

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderStatusChangedToAwaitingValidationIntegrationEvent, OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
eventBus.Subscribe<OrderStatusChangedToShippedIntegrationEvent, OrderStatusChangedToShippedIntegrationEventHandler>();
eventBus.Subscribe<OrderStatusChangedToCancelledIntegrationEvent, OrderStatusChangedToCancelledIntegrationEventHandler>();
eventBus.Subscribe<OrderStatusChangedToSubmittedIntegrationEvent, OrderStatusChangedToSubmittedIntegrationEventHandler>();

await app.RunAsync();
