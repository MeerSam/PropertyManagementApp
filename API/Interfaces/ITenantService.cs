using System;

namespace API.Interfaces;

public interface ITenantService
{

    string GetCurrentClientId();

    string GetCurrentUserId();

    string GetCurrentMemberId();
    bool HasAccessToClient(string clientId);

}
