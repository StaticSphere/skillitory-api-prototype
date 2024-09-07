using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Skillitory.Api.Services.Interfaces;

namespace Skillitory.Api.Features.Users.Queries;

public class GetUserInfoEndpoint : EndpointWithoutRequest<Results<NotFound, Ok<GetUserInfoResponse>>>
{
    private readonly IPrincipalService _principalService;
    private readonly IGetUserInfoDataService _getUserInfoDataService;

    public GetUserInfoEndpoint(IPrincipalService principalService, IGetUserInfoDataService getUserInfoDataService)
    {
        _principalService = principalService;
        _getUserInfoDataService = getUserInfoDataService;
    }

    public override void Configure()
    {
        Get("/users/info");
    }

    public override async Task<Results<NotFound, Ok<GetUserInfoResponse>>> ExecuteAsync(CancellationToken ct)
    {
        var userInfo = await _getUserInfoDataService.GetUserInfoAsync(_principalService.UserUniqueKey, ct);
        if (userInfo is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(userInfo);
    }
}
