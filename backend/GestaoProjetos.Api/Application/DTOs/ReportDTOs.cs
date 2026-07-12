namespace GestaoProjetos.Api.Application.DTOs;

public record DashboardSummary(
    int TotalCompleted,
    int TotalInDevelopment,
    int TotalPending,
    int TotalCritical
);

public record DeveloperWorkload(
    int DeveloperId,
    string DeveloperName,
    List<IssueResponse> Issues
);
