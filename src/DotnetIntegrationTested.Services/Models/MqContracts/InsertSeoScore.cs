using DotnetIntegrationTested.ExternalApis.Http.Wincher.V1.Endpoints.PostOnPageSeoChecks;

namespace DotnetIntegrationTested.Services.Models.MqContracts;

public record InsertSeoScore(int UserId, OnPageSeoCheckScore SeoScoreData);
