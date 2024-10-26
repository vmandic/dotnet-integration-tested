namespace DotnetIntegrationTested.ExternalApis.Http.Wincher.V1.Endpoints.PostOnPageSeoChecks;

public sealed class PostOnPageSeoChecksRequestResponse
{
  public string? Id { get; set; }

  public bool Expired { get; set; }

  public OnPageSeoCheckScore? Score { get; set; }
}

public sealed class OnPageSeoCheckScore
{
  public string? Version { get; set; }

  public int? Score { get; set; }

  public DateTime DateAndTime { get; set; }

  public int ResponseTime { get; set; }

  public string? Status { get; set; }

  public string? ErrorReason { get; set; }

  public string? ResponseUrl { get; set; }

  public string? Url { get; set; }

  public string? Keyword { get; set; }

  public int MaxScore { get; set; }

  public IndexingInstructions? IndexingInstructions { get; set; }

  public List<OnPageSeoCheckCategory> Categories { get; set; } = new();
}

public sealed class IndexingInstructions
{
  public CanonicalInstructions? CanonicalInstructions { get; set; }

  public RobotInstructions? RobotInstructions { get; set; }
}

public sealed class CanonicalInstructions
{
  public bool CanonicallyRedirected { get; set; }

  public string? CanonicalUrl { get; set; }

  public string? Html { get; set; }
}

public sealed class RobotInstructions
{
  public bool Allow { get; set; }

  public string? MetaName { get; set; }

  public string? MetaContent { get; set; }

  public string? Html { get; set; }
}

public sealed class ReducedOnPageSeoCheckScore
{
  public string? Id { get; set; }

  public required string? Keyword { get; set; }

  public required string? Url { get; set; }

  public int Score { get; set; }

  public DateTime CreatedAt { get; set; }
}

public sealed class OnPageSeoCheckCategory
{
  public string? Key { get; set; }

  public string? Category { get; set; }

  public string? Title { get; set; }

  public string? Grade { get; set; }

  public int Score { get; set; }

  public int MaxScore { get; set; }

  public bool FullScore { get; set; }

  public string? BestPractice { get; set; }

  public List<OnPageSeoCheckRating> Ratings { get; set; } = new();
}

public sealed class OnPageSeoCheckRating
{
  public string? RatingId { get; set; }

  public string? RatingType { get; set; }

  public bool FullScore { get; set; }

  public string? RatingComment { get; set; }

  public string? Grade { get; set; }

  public int Score { get; set; }

  public int MaxScore { get; set; }
}
