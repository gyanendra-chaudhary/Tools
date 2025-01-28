var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TextExtractor>("textextractor");


builder.Build().Run();
