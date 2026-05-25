using System;
using API.Entities;

namespace API.Helpers;

public sealed class UserAccess
{
    //sealed = cannot be inherited.
    public required string UserId { get; init; }
    public required string ClientId { get; init; }

    public string Role { get; init; } = default!;

    public IReadOnlyCollection<string> PropertyIdsOwned { get; init; } = [];

    public IReadOnlyCollection<string> PropertyIdsMember { get; init; } = [];
}

public enum ResourceType
{
    Document,
    Property,
    Member,
    User
}

public static class UserAccessMapper
{
    public static UserAccess ToUserAccess( UserClientAccess uca, 
        IEnumerable<PropertyOwnership> ownerships,
        IEnumerable<PropertyOwnership> memberships)
    {
        var ownedPropertyIds = ownerships
            .Where(o => o.IsCurrent && o.OwnershipType == OwnershipType.Primary)
            .Select(o => o.PropertyId)
            .Distinct()
            .ToArray();

        var memberPropertyIds = memberships
            .Where(o => o.IsCurrent)
            .Select(o => o.PropertyId)
            .Distinct()
            .ToArray();

        return new UserAccess
        {
            UserId = uca.UserId,
            ClientId = uca.ClientId,
            Role = uca.Role,
            PropertyIdsOwned = ownedPropertyIds,
            PropertyIdsMember = memberPropertyIds
        };
    }
}