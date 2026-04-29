using System;

namespace API.Helpers;

public class FileUploadResult
{
    public bool IsSuccess { get; set; } = false;
    public required string Message { get; set; }

}
