using cron_webapi.Service;
using cron_webapi.Worker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<CronJobScheduler>();
builder.Services.AddSingleton<ICronJobService, CronJobService>();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Cron/History", async (ICronJobService service) =>
{
    return await service.GetCronHistoricAsync();
})
.WithName("GetCronHistory");

app.MapPost("/Cron/ExeculteProcess", async (ICronJobService service) =>
{
    await service.ExeculteProcessAsync();
})
.WithName("PostCronExeculteProcess");

app.MapPut("/Cron/UpdateJobTimer", async ([FromBody] string timer, ICronJobService service) =>
{
    await service.UpdateJobTimerAsync(timer);
})
.WithName("PutCronUpdateJobTimer");

app.MapDelete("/Cron/DeleteHistoric/{id}", async ([FromRoute] Guid id, ICronJobService service) =>
{
    await service.DeleteHistoricAsync(id);
})
.WithName("DeleteHistoric");

app.MapDelete("/Cron/CleanHistoric", async (ICronJobService service) =>
{
    await service.CleanHistoricAsync();
})
.WithName("DeleteCleanHistoric");

app.Run();
