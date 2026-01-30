using System;

namespace API.Interfaces;

public interface ITenantService
{

    string GetCurrentClientId();
    bool HasAccessToClient(string clientId);

}
