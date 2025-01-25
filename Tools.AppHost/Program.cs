var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TextExtractor>("textextractor");

builder.AddProject<Projects.AiChatBot>("aichatbot");

builder.Build().Run();
